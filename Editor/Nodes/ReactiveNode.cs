using Reactive.Runtime.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Reactive.Editor.Nodes
{
    /// <summary>
    /// Base class for all reactive nodes
    /// </summary>
    /// <seealso cref="ReactiveCurveNode"/>
    /// <seealso cref="ReactiveMaterialNode"/>
    /// <seealso cref="ReactiveMathNode"/>
    /// <seealso cref="ReactiveTransformNode"/>
    /// <seealso cref="ReactiveAnimatorNode"/>
    public class ReactiveNode : Node
    {
        [Tooltip("The unique identifier of this node")]
        public string Guid;
        [Tooltip("The type of this node")]
        public ReactiveNodeType Type = ReactiveNodeType.Entry;
        [Tooltip("Whether this node is an entry point to the graph")]
        public bool IsEntryPoint = false;
        
        /// <summary>
        /// The (default) size of the node
        /// </summary>
        public virtual Vector2 NodeSize { get; } = new(200, 200);
        
        /// <summary>
        /// Whether this node has an output port
        /// </summary>
        public virtual bool HasOutput { get; } = true;
        
        /// <summary>
        /// Returns the node data for this node
        /// </summary>
        /// <returns>A new data object containing this node's control values</returns>
        public virtual ReactiveNodeData GetNodeData()
        {
            return new ReactiveNodeData
            {
                guid = Guid,
                type = Type,
                graphPosition = GetPosition().position
            };
        }
        
        /// <summary>
        /// Updates all control fields to match the current node data
        /// </summary>
        protected virtual void UpdateFields() { }

        /// <summary>
        /// Applies the specified node data to this node
        /// </summary>
        /// <param name="data">The data to apply</param>
        public virtual void SetNodeData(ReactiveNodeData data)
        {
            Guid = data.guid;
            UpdateFields();
        }

        /// <summary>
        /// Adds all required control fields to the node
        /// </summary>
        public virtual void AddControls() { }

        /// <summary>
        /// Adds all USS class names to the node
        /// </summary>
        public virtual void AddClassNames()
        {
            mainContainer.AddToClassList("main");
        }
    }
}