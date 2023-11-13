using Reactive.Runtime.Nodes;
using UnityEngine;

namespace Reactive.Runtime.Processors
{
    /// <summary>
    /// Processor for reactive math nodes
    /// </summary>
    public class ReactiveMathNodeProcessor : ReactiveNodeProcessor
    {
        public ReactiveMathNodeProcessor(GameObject gameObject) : base(gameObject) { }
        
        public override float ProcessNode(ReactiveNodeData node, float inputValue)
        {
            if (node is not ReactiveMathNodeData nodeData)
                return inputValue;
            
            return nodeData.function switch
            {
                ReactiveMathNodeFunction.Sin => Mathf.Sin(inputValue),
                ReactiveMathNodeFunction.Cos => Mathf.Cos(inputValue),
                ReactiveMathNodeFunction.Tan => Mathf.Tan(inputValue),
                ReactiveMathNodeFunction.Abs => Mathf.Abs(inputValue),
                ReactiveMathNodeFunction.Floor => Mathf.Floor(inputValue),
                ReactiveMathNodeFunction.Ceil => Mathf.Ceil(inputValue),
                ReactiveMathNodeFunction.Round => Mathf.Round(inputValue),
                ReactiveMathNodeFunction.Sqrt => Mathf.Sqrt(inputValue),
                ReactiveMathNodeFunction.Pow => Mathf.Pow(inputValue, nodeData.param1),
                ReactiveMathNodeFunction.Log => Mathf.Log(inputValue),
                ReactiveMathNodeFunction.Log10 => Mathf.Log10(inputValue),
                ReactiveMathNodeFunction.Clamp => Mathf.Clamp(inputValue, nodeData.param1, nodeData.param2),
                ReactiveMathNodeFunction.Clamp01 => Mathf.Clamp01(inputValue),
                ReactiveMathNodeFunction.Min => Mathf.Min(inputValue, nodeData.param1),
                ReactiveMathNodeFunction.Max => Mathf.Max(inputValue, nodeData.param1),
                ReactiveMathNodeFunction.Add => inputValue + nodeData.param1,
                ReactiveMathNodeFunction.Multiply => inputValue * nodeData.param1,
                _ => inputValue
            };
        }
    }
}