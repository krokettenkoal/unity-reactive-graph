using UnityEngine;

namespace Reactive.Runtime.Nodes
{
    /// <summary>
    /// Data class for reactive transform nodes, used to serialize the graph
    /// </summary>
    [System.Serializable]
    public class ReactiveTransformNodeData : ReactiveNodeData
    {
        [Tooltip("The directional Vector to add to the according transform property, multiplied with the input value")]
        public Vector3 direction = Vector3.zero;
        [Tooltip("When set to true, the target property will be anchored to its original value, falling back to the original value when the input value is 0. When set to false, the target property will not fall back, and will stay at the last value when the input value is 0.")]
        public bool anchor = false;
    }
}