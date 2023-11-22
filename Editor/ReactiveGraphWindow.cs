using System;
using System.Linq;
using Reactive.Editor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Reactive.Editor
{
    /// <summary>
    /// Reactive graph editor window
    /// </summary>
    public class ReactiveGraphWindow : EditorWindow
    {
        [SerializeField, Tooltip("The style sheet to use for the graph view")]
        private StyleSheet graphStyleSheet;
        [SerializeField, Tooltip("The style sheet to use for the nodes in the graph view")]
        private StyleSheet nodeStyleSheet;
        
        /// <summary>
        /// The graph view to display/edit
        /// </summary>
        private ReactiveGraphView _graphView;
        
        /// <summary>
        /// The mini map to display on top of the graph view
        /// </summary>
        private MiniMap _miniMap;

        /// <summary>
        /// The (default) tab/window title
        /// </summary>
        private const string GraphTitle = "Reactive Graph";
        
        [MenuItem("Window/Reactive Graph")]
        public static void OpenAudioReactiveGraphWindow()
        {
            var window = GetWindow<ReactiveGraphWindow>();
            window.titleContent = new GUIContent(GraphTitle);
        }
        
        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            GenerateMiniMap();
            UpdateWindowTitle();
        }

        private void OnDisable()
        {
            if(_graphView != null)
                rootVisualElement.Remove(_graphView);
        }

        /// <summary>
        /// Creates a new graph view and adds it to the window
        /// </summary>
        private void ConstructGraphView()
        {
            _graphView = new ReactiveGraphView
            {
                name = GraphTitle,
                NodeStyleSheet = nodeStyleSheet,
            };
            
            if(graphStyleSheet)
                _graphView.styleSheets.Add(graphStyleSheet);
            
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        /// <summary>
        /// Creates a toolbar with various options
        /// </summary>
        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();
            
            var fileMenu = new ToolbarMenu {text = "File"};
            fileMenu.menu.AppendAction("New", action => { ClearGraph(); });
            fileMenu.menu.AppendAction("Open...", action => { RequestDataOperation(false); });
            fileMenu.menu.AppendAction("Save", action => { RequestDataOperation(true); });
            fileMenu.menu.AppendAction("Save as...", action => { RequestDataOperation(true, true); });

            var nodeCreateMenu = new ToolbarMenu {text = "Add Node"};
            foreach (var t in Enum.GetValues(typeof(ReactiveNodeType)).Cast<ReactiveNodeType>())
            {
                if(t == ReactiveNodeType.Entry)
                    continue;
                
                nodeCreateMenu.menu.AppendAction(ReactiveNodeFactory.GetNodeMenuTitle(t), action => { _graphView.AddNode(t); });
            }
            
            var miniMapToggle = new Toggle("Mini Map") {value = _miniMap?.visible ?? false};
            miniMapToggle.RegisterValueChangedCallback(evt => _miniMap.visible = evt.newValue);
            
            toolbar.Add(fileMenu);
            toolbar.Add(nodeCreateMenu);
            toolbar.Add(miniMapToggle);
            rootVisualElement.Add(toolbar);
        }

        /// <summary>
        /// Creates a mini map and adds it to the graph view
        /// </summary>
        /// <remarks>By default, the mini map is not visible and needs to be enabled by the user</remarks>
        private void GenerateMiniMap()
        {
            _miniMap = new MiniMap { anchored = true };
            _miniMap.SetPosition(new Rect(10, 30, 200, 140));
            _miniMap.visible = false;
            _graphView.Add(_miniMap);
        }

        /// <summary>
        /// Requests a data operation (save or load) from the user
        /// </summary>
        /// <param name="save">Whether to save (<c>true</c>) the current graph to or load (<c>false</c>) a graph from a <see cref="Reactive.Runtime.ReactiveGraphData"/> asset</param>
        /// <param name="saveAs"></param>
        private void RequestDataOperation(bool save, bool saveAs = false)
        {
            var saveUtility = ReactiveGraphSaveUtility.GetInstance(_graphView);
            if (save)
                saveUtility.SaveGraph(saveAs);
            else
                saveUtility.LoadGraph();
            
            UpdateWindowTitle();
        }

        /// <summary>
        /// Clears the current graph
        /// </summary>
        private void ClearGraph()
        {
            var util = ReactiveGraphSaveUtility.GetInstance(_graphView);
            util.ClearGraph();
        }

        /// <summary>
        /// Updates the window title to reflect the current graph name
        /// </summary>
        private void UpdateWindowTitle()
        {
            titleContent = string.IsNullOrEmpty(_graphView.AssetPath) 
                ? new GUIContent(GraphTitle) 
                : new GUIContent($"{_graphView.AssetName} | {GraphTitle}");
        }
    }
    
}