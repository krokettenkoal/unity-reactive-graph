using Reactive.Runtime.Nodes;
using UnityEngine;

namespace Reactive.Runtime.Processors
{
    /// <summary>
    /// Processor for reactive curve nodes
    /// </summary>
    public class ReactiveCurveNodeProcessor : ReactiveNodeProcessor
    {
        public ReactiveCurveNodeProcessor(GameObject gameObject) : base(gameObject) { }
        
        public override float ProcessNode(ReactiveNodeData node, float inputValue)
        {
            if (node is not ReactiveCurveNodeData nodeData)
                return inputValue;
            
            return nodeData.type switch
            {
                ReactiveNodeType.Curve => nodeData.curve.Evaluate(inputValue),
                _ => inputValue
            };
        }
    }
}