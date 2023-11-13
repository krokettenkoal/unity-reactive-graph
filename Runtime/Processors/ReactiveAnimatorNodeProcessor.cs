using Reactive.Runtime.Nodes;
using UnityEngine;

namespace Reactive.Runtime.Processors
{
    /// <summary>
    /// Processor for reactive animator nodes
    /// </summary>
    public class ReactiveAnimatorNodeProcessor : ReactiveNodeProcessor
    {
        /// <summary>
        /// The animator component to set parameters on
        /// </summary>
        private readonly Animator _animator;

        public ReactiveAnimatorNodeProcessor(GameObject gameObject) : base(gameObject)
        {
            _animator = gameObject.GetComponent<Animator>();
        }
        
        public override float ProcessNode(ReactiveNodeData node, float inputValue)
        {
            if (node is not ReactiveAnimatorNodeData nodeData)
                return inputValue;

            foreach (var parameterName in nodeData.parameterNames)
            {
                _animator.SetFloat(parameterName, inputValue);
            }

            return inputValue;
        }
    }
}