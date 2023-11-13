using System.Collections.Generic;
using Reactive.Runtime.Nodes;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Reactive.Editor.Nodes
{
    /// <summary>
    /// A reactive node that modifies material properties
    /// </summary>
    public class ReactiveMaterialNode : ReactiveEffectNode
    {
        [Tooltip("The target properties to modify")]
        public ReactiveMaterialProperty Properties;
        [Tooltip("The colors to interpolate between based on the input value")]
        public Gradient Color;
        [Tooltip("The names of the custom properties to modify")]
        public List<string> CustomPropertyNames = new();
        [Tooltip("The values of the custom properties to modify")]
        public List<Gradient> CustomPropertyValues = new();
        
        public override Vector2 NodeSize { get; } = new(250, 200);
        
        private const string CustomPropertyDefaultName = "_CustomProperty";

        private EnumFlagsField _targetField;
        private GradientField _gradientField;
        private Label _customPropertiesLabel;
        private VisualElement _customPropertyFields;
        private Button _addCustomPropertyButton;

        public override ReactiveNodeData GetNodeData()
        {
            return new ReactiveMaterialNodeData
            {
                guid = Guid,
                type = Type,
                graphPosition = GetPosition().position,
                property = Properties,
                color = Color,
                customPropertyNames = CustomPropertyNames,
                customPropertyValues = CustomPropertyValues,
            };
        }

        public override void SetNodeData(ReactiveNodeData data)
        {
            if(data is not ReactiveMaterialNodeData materialData)
                return;

            Properties = materialData.property;
            Color = materialData.color;
            CustomPropertyNames = materialData.customPropertyNames;
            CustomPropertyValues = materialData.customPropertyValues;
            
            base.SetNodeData(data);
        }

        public override void AddControls()
        {
            base.AddControls();
            
            //  Construct control fields
            _targetField = new EnumFlagsField("Properties", Properties)
            {
                tooltip = ReactiveNodeFactory.GetNodeControlTooltip(this, nameof(Properties))
            };
            _gradientField = new GradientField("Colors")
            {
                value = Color,
                tooltip = ReactiveNodeFactory.GetNodeControlTooltip(this, nameof(Color))
            };
            _customPropertiesLabel = new Label("Custom Properties");
            _customPropertiesLabel.AddToClassList("custom-properties-label");
            _customPropertyFields = new VisualElement();
            CreateCustomPropertyFields();
            _addCustomPropertyButton = new Button(AddCustomProperty)
            {
                text = "+",
                tooltip = "Add a custom property to modify"
            };
            
            //  Attach event listeners
            _targetField.RegisterValueChangedCallback(evt =>
            {
                Properties = (ReactiveMaterialProperty) evt.newValue;
            });
            _gradientField.RegisterValueChangedCallback(evt =>
            {
                Color = evt.newValue;
            });
            
            //  Add fields to node
            extensionContainer.Add(_targetField);
            extensionContainer.Add(_gradientField);
            extensionContainer.Add(_customPropertiesLabel);
            extensionContainer.Add(_customPropertyFields);
            extensionContainer.Add(_addCustomPropertyButton);
        }

        public override void AddClassNames()
        {
            base.AddClassNames();
            AddToClassList("node-material");
        }

        protected override void UpdateFields()
        {
            _targetField?.SetValueWithoutNotify(Properties);
            _gradientField?.SetValueWithoutNotify(Color);
            
            CreateCustomPropertyFields();
        }
        
        private void AddCustomProperty()
        {
            CustomPropertyNames.Add(CustomPropertyDefaultName);
            CustomPropertyValues.Add(new Gradient());
            var elem = CreateCustomPropertyField();
            _customPropertyFields.Add(elem);
        }
        
        private void CreateCustomPropertyFields()
        {
            _customPropertyFields.Clear();
            
            for (var i = 0; i < CustomPropertyNames.Count; i++)
            {
                var elem = CreateCustomPropertyField(i);
                _customPropertyFields.Add(elem);
            }
        }
        
        private VisualElement CreateCustomPropertyField() => CreateCustomPropertyField(CustomPropertyNames.Count - 1);

        private VisualElement CreateCustomPropertyField(int idx)
        {
            if(idx < 0 || idx >= CustomPropertyNames.Count)
                return null;
            
            var propName = CustomPropertyNames[idx];
            var propVal = CustomPropertyValues[idx];
            var wrapper = new VisualElement();
            var nameField = new TextField
            {
                value = propName,
                tooltip = "The name of the custom property to modify"
            };
            var gradientField = new GradientField
            {
                value = propVal,
                tooltip = "Colors to interpolate between based on the input value"
            };
            
            nameField.RegisterValueChangedCallback(evt =>
            {
                CustomPropertyNames[idx] = evt.newValue;
            });
            gradientField.RegisterValueChangedCallback(evt =>
            {
                CustomPropertyValues[idx] = evt.newValue;
            });
            
            wrapper.AddToClassList("custom-property-field");
            wrapper.Add(nameField);
            wrapper.Add(gradientField);

            return wrapper;
        }
    }
}