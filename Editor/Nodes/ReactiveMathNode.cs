using Reactive.Runtime.Nodes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Reactive.Editor.Nodes
{
    /// <summary>
    /// A reactive node that modifies material properties
    /// </summary>
    public class ReactiveMathNode : ReactiveSignalNode
    {
        [Tooltip("The function to apply to the input value")]
        public ReactiveMathNodeFunction Function;
        [Tooltip("The first (additional) parameter supplied to the function")]
        public float Param1 = 0;
        [Tooltip("The second (additional) parameter supplied to the function")]
        public float Param2 = 1;
        
        public override Vector2 NodeSize { get; } = new(200, 200);

        private EnumField _functionField;
        private FloatField _param1Field;
        private FloatField _param2Field;

        public override ReactiveNodeData GetNodeData()
        {
            return new ReactiveMathNodeData
            {
                guid = Guid,
                type = Type,
                graphPosition = GetPosition().position,
                function = Function,
                param1 = Param1,
                param2 = Param2,
            };
        }

        public override void SetNodeData(ReactiveNodeData data)
        {
            if(data is not ReactiveMathNodeData mathData)
                return;

            Function = mathData.function;
            Param1 = mathData.param1;
            Param2 = mathData.param2;
            
            base.SetNodeData(data);
        }

        public override void AddControls()
        {
            base.AddControls();
            
            //  Construct control fields
            _functionField = new EnumField(nameof(Function), Function)
            {
                tooltip = ReactiveNodeFactory.GetNodeControlTooltip(this, nameof(Function))
            };
            _param1Field = new FloatField("Param 1")
            {
                value = Param1,
                tooltip = ReactiveNodeFactory.GetNodeControlTooltip(this, nameof(Param1))
            };
            _param2Field = new FloatField("Param 2")
            {
                value = Param2,
                tooltip = ReactiveNodeFactory.GetNodeControlTooltip(this, nameof(Param2))
            };
            //  Attach event listeners
            _functionField.RegisterValueChangedCallback(evt =>
            {
                Function = (ReactiveMathNodeFunction) evt.newValue;
                OnTargetValueChanged();
            });
            _param1Field.RegisterValueChangedCallback(evt =>
            {
                Param1 = evt.newValue;
            });
            _param2Field.RegisterValueChangedCallback(evt =>
            {
                Param2 = evt.newValue;
            });
            
            OnTargetValueChanged();
            
            //  Add fields to node
            extensionContainer.Add(_functionField);
            extensionContainer.Add(_param1Field);
            extensionContainer.Add(_param2Field);
        }

        public override void AddClassNames()
        {
            base.AddClassNames();
            AddToClassList("node-math");
        }

        protected override void UpdateFields()
        {
            _functionField?.SetValueWithoutNotify(Function);
            _param1Field?.SetValueWithoutNotify(Param1);
            _param2Field?.SetValueWithoutNotify(Param2);
            
            OnTargetValueChanged();
        }

        private void OnTargetValueChanged()
        {
            _param1Field.visible = RequiresParam1(Function);
            _param1Field.label = GetParam1Label(Function);
            _param2Field.visible = RequiresParam2(Function);
            _param2Field.label = GetParam2Label(Function);
        }

        private static bool RequiresParam1(ReactiveMathNodeFunction fn)
        {
            return fn switch
            {
                ReactiveMathNodeFunction.Pow => true,
                ReactiveMathNodeFunction.Clamp => true,
                ReactiveMathNodeFunction.Min => true,
                ReactiveMathNodeFunction.Max => true,
                ReactiveMathNodeFunction.Add => true,
                ReactiveMathNodeFunction.Multiply => true,
                _ => false
            };
        }

        private static bool RequiresParam2(ReactiveMathNodeFunction fn)
        {
            return fn switch
            {
                ReactiveMathNodeFunction.Clamp => true,
                _ => false
            };
        }

        private static string GetParam1Label(ReactiveMathNodeFunction fn)
        {
            return fn switch
            {
                ReactiveMathNodeFunction.Pow => "Exponent",
                ReactiveMathNodeFunction.Clamp => "Min",
                ReactiveMathNodeFunction.Min => "Compare",
                ReactiveMathNodeFunction.Max => "Compare",
                ReactiveMathNodeFunction.Add => "Summand",
                ReactiveMathNodeFunction.Multiply => "Factor",
                _ => "Param 1"
            };
        }

        private static string GetParam2Label(ReactiveMathNodeFunction fn)
        {
            return fn switch
            {
                ReactiveMathNodeFunction.Clamp => "Max",
                _ => "Param 2"
            };
        }
    }
}