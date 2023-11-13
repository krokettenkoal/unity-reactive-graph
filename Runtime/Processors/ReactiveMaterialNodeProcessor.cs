using Reactive.Runtime.Nodes;
using UnityEngine;

namespace Reactive.Runtime.Processors
{
    /// <summary>
    /// Processor for reactive material nodes
    /// </summary>
    public class ReactiveMaterialNodeProcessor : ReactiveNodeProcessor
    {
        /// <summary>
        /// The material component to set properties on
        /// </summary>
        private readonly Material _material;
        
        /// <summary>
        /// The property id for the emission color property
        /// </summary>
        private readonly int _emissionColorPropertyId;

        public ReactiveMaterialNodeProcessor(GameObject gameObject) : base(gameObject)
        {
            _material = gameObject.GetComponent<Renderer>().material;
            _emissionColorPropertyId = Shader.PropertyToID(ReactiveMaterialNodeData.EmissionColorPropertyName);
        }
        
        public override float ProcessNode(ReactiveNodeData node, float inputValue)
        {
            if (node is not ReactiveMaterialNodeData nodeData)
                return inputValue;
            
            if (nodeData.property.HasFlag(ReactiveMaterialProperty.Color))
                ProcessColor(nodeData, ref inputValue);

            if (nodeData.property.HasFlag(ReactiveMaterialProperty.Emission))
                ProcessEmission(nodeData, ref inputValue);
            
            ProcessCustomProperties(nodeData, ref inputValue);

            return inputValue;
        }

        /// <summary>
        /// Processes the material's color property, based on the given <paramref name="value"/>
        /// </summary>
        /// <param name="node">The node data to retrieve the color from</param>
        /// <param name="value">The input value of the node</param>
        private void ProcessColor(ReactiveMaterialNodeData node, ref float value)
        {
            var col = node.color.Evaluate(value);
            _material.color = col;
        }
        
        /// <summary>
        /// Processes the material's emission color property, based on the given <paramref name="value"/>
        /// </summary>
        /// <param name="node">The node data to retrieve the color from</param>
        /// <param name="value">The input value of the node</param>
        private void ProcessEmission(ReactiveMaterialNodeData node, ref float value)
        {
            var col = node.color.Evaluate(value);
            _material.SetColor(_emissionColorPropertyId, col);
        }
        
        /// <summary>
        /// Processes the material's custom properties, based on the given <paramref name="value"/>
        /// </summary>
        /// <param name="node">The node data to retrieve the colors from</param>
        /// <param name="value">The input value of the node</param>
        private void ProcessCustomProperties(ReactiveMaterialNodeData node, ref float value)
        {
            for(var i = 0; i < node.customPropertyNames.Count; i++)
            {
                var name = node.customPropertyNames[i];
                var col = node.customPropertyValues[i].Evaluate(value);
                _material.SetColor(name, col);
            }
        }
    }
}