using UnityEngine;

namespace Reactive.Runtime.Nodes
{
    /// <summary>
    /// Data class for reactive curve nodes, used to serialize the graph
    /// </summary>
    [System.Serializable]
    public class ReactiveCurveNodeData : ReactiveNodeData
    {
        [Tooltip("The curve to apply to the input value")]
        public AnimationCurve curve;
    }
}