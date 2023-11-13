using UnityEngine;

namespace Reactive.Runtime.Nodes
{
    /// <summary>
    /// Data class for reactive math nodes, used to serialize the graph
    /// </summary>
    [System.Serializable]
    public class ReactiveMathNodeData : ReactiveNodeData
    {
        [Tooltip("The function to apply to the input value")]
        public ReactiveMathNodeFunction function;
        [Tooltip("The first (additional) parameter supplied to the function")]
        public float param1 = 0;
        [Tooltip("The second (additional) parameter supplied to the function")]
        public float param2 = 1;
    }
}