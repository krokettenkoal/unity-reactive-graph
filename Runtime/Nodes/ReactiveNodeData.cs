using System;
using UnityEngine;

namespace Reactive.Runtime.Nodes
{
    /// <summary>
    /// Data class for reactive nodes, used to serialize the graph
    /// </summary>
    [System.Serializable]
    public class ReactiveNodeData
    {
        [Tooltip("The unique identifier of the serialized node")]
        public string guid;
        [Tooltip("The type of the serialized node")]
        public ReactiveNodeType type;
        [Tooltip("The position of the serialized node in the graph")]
        public Vector2 graphPosition;
        
        public static T ConvertNodeData<T>(ReactiveNodeData source) where T : ReactiveNodeData
        {
            var target = (T) Activator.CreateInstance(typeof(T));
            target.guid = source.guid;
            target.type = source.type;
            target.graphPosition = source.graphPosition;
            return target;
        }
    }
}