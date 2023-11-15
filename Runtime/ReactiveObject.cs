using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reactive.Runtime.Nodes;
using Reactive.Runtime.Processors;
using UnityEngine;

namespace Reactive.Runtime
{
    /// <summary>
    /// Component for reactive objects. Reactive objects are objects that use a reactive graph to modify their properties.
    /// </summary>
    [AddComponentMenu("Reactive/Reactive Object")]
    public class ReactiveObject : MonoBehaviour
    {
        [SerializeField, Tooltip("The graph to use for this reactive object")]
        protected ReactiveGraphData graph;

        /// <summary>
        /// The processors required by the nodes in the <see cref="graph"/>
        /// </summary>
        private Dictionary<Type, ReactiveNodeProcessor> Processors { get; } = new();

        /// <summary>
        /// The source object that provides the input value for the reactive graph
        /// </summary>
        [SerializeReference] private UnityEngine.Object sourceObject;
        
        /// <summary>
        /// The source object that provides the input value for the reactive graph, only used in the editor
        /// </summary>
        /// <remarks>
        /// This field is only used in the editor to store a reference to the base object of <see cref="sourceObject"/>.
        /// This enables us to retrieve ALL members of the <see cref="sourceObject"/>, including component members.
        /// </remarks>
        [SerializeField] private UnityEngine.Object sourceObjectEditor;
        
        /// <summary>
        /// The name of the member (property, field or method) of the <see cref="sourceObject"/> that provides the input value for the reactive graph
        /// </summary>
        [SerializeField] private string sourceMemberName;
        
        /// <summary>
        /// The member (property, field or method) of the <see cref="sourceObject"/> that provides the input value for the reactive graph
        /// </summary>
        /// <remarks>The assigned member must have a (return) type of <c>float</c></remarks>
        public MemberInfo SourceMember => sourceObject ? sourceObject.GetType().GetMember(sourceMemberName).FirstOrDefault() : null;

        /// <summary>
        /// The validated <see cref="SourceMember"/> that is used to retrieve the input value.
        /// This is set inside <see cref="ValidateSource"/> to cache the validated member, avoiding redundant reflection calls in <see cref="GetInputValue"/>.
        /// </summary>
        private MemberInfo _validatedSourceMember;

        /// <summary>
        /// Initializes the reactive object by validating the required fields and setting up the required processors
        /// </summary>
        private void Awake()
        {
            if (!ValidateSource() || graph == null)
            {
                enabled = false;
                return;
            }
            
            _validatedSourceMember = SourceMember;
            
            foreach (var node in graph.nodes.Where(node => !Processors.ContainsKey(node.GetType())))
            {
                Processors.Add(node.GetType(), NodeProcessorFactory.CreateProcessor(node, gameObject));
            }
        }

        private void Update()
        {
            ProcessGraph();
        }

        /// <summary>
        /// Processes the whole graph by starting at the entry node and recursively processing all its (output) links
        /// </summary>
        /// <remarks>This method is called every frame</remarks>
        private void ProcessGraph()
        {
            foreach (var nodeLink in graph.nodeLinks.Where(link => link.baseNodeGuid == ReactiveGraphData.EntryNodeGuid))
            {
                var node = graph.nodes.Find(n => n.guid == nodeLink.targetNodeGuid);
                ProcessBranch(node, GetInputValue());
            }
        }

        /// <summary>
        /// Processes a node and all its (output) links recursively
        /// </summary>
        /// <param name="node">The node to process</param>
        /// <param name="inputValue">The input value provided to the specified <paramref name="node"/></param>
        private void ProcessBranch(ReactiveNodeData node, float inputValue)
        {
            var outputValue = ProcessNode(node, inputValue);
            foreach (var nodeLink in graph.nodeLinks.Where(link => link.baseNodeGuid == node.guid))
            {
                var next = graph.nodes.Find(n => n.guid == nodeLink.targetNodeGuid);
                ProcessBranch(next, outputValue);
            }
        }

        /// <summary>
        /// Processes a single reactive node by modifying the according properties and/or the output value
        /// </summary>
        /// <param name="node">The node to process</param>
        /// <param name="inputValue">The input value provided to the node</param>
        /// <returns>The value returned by the <paramref name="node"/>, based on the <paramref name="inputValue"/></returns>
        private float ProcessNode(ReactiveNodeData node, float inputValue)
        {
            return Processors.TryGetValue(node.GetType(), out var processor) ? processor.ProcessNode(node, inputValue) : inputValue;
        }

        /// <summary>
        /// Validates the <see cref="sourceObject"/> and <see cref="SourceMember"/> to ensure that they are not null and that the <see cref="SourceMember"/> has a (return) type of <c>float</c>
        /// </summary>
        /// <returns><c>true</c> if <see cref="sourceObject"/> and <see cref="SourceMember"/> are valid input sources, <c>false</c> otherwise.</returns>
        private bool ValidateSource()
        {
            if (sourceObject == null || SourceMember == null)
                return false;

            return SourceMember switch
            {
                PropertyInfo propertyInfo => propertyInfo.PropertyType == typeof(float),
                FieldInfo fieldInfo => fieldInfo.FieldType == typeof(float),
                MethodInfo methodInfo => methodInfo.ReturnType == typeof(float),
                _ => false
            };
        }

        private float GetInputValue()
        {
            return _validatedSourceMember switch
            {
                PropertyInfo propertyInfo => (float) propertyInfo.GetValue(sourceObject),
                FieldInfo fieldInfo => (float) fieldInfo.GetValue(sourceObject),
                MethodInfo methodInfo => (float) methodInfo.Invoke(sourceObject, null),
                _ => 0f
            };
        }
    }
}