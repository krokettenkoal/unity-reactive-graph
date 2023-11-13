using System.Collections.Generic;
using UnityEngine;

namespace Reactive.Runtime.Nodes
{
    /// <summary>
    /// Data class for reactive material nodes, used to serialize the graph
    /// </summary>
    [System.Serializable]
    public class ReactiveMaterialNodeData : ReactiveNodeData
    {
        public const string EmissionColorPropertyName = "_EmissionColor";
        
        [Tooltip("The target properties to modify")]
        public ReactiveMaterialProperty property;
        [Tooltip("The colors to interpolate between based on the input value")]
        public Gradient color;
        [Tooltip("The names of the custom properties to modify")]
        public List<string> customPropertyNames = new();
        [Tooltip("The values of the custom properties to modify")]
        public List<Gradient> customPropertyValues = new();
    }
}