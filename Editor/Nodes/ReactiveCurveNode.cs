using Reactive.Runtime.Nodes;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Reactive.Editor.Nodes
{
    /// <summary>
    /// A reactive node that modifies the input value
    /// </summary>
    public class ReactiveCurveNode : ReactiveSignalNode
    {
        [Tooltip("The curve to apply to the input value")]
        public AnimationCurve Curve;

        private CurveField _curveField;

        public override ReactiveNodeData GetNodeData()
        {
            return new ReactiveCurveNodeData
            {
                guid = Guid,
                type = Type,
                graphPosition = GetPosition().position,
                curve = Curve,
            };
        }

        public override void SetNodeData(ReactiveNodeData data)
        {
            if(data is not ReactiveCurveNodeData curveData)
                return;
            
            Curve = curveData.curve;
            
            base.SetNodeData(data);
        }

        public override void AddControls()
        {
            base.AddControls();
            
            _curveField = new CurveField
            {
                label = "Curve",
                value = Curve,
                tooltip = ReactiveNodeFactory.GetNodeControlTooltip(this, nameof(Curve))
            };
            
            _curveField.RegisterValueChangedCallback(evt =>
            {
                Curve = evt.newValue;
            });
            
            extensionContainer.Add(_curveField);
        }

        public override void AddClassNames()
        {
            base.AddClassNames();
            AddToClassList("node-curve");
        }

        protected override void UpdateFields()
        {
            _curveField?.SetValueWithoutNotify(Curve);
        }
    }
}