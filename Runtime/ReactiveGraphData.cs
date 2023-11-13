using System.Collections.Generic;
using Reactive.Runtime.Nodes;
using UnityEngine;

namespace Reactive.Runtime
{
    /// <summary>
    /// Data class for storing a reactive graph in a serialized asset
    /// </summary>
    [System.Serializable]
    public class ReactiveGraphData : ScriptableObject
    {
        /// <summary>
        /// The GUID of the entry node in the graph
        /// </summary>
        public const string EntryNodeGuid = "__baseNode__";
        
        /// <summary>
        /// All the nodes in the graph
        /// </summary>
        [SerializeReference]
        public List<ReactiveNodeData> nodes = new();
        
        /// <summary>
        /// All the links between the graph's nodes
        /// </summary>
        public List<NodeLinkData> nodeLinks = new();
    }
}