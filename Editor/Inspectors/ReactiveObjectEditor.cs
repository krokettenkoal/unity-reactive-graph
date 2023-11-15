using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Reactive.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Reactive.Editor.Inspectors
{
    [CustomEditor(typeof(ReactiveObject))]
    public class ReactiveObjectEditor : UnityEditor.Editor
    {
        private SerializedProperty _graph;
        private SerializedProperty _sourceObject;
        private SerializedProperty _sourceObjectEditor;
        private SerializedProperty _sourceMemberName;
        
        private ReactiveObject Target => target as ReactiveObject;

        private PopupField<MemberInfo> _sourceMemberField;
        
        private const string StyleSheetName = "ReactiveObjectEditor";
        private const string HiddenClassName = "hidden";
        private const string InputObjectTooltip = "The source object that provides the input value for the reactive graph";
        private const string InputMemberTooltip = "The member (property, field or method) of the source object that provides the input value for the reactive graph";
        
        private void OnEnable()
        {
            _graph = serializedObject.FindProperty("graph");
            _sourceObject = serializedObject.FindProperty("sourceObject");
            _sourceObjectEditor = serializedObject.FindProperty("sourceObjectEditor");
            _sourceMemberName = serializedObject.FindProperty("sourceMemberName");
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var graphField = new PropertyField(_graph);
            var sourceWrapper = new VisualElement();
            var sourceLabel = new Label("Source");
            var sourceObjectField = new ObjectField
            {
                value = _sourceObjectEditor.objectReferenceValue,
                tooltip = InputObjectTooltip,
                objectType = typeof(Object),
            };
            _sourceMemberField = new PopupField<System.Reflection.MemberInfo>
            {
                name = "sourceMember",
                tooltip = InputMemberTooltip,
                formatListItemCallback = FormatMemberOption,
                formatSelectedValueCallback = FormatMemberSelection,
            };
            
            UpdateInputMemberField(Target.SourceMember);
            
            sourceLabel.AddToClassList("source-label");
            sourceWrapper.AddToClassList("source-wrapper");
            sourceObjectField.AddToClassList("source-field");
            _sourceMemberField.AddToClassList("source-field");

            sourceObjectField.RegisterValueChangedCallback(OnSourceObjectChanged);
            _sourceMemberField.RegisterValueChangedCallback(OnSourceMemberChanged);
            
            //Debug.Log(_sourceObjectEditor.objectReferenceValue);
            //Debug.Log(_sourceObject.objectReferenceValue);
            //Debug.Log(_sourceMemberName.stringValue);
            //Debug.Log(Target.SourceMember);

            sourceWrapper.Add(sourceObjectField);
            sourceWrapper.Add(_sourceMemberField);
            
            var styleSheet = Resources.Load<StyleSheet>(StyleSheetName);
            root.styleSheets.Add(styleSheet);
            
            root.Add(graphField);
            root.Add(sourceLabel);
            root.Add(sourceWrapper);

            return root;
        }
        
        private void OnSourceObjectChanged(ChangeEvent<Object> evt)
        {
            _sourceObjectEditor.objectReferenceValue = evt.newValue;
            _sourceObject.objectReferenceValue = evt.newValue;
            _sourceMemberName.stringValue = null;
            serializedObject.ApplyModifiedProperties();
            
            //Debug.Log("Source object changed!");
            
            UpdateInputMemberField();
        }
        
        private void OnSourceMemberChanged(ChangeEvent<MemberInfo> evt)
        {
            _sourceMemberName.stringValue = evt.newValue?.Name;
            
            // If the selected member is a member of a component, set the source object to the component on the source object
            if (evt.newValue != null && _sourceObjectEditor.objectReferenceValue is GameObject gameObject && typeof(Component).IsAssignableFrom(evt.newValue.DeclaringType))
            {
                _sourceObject.objectReferenceValue = gameObject.GetComponent(evt.newValue.DeclaringType);
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Enables/disables the input member field and populates it with the members of the input object
        /// </summary>
        private void UpdateInputMemberField([CanBeNull] MemberInfo currentMember = null)
        {
            if (!_sourceObjectEditor.objectReferenceValue)
            {
                _sourceMemberField.AddToClassList(HiddenClassName);
                return;
            }
            
            _sourceMemberField.choices = GetMembers(_sourceObjectEditor.objectReferenceValue);
            _sourceMemberField.value = currentMember;
            _sourceMemberField.RemoveFromClassList(HiddenClassName);
        }
        
        /// <summary>
        /// Retrieves all members of the specified object that have a (return) type of <c>float</c>
        /// </summary>
        /// <param name="obj">The object to retrieve members for</param>
        /// <returns>A list of members on <paramref name="obj"/> that have a (return) type of <c>float</c></returns>
        private static List<MemberInfo> GetMembers(Object obj)
        {
            if (!obj)
                return null;
            
            var members = new List<IEnumerable<MemberInfo>>();
            var objMembers = GetObjectMembers<float>(obj);
            
            members.Add(objMembers);

            if (obj is GameObject gameObject)
            {
                members.AddRange(gameObject.GetComponents<Component>().Select(GetObjectMembers<float>));
            }

            return members.SelectMany(m => m).ToList();
        }

        private static IEnumerable<MemberInfo> GetObjectMembers<T>(Object obj)
        {
            return obj.GetType().GetMembers().Where(m =>
            {
                return m.MemberType switch
                {
                    MemberTypes.Field => (m as FieldInfo)?.FieldType == typeof(T),
                    MemberTypes.Property => (m as PropertyInfo)?.PropertyType == typeof(T),
                    MemberTypes.Method => m is MethodInfo {IsSpecialName: false} methodInfo && methodInfo.ReturnType == typeof(T) && methodInfo.GetParameters().Length == 0,
                    _ => false
                };
            });
        }
        
        /// <summary>
        /// Formats a member option for the input member dropdown
        /// </summary>
        /// <param name="member">The member info to format</param>
        /// <returns>A string representation of the specified <paramref name="member"/></returns>
        private static string FormatMemberOption(MemberInfo member)
        {
            var prefix = member.DeclaringType != null ? member.DeclaringType.Name + "/" : "";
            var suffix = member.MemberType == MemberTypes.Method ? "()" : "";
            return prefix + member.Name + suffix;
        }
        
        /// <summary>
        /// Formats the selected member for the input member dropdown
        /// </summary>
        /// <param name="member">The member info to format</param>
        /// <returns>A string representation of the specified <paramref name="member"/></returns>
        private static string FormatMemberSelection(MemberInfo member)
        {
            if(member == null)
                return "None";
            
            var suffix = member.MemberType == MemberTypes.Method ? "()" : "";
            return member.Name + suffix;
        }
    }
}