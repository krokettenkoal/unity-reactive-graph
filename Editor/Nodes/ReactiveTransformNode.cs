using Reactive.Runtime.Nodes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Reactive.Editor.Nodes
{
    /// <summary>
    /// A reactive node that modifies transform properties
    /// </summary>
    public class ReactiveTransformNode : ReactiveEffectNode
    {
        [Tooltip("The directional Vector to add to the according transform property, multiplied with the input value")]
        public Vector3 Direction = Vector3.zero;
        [Tooltip("When set to true, the target property will be anchored to its original value, falling back to the original value when the input value is 0. When set to false, the target property will not fall back, and will stay at the last value when the input value is 0.")]
        public bool Anchor = false;

        public override Vector2 NodeSize { get; } = new(250, 200);

        private Vector3Field _directionField;
        private Toggle _anchorField;
        
        public override ReactiveNodeData GetNodeData()
        {
            return new ReactiveTransformNodeData
            {
                guid = Guid,
                type = Type,
                graphPosition = GetPosition().position,
                direction = Direction,
                anchor = Anchor,
            };
        }

        public override void SetNodeData(ReactiveNodeData data)
        {
            if(data is not ReactiveTransformNodeData transformData)
                return;
            
            Direction = transformData.direction;
            Anchor = transformData.anchor;
            
            base.SetNodeData(data);
        }

        public override void AddControls()
        {
            base.AddControls();
            
            _directionField = new Vector3Field
            {
                label = nameof(Direction),
                value = Direction,
                tooltip = ReactiveNodeFactory.GetNodeControlTooltip(this, nameof(Direction))
            };
            _anchorField = new Toggle
            {
                label = nameof(Anchor),
                value = Anchor,
                tooltip = ReactiveNodeFactory.GetNodeControlTooltip(this, nameof(Anchor))
            };
            
            _directionField.RegisterValueChangedCallback(evt =>
            {
                Direction = evt.newValue;
            });
            _anchorField.RegisterValueChangedCallback(evt =>
            {
                Anchor = evt.newValue;
            });
            
            extensionContainer.Add(_directionField);
            extensionContainer.Add(_anchorField);
        }

        public override void AddClassNames()
        {
            base.AddClassNames();
            AddToClassList("node-transform");
        }

        protected override void UpdateFields()
        {
            _directionField?.SetValueWithoutNotify(Direction);
            _anchorField?.SetValueWithoutNotify(Anchor);
        }
    }
}