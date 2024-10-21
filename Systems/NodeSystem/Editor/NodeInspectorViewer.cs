//****************** 代码文件申明 ************************
//* 文件：NodeInspectorViewer                                       
//* 作者：wheat
//* 创建时间：2024/10/16 09:08:28 星期三
//* 功能：节点信息查看器
//*****************************************************

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace KFrame.Systems.NodeSystem.Editor
{
    public class NodeInspectorViewer : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<NodeInspectorViewer, VisualElement.UxmlTraits> { }

        /// <summary>
        /// 编辑器
        /// </summary>
        private UnityEditor.Editor _editor;

        /// <summary>
        /// ScrollPos
        /// </summary>
        private Vector2 _scrollPosition = Vector2.zero;

        public NodeInspectorViewer()
        {
        }

        /// <summary>
        /// 更新视图选项
        /// </summary>
        /// <param name="nodeViews"></param>
        internal void UpdateSelection(List<NodeView> nodeViews)
        {
            Clear(); //清除之前的信息
            UnityEngine.Object.DestroyImmediate(_editor);

            List<Node> nodes = new();

            foreach (NodeView nodeView in nodeViews)
            {
                if (nodeView != null)
                {
                    nodes.Add(nodeView.Node);
                }
            }

            Node[] nodesArray = nodes.ToArray();
            _editor = UnityEditor.Editor.CreateEditor(nodesArray);
            
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (_editor)
                {
                    //使用滚动视图
                    _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                    _editor.OnInspectorGUI();
                    EditorGUILayout.EndScrollView();
                }
            });

            Add(container);
        }
    }
}