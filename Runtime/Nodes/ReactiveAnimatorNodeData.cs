using System.Collections.Generic;
using UnityEngine;

namespace Reactive.Runtime.Nodes
{
    /// <summary>
    /// Data class for reactive animator nodes, used to serialize the graph
    /// </summary>
    [System.Serializable]
    public class ReactiveAnimatorNodeData : ReactiveNodeData
    {
        [Tooltip("The names of the parameters to modify")]
        public List<string> parameterNames = new();
    }
}