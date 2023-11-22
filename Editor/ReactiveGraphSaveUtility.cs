using System.Collections.Generic;
using System.IO;
using System.Linq;
using Reactive.Editor.Nodes;
using Reactive.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Reactive.Editor
{
    /// <summary>
    /// Utility class for saving and loading reactive graphs
    /// </summary>
    public class ReactiveGraphSaveUtility
    {
        /// <summary>
        /// The reactive graph view that this utility is used for
        /// </summary>
        private ReactiveGraphView _graphView;
        
        /// <summary>
        /// The reactive graph data that is currently loaded
        /// </summary>
        private ReactiveGraphData _graphData;
        
        /// <summary>
        /// The edges (connections) in the graph
        /// </summary>
        private IEnumerable<Edge> Edges => _graphView.edges;
        
        /// <summary>
        /// The nodes in the graph
        /// </summary>
        private IEnumerable<ReactiveNode> Nodes => _graphView.nodes.Cast<ReactiveNode>();
        
        /// <summary>
        /// The default name for a new reactive graph asset
        /// </summary>
        private const string DefaultAssetName = "New Reactive Graph";
        
        /// <summary>
        /// Instantiates a new instance of the <see cref="ReactiveGraphSaveUtility"/> class for the specified <paramref name="targetView"/>
        /// </summary>
        /// <param name="targetView">The reactive graph view to instantiate the utility for</param>
        /// <returns>The newly created utility</returns>
        public static ReactiveGraphSaveUtility GetInstance(ReactiveGraphView targetView)
        {
            return new ReactiveGraphSaveUtility
            {
                _graphView = targetView
            };
        }

        /// <summary>
        /// Saves the current graph to a <see cref="ReactiveGraphData"/> asset
        /// </summary>
        /// <param name="saveAs">Whether to save the graph to a new asset (<c>true</c>) or the currently loaded asset (<c>false</c>)</param>
        /// <remarks>
        /// When saving to an existing asset, the old asset is deleted and a new asset is created.
        /// This causes Unity to lose all existing references to the asset and requires the user to manually assign all references again.
        /// This issue will be addressed in a future version.
        /// </remarks>
        public void SaveGraph(bool saveAs = false)
        {
            if(!Edges.Any()) return;
            
            var graphData = ScriptableObject.CreateInstance<ReactiveGraphData>();
            var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
            foreach (var edge in connectedPorts)
            {
                if(edge.output.node is not ReactiveNode outputNode || edge.input.node is not ReactiveNode inputNode) continue;
                
                graphData.nodeLinks.Add(new NodeLinkData
                {
                    baseNodeGuid = outputNode.Guid,
                    targetNodeGuid = inputNode.Guid
                });
            }
            
            foreach (var node in Nodes.Where(node => !node.IsEntryPoint))
            {
                graphData.nodes.Add(node.GetNodeData());
            }

            var assetPath = _graphView.AssetPath;
            if (saveAs || string.IsNullOrEmpty(assetPath))
            {
                var filePath = EditorUtility.SaveFilePanel(
                    "Save Graph",
                    GetAssetDirectory(),
                    DefaultAssetName + ".asset",
                    "asset"
                    );

                if (string.IsNullOrEmpty(filePath))
                    return;

                assetPath = GetAssetPath(filePath);
            }

            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.CreateAsset(graphData, assetPath);
            AssetDatabase.SaveAssets();

            _graphView.AssetPath = assetPath;
        }
        
        /// <summary>
        /// Loads a graph from a <see cref="ReactiveGraphData"/> asset
        /// </summary>
        public void LoadGraph()
        {
            var filePath = EditorUtility.OpenFilePanel(
                "Load Graph", 
                GetAssetDirectory(), 
                "asset"
                );
            
            if (string.IsNullOrEmpty(filePath))
                return;
            
            var assetPath = GetAssetPath(filePath);
            _graphData = AssetDatabase.LoadAssetAtPath<ReactiveGraphData>(assetPath);
            if(_graphData == null)
            {
                EditorUtility.DisplayDialog("Error loading Graph", $"Target graph file {assetPath} could not be opened!", "OK");
                return;
            }
            
            ClearGraph();
            CreateNodes();
            ConnectNodes();

            _graphView.AssetPath = assetPath;
        }

        /// <summary>
        /// Gets the asset (project-relative) path for the specified (absolute) file path
        /// </summary>
        /// <param name="filePath">The file path to get the asset path for</param>
        /// <returns>The (project-relative) asset path of the given <paramref name="filePath"/></returns>
        private static string GetAssetPath(string filePath)
        {
            return "Assets/" + Path.GetRelativePath(Application.dataPath, filePath).TrimStart('.').Replace("\\", "/");
        }

        /// <summary>
        /// Gets the directory (absolute path) of the current graph asset
        /// </summary>
        /// <returns>The full path to the directory of the currently loaded <see cref="_graphData"/>.
        /// If no asset is loaded, the project path is returned.</returns>
        private string GetAssetDirectory()
        {
            return Path.Join(Application.dataPath, Path.GetDirectoryName(_graphView.AssetPath));
        }

        /// <summary>
        /// Connects all nodes in the graph based on the <see cref="ReactiveGraphData.nodeLinks"/> in the <see cref="_graphData"/>
        /// </summary>
        private void ConnectNodes()
        {
            foreach (var node in Nodes)
            {
                var connections = _graphData.nodeLinks.Where(x => x.baseNodeGuid == node.Guid).ToList();
                foreach (var targetNode in connections.Select(connection => Nodes.First(x => x.Guid == connection.targetNodeGuid)))
                {
                    LinkNodes(node, targetNode);
                    
                    targetNode.SetPosition(new Rect(
                        _graphData.nodes.First(x => x.guid == targetNode.Guid).graphPosition,
                        node.NodeSize
                    ));
                }
            }
        }

        /// <summary>
        /// Links (connects) the specified <paramref name="baseNode"/> to the specified <paramref name="targetNode"/>
        /// </summary>
        /// <param name="baseNode">The base node (outgoing) to connect</param>
        /// <param name="targetNode">The target node (incoming) to connect</param>
        private void LinkNodes(Node baseNode, Node targetNode)
        {
            var inputPort = (Port) targetNode.inputContainer[0];
            var outputPort = (Port) baseNode.outputContainer[0];
            LinkPorts(outputPort, inputPort);
        }

        /// <summary>
        /// Links (connects) the specified <paramref name="output"/> port to the specified <paramref name="input"/> port
        /// </summary>
        /// <param name="output">The output port (base) to connect</param>
        /// <param name="input">The input port (target) to connect</param>
        private void LinkPorts(Port output, Port input)
        {
            var edge = new Edge
            {
                output = output,
                input = input
            };
            
            edge.input.Connect(edge);
            edge.output.Connect(edge);
            _graphView.Add(edge);
        }

        /// <summary>
        /// Creates all nodes in the graph based on the <see cref="ReactiveGraphData.nodes"/> in the <see cref="_graphData"/>
        /// </summary>
        private void CreateNodes()
        {
            foreach (var nodeData in _graphData.nodes)
            {
                _graphView.AddNode(nodeData);
            }
        }

        /// <summary>
        /// Clears the graph by removing all nodes and edges, and resets the <see cref="_graphView"/>'s <see cref="ReactiveGraphView.AssetPath"/>
        /// </summary>
        internal void ClearGraph()
        {
            foreach (var node in Nodes.Where(node => !node.IsEntryPoint))
            {
                foreach (var edge in Edges.Where(edge => edge.input.node == node))
                {
                    _graphView.RemoveElement(edge);
                }
                
                _graphView.RemoveElement(node);
            }

            _graphView.AssetPath = null;
        }
    }
}