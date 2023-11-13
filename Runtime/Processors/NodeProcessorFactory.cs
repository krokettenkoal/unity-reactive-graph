using Reactive.Runtime.Nodes;
using UnityEngine;

namespace Reactive.Runtime.Processors
{
    /// <summary>
    /// Factory class for creating node processors
    /// </summary>
    public static class NodeProcessorFactory
    {
        /// <summary>
        /// Creates a node processor for the given <paramref name="node"/> on the given <paramref name="gameObject"/>
        /// </summary>
        /// <param name="node">The node to create the processor for</param>
        /// <param name="gameObject">The gameObject the processor will be running for</param>
        /// <returns>A new processor for the specified <paramref name="node"/> and <paramref name="gameObject"/></returns>
        public static ReactiveNodeProcessor CreateProcessor(ReactiveNodeData node, GameObject gameObject)
        {
            return node switch
            {
                ReactiveMaterialNodeData _ => new ReactiveMaterialNodeProcessor(gameObject),
                ReactiveTransformNodeData _ => new ReactiveTransformNodeProcessor(gameObject),
                ReactiveCurveNodeData _ => new ReactiveCurveNodeProcessor(gameObject),
                ReactiveMathNodeData _ => new ReactiveMathNodeProcessor(gameObject),
                ReactiveAnimatorNodeData _ => new ReactiveAnimatorNodeProcessor(gameObject),
                _ => new ReactiveNodeProcessor(gameObject)
            };
        }
    }
}