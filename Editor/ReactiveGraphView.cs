using System;
using System.Collections.Generic;
using System.IO;
using Reactive.Editor.Nodes;
using Reactive.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Reactive.Editor
{
    /// <summary>
    /// Graph view for editing reactive graphs
    /// </summary>
    public class ReactiveGraphView : GraphView
    {
        /// <summary>
        /// The path to the asset that this graph is saved to or loaded from
        /// </summary>
        public string AssetPath { get; internal set; }

        /// <summary>
        /// The name of the asset that this graph is saved to or loaded from
        /// </summary>
        /// <seealso cref="AssetPath"/>
        /// <remarks>The asset name does not include any file extensions</remarks>
        public string AssetName => Path.GetFileNameWithoutExtension(AssetPath);
     
        /// <summary>
        /// Constructs a new reactive graph view and sets up the required elements, manipulators and stylesheets
        /// </summary>
        public ReactiveGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("ReactiveGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            
            //  Add manipulation handlers for dragging
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            // Add a grid background
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GenerateEntryPointNode());
        }

        /// <summary>
        /// Gets all ports in the graph that are compatible with the given port
        /// </summary>
        /// <param name="startPort">The port to get compatible (connectable) ports for</param>
        /// <param name="nodeAdapter">The node adapter used for connecting nodes</param>
        /// <returns>A list of ports that are compatible with <paramref name="startPort"/></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if(startPort != port && startPort.node != port.node)
                    compatiblePorts.Add(port);
            });
            
            return compatiblePorts;
        }

        /// <summary>
        /// Generates a port for the specified node
        /// </summary>
        /// <param name="node">The node to generate a port for</param>
        /// <param name="portDirection">The direction (in or out) of the new port</param>
        /// <param name="capacity">The capacity (single or multi) of the new port</param>
        /// <returns>A new port that has been instantiated on the given <paramref name="node"/></returns>
        private static Port GeneratePort(Node node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }

        /// <summary>
        /// Creates the entry point node for the graph
        /// </summary>
        /// <returns>The newly created entry point node</returns>
        private static ReactiveNode GenerateEntryPointNode()
        {
            var node = new ReactiveNode
            {
                title = "START",
                Guid = ReactiveGraphData.EntryNodeGuid,
                IsEntryPoint = true,
                Type = ReactiveNodeType.Entry,
            };
            
            node.AddClassNames();
            node.AddToClassList("node-entry");

            var port = GeneratePort(node, Direction.Output, Port.Capacity.Multi);
            port.portName = "OUT";
            node.outputContainer.Add(port);
            
            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;
            
            node.RefreshExpandedState();
            node.RefreshPorts();
            
            node.SetPosition(new Rect(10, 200, 100, 150));
            return node;
        }

        /// <summary>
        /// Creates a new reactive node of the specified type
        /// </summary>
        /// <param name="nodeType">The type of the reactive node to create</param>
        /// <returns>The newly created reactive node</returns>
        public static ReactiveNode CreateNode(ReactiveNodeType nodeType)
        {
            var node = ReactiveNodeFactory.CreateNodeOfType(Guid.NewGuid().ToString(), ReactiveNodeFactory.GetNodeTitle(nodeType), nodeType);
            node.styleSheets.Add(Resources.Load<StyleSheet>("ReactiveNode"));
            node.AddControls();
            node.AddClassNames();
            
            var inputPort = GeneratePort(node, Direction.Input);
            inputPort.portName = "IN";
            node.inputContainer.Add(inputPort);

            if (node.HasOutput)
            {
                var outputPort = GeneratePort(node, Direction.Output, ReactiveNodeFactory.GetNodeOutputPortCapacity(nodeType));
                outputPort.portName = "OUT";
                node.outputContainer.Add(outputPort);
            }
            
            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(Vector2.zero, node.NodeSize));

            return node;
        }

        /// <summary>
        /// Creates a new node of the specified type and adds it to the graph
        /// </summary>
        /// <param name="nodeType">The type of node to create</param>
        /// <remarks>This is a shorthand for <see cref="CreateNode"/> and <see cref="GraphView.AddElement"/></remarks>
        public void AddNode(ReactiveNodeType nodeType)
        {
            AddElement(CreateNode(nodeType));
        }
    }
}