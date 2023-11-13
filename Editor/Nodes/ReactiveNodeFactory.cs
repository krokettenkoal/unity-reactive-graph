using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Reactive.Editor.Nodes
{
    /// <summary>
    /// Factory class for creating reactive nodes
    /// </summary>
    public static class ReactiveNodeFactory
    {
        /// <summary>
        /// Returns the (node) title for the specified node type
        /// </summary>
        /// <param name="nodeType">The node type to get the title for</param>
        /// <returns>The title to be displayed in the title bar of the node itself</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid/unknown <paramref name="nodeType"/> was provided</exception>
        public static string GetNodeTitle(ReactiveNodeType nodeType)
        {
            return nodeType switch
            {
                ReactiveNodeType.PassThrough => "Pass-through",
                ReactiveNodeType.Material => "Material",
                ReactiveNodeType.TransformPosition => "Position",
                ReactiveNodeType.TransformRotation => "Rotation",
                ReactiveNodeType.TransformScale => "Scale",
                ReactiveNodeType.Animator => "Animator",
                ReactiveNodeType.Curve => "Curve",
                ReactiveNodeType.Math => "Function",
                ReactiveNodeType.Entry => "START",
                _ => throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null)
            };
        }
        
        /// <summary>
        /// Returns the title (path) for the specified node type to be displayed in the 'Add Node' toolbar menu
        /// </summary>
        /// <param name="nodeType">The node type to get the menu title for</param>
        /// <returns>The title (path) to be displayed in the 'Add Node' menu dropdown</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid/unknown <paramref name="nodeType"/> was provided</exception>
        public static string GetNodeMenuTitle(ReactiveNodeType nodeType)
        {
            return nodeType switch
            {
                ReactiveNodeType.PassThrough => "Signal/Pass-through",
                ReactiveNodeType.Material => "Effects/Material",
                ReactiveNodeType.TransformPosition => "Effects/Transform/Position",
                ReactiveNodeType.TransformRotation => "Effects/Transform/Rotation",
                ReactiveNodeType.TransformScale => "Effects/Transform/Scale",
                ReactiveNodeType.Animator => "Effects/Animator",
                ReactiveNodeType.Curve => "Signal/Curve",
                ReactiveNodeType.Math => "Signal/Function",
                ReactiveNodeType.Entry => "START",
                _ => throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null)
            };
        }

        /// <summary>
        /// Returns the output port capacity for the specified node type
        /// </summary>
        /// <param name="nodeType">The node type to get the output port capacity for</param>
        /// <returns>The port capacity for the specified <paramref name="nodeType"/></returns>
        public static Port.Capacity GetNodeOutputPortCapacity(ReactiveNodeType nodeType)
        {
            return nodeType switch
            {
                ReactiveNodeType.Material => Port.Capacity.Single,
                ReactiveNodeType.TransformPosition => Port.Capacity.Single,
                ReactiveNodeType.TransformRotation => Port.Capacity.Single,
                ReactiveNodeType.TransformScale => Port.Capacity.Single,
                ReactiveNodeType.Animator => Port.Capacity.Single,
                ReactiveNodeType.PassThrough => Port.Capacity.Multi,
                ReactiveNodeType.Entry => Port.Capacity.Multi,
                ReactiveNodeType.Curve => Port.Capacity.Multi,
                ReactiveNodeType.Math => Port.Capacity.Multi,
                _ => Port.Capacity.Single
            };
        }

        /// <summary>
        /// Returns the tooltip text for the specified node control (field)
        /// </summary>
        /// <param name="node">The node to get the tooltip text for</param>
        /// <param name="controlName">The name of the control (field) to get the tooltip text for</param>
        /// <returns>The tooltip text for the specified <paramref name="controlName"/> on the specified <paramref name="node"/></returns>
        public static string GetNodeControlTooltip(ReactiveNode node, string controlName)
        {
            var field = node.GetType().GetField(controlName);
            var attributes = field?.GetCustomAttributes(typeof(TooltipAttribute), true) as TooltipAttribute[];
 
            return attributes?.Length > 0 ? attributes[0].tooltip : null;
        }
        
        /// <summary>
        /// Converts the specified node to a node of the specified type
        /// </summary>
        /// <param name="source">The node to convert</param>
        /// <typeparam name="T">The type to convert the node to</typeparam>
        /// <returns>A new <see cref="ReactiveNode"/> instance with the specified type <typeparamref name="T"/></returns>
        private static T ConvertNode<T>(ReactiveNode source) where T : ReactiveNode
        {
            var target = (T) Activator.CreateInstance(typeof(T));
            target.title = source.title;
            target.Guid = source.Guid;
            target.Type = source.Type;
            return target;
        }
        
        /// <summary>
        /// Creates a new reactive node of the specified type
        /// </summary>
        /// <param name="nodeGuid">The GUID of the new node</param>
        /// <param name="nodeTitle">The title of the new node</param>
        /// <param name="nodeType">The type of the new node</param>
        /// <returns>A new reactive node corresponding to the specified <paramref name="nodeType"/></returns>
        public static ReactiveNode CreateNodeOfType(string nodeGuid, string nodeTitle, ReactiveNodeType nodeType)
        {
            var node = new ReactiveNode
            {
                title = nodeTitle,
                Guid = nodeGuid,
                Type = nodeType
            };
            
            return nodeType switch
            {
                ReactiveNodeType.Material => ConvertNode<ReactiveMaterialNode>(node),
                ReactiveNodeType.TransformPosition => ConvertNode<ReactiveTransformNode>(node),
                ReactiveNodeType.TransformRotation => ConvertNode<ReactiveTransformNode>(node),
                ReactiveNodeType.TransformScale => ConvertNode<ReactiveTransformNode>(node),
                ReactiveNodeType.Animator => ConvertNode<ReactiveAnimatorNode>(node),
                ReactiveNodeType.Curve => ConvertNode<ReactiveCurveNode>(node),
                ReactiveNodeType.Math => ConvertNode<ReactiveMathNode>(node),
                _ => node
            };
        }
    }
}