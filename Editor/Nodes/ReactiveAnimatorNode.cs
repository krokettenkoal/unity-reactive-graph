using System.Collections.Generic;
using Reactive.Runtime.Nodes;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Reactive.Editor.Nodes
{
    /// <summary>
    /// A reactive node that modifies animator parameters
    /// </summary>
    public class ReactiveAnimatorNode : ReactiveEffectNode
    {
        [Tooltip("The names of the parameters to modify")]
        public List<string> ParameterNames = new();
        
        public override Vector2 NodeSize { get; } = new(250, 200);
        
        private const string ParameterDefaultName = "New Float";

        private Label _parametersLabel;
        private VisualElement _parameterFields;
        private Button _addParameterButton;

        public override ReactiveNodeData GetNodeData()
        {
            return new ReactiveAnimatorNodeData
            {
                guid = Guid,
                type = Type,
                graphPosition = GetPosition().position,
                parameterNames = ParameterNames,
            };
        }

        public override void SetNodeData(ReactiveNodeData data)
        {
            if(data is not ReactiveMaterialNodeData materialData)
                return;

            ParameterNames = materialData.customPropertyNames;
            
            base.SetNodeData(data);
        }

        public override void AddControls()
        {
            base.AddControls();
            
            //  Construct control fields
            _parametersLabel = new Label("Parameters");
            _parametersLabel.AddToClassList("parameters-label");
            _parameterFields = new VisualElement();
            CreateParameterFields();
            _addParameterButton = new Button(AddParameter)
            {
                text = "+",
                tooltip = "Add a parameter to modify"
            };
            
            //  Add fields to node
            extensionContainer.Add(_parametersLabel);
            extensionContainer.Add(_parameterFields);
            extensionContainer.Add(_addParameterButton);
        }

        public override void AddClassNames()
        {
            base.AddClassNames();
            AddToClassList("node-animator");
        }

        protected override void UpdateFields()
        {
            CreateParameterFields();
        }
        
        private void AddParameter()
        {
            ParameterNames.Add(ParameterDefaultName);
            var elem = CreateParameterField();
            _parameterFields.Add(elem);
        }
        
        private void CreateParameterFields()
        {
            _parameterFields.Clear();
            
            for (var i = 0; i < ParameterNames.Count; i++)
            {
                var elem = CreateParameterField(i);
                _parameterFields.Add(elem);
            }
        }
        
        private VisualElement CreateParameterField() => CreateParameterField(ParameterNames.Count - 1);

        private VisualElement CreateParameterField(int idx)
        {
            if(idx < 0 || idx >= ParameterNames.Count)
                return null;
            
            var paramName = ParameterNames[idx];
            var paramField = new TextField
            {
                value = paramName,
                tooltip = "The name of the parameter to modify"
            };
            
            paramField.RegisterValueChangedCallback(evt =>
            {
                ParameterNames[idx] = evt.newValue;
            });
            
            return paramField;
        }
    }
}