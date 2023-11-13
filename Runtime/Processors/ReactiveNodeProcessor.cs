using Reactive.Runtime.Nodes;
using UnityEngine;

namespace Reactive.Runtime.Processors
{
    /// <summary>
    /// Base class for reactive node processors
    /// </summary>
    public class ReactiveNodeProcessor
    {
        /// <summary>
        /// The target game object to apply processing on
        /// </summary>
        protected GameObject Target;
        
        public ReactiveNodeProcessor(GameObject gameObject)
        {
            Target = gameObject;
        }
        
        /// <summary>
        /// Processes the given <paramref name="node"/> with the given <paramref name="inputValue"/>
        /// </summary>
        /// <param name="node">The node to process</param>
        /// <param name="inputValue">The input value provided to the processor</param>
        /// <returns>The new (output) value after processing</returns>
        public virtual float ProcessNode(ReactiveNodeData node, float inputValue)
        {
            return inputValue;
        }

    }
}