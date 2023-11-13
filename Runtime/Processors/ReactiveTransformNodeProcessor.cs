using Reactive.Runtime.Nodes;
using UnityEngine;

namespace Reactive.Runtime.Processors
{
    /// <summary>
    /// Processor for reactive transform nodes
    /// </summary>
    public class ReactiveTransformNodeProcessor : ReactiveNodeProcessor
    {
        /// <summary>
        /// The transform component to modify
        /// </summary>
        private readonly Transform _targetTransform;
        
        /// <summary>
        /// The <see cref="_targetTransform"/>'s initial local position before any processing is applied
        /// </summary>
        private readonly Vector3 _initialPosition;
        
        /// <summary>
        /// The <see cref="_targetTransform"/>'s initial rotation (local euler angles) before any processing is applied
        /// </summary>
        private readonly Vector3 _initialRotation;
        
        /// <summary>
        /// The <see cref="_targetTransform"/>'s initial local scale before any processing is applied
        /// </summary>
        private readonly Vector3 _initialScale;

        public ReactiveTransformNodeProcessor(GameObject gameObject) : base(gameObject)
        {
            _targetTransform = gameObject.transform;
            _initialPosition = _targetTransform.localPosition;
            _initialRotation = _targetTransform.localEulerAngles;
            _initialScale = _targetTransform.localScale;
        }
        
        public override float ProcessNode(ReactiveNodeData node, float inputValue)
        {
            if (node is not ReactiveTransformNodeData nodeData)
                return inputValue;
            
            return nodeData.type switch
            {
                ReactiveNodeType.TransformPosition => ProcessPosition(nodeData, inputValue),
                ReactiveNodeType.TransformRotation => ProcessRotation(nodeData, inputValue),
                ReactiveNodeType.TransformScale => ProcessScale(nodeData, inputValue),
                _ => inputValue
            };
        }

        /// <summary>
        /// Processes the <see cref="_targetTransform"/>'s position, based on the given <paramref name="inputValue"/>
        /// </summary>
        /// <param name="node">The node data to retrieve the position direction from</param>
        /// <param name="inputValue">The input value provided to node</param>
        /// <returns>
        /// The output value after processing.
        /// Currently, no input value modifications are supported. Therefore, the output value always equals the input value.
        /// </returns>
        private float ProcessPosition(ReactiveTransformNodeData node, float inputValue)
        {
            var delta = node.direction * inputValue;
            var origin = node.anchor ? _initialPosition : _targetTransform.localPosition;
            _targetTransform.localPosition = origin + delta;
            return inputValue;
        }

        /// <summary>
        /// Processes the <see cref="_targetTransform"/>'s rotation, based on the given <paramref name="inputValue"/>
        /// </summary>
        /// <param name="node">The node data to retrieve the rotation direction from</param>
        /// <param name="inputValue">The input value provided to node</param>
        /// <returns>
        /// The output value after processing.
        /// Currently, no input value modifications are supported. Therefore, the output value always equals the input value.
        /// </returns>
        private float ProcessRotation(ReactiveTransformNodeData node, float inputValue)
        {
            var delta = node.direction * inputValue;
            var origin = node.anchor ? _initialRotation : _targetTransform.localEulerAngles;
            _targetTransform.localEulerAngles = origin + delta;
            return inputValue;
        }

        /// <summary>
        /// Processes the <see cref="_targetTransform"/>'s scale, based on the given <paramref name="inputValue"/>
        /// </summary>
        /// <param name="node">The node data to retrieve the scale direction from</param>
        /// <param name="inputValue">The input value provided to node</param>
        /// <returns>
        /// The output value after processing.
        /// Currently, no input value modifications are supported. Therefore, the output value always equals the input value.
        /// </returns>
        private float ProcessScale(ReactiveTransformNodeData node, float inputValue)
        {
            var delta = node.direction * inputValue;
            var origin = node.anchor ? _initialScale : Vector3.zero;
            _targetTransform.localScale = origin + delta;
            return inputValue;
        }
    }
}