using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Runtime.Nodes;
using Reactive.Runtime.Processors;
using UnityEngine;

namespace Reactive.Runtime
{
    /// <summary>
    /// Base class for reactive objects. Reactive objects are objects that use a reactive graph to modify their properties.
    /// </summary>
    public abstract class ReactiveObject : MonoBehaviour
    {
        [SerializeField, Tooltip("The graph to use for this reactive object")]
        protected ReactiveGraphData graph;

        /// <summary>
        /// The processors required by the nodes in the <see cref="graph"/>
        /// </summary>
        private Dictionary<Type, ReactiveNodeProcessor> Processors { get; } = new();
        
        /// <summary>
        /// The input source for this reactive object, providing a starting value for the graph retrieved every frame
        /// </summary>
        protected IReactiveInputSource Input { get; set; }

        /// <summary>
        /// Initializes the reactive object by validating the required fields and setting up the required processors
        /// </summary>
        /// <remarks>When overriding this method, always make sure to call <c>base.Awake()</c></remarks>
        protected virtual void Awake()
        {
            if (Input == null || graph == null)
            {
                enabled = false;
                return;
            }
            
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
                ProcessBranch(node, Input.Value);
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
    }
}