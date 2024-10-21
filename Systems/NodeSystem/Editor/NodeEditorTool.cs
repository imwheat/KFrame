//****************** 代码文件申明 ************************
//* 文件：NodeView                                       
//* 作者：wheat
//* 创建时间：2024/10/16 13:37:54 星期三
//* 功能：节点编辑器工具类
//*****************************************************

using System;
using System.IO;
using KFrame.Editor;
using KFrame.Utilities;
using UnityEditor;
using UnityEngine;

namespace KFrame.Systems.NodeSystem.Editor
{
    public static class NodeEditorTool
    {
        public static void AddNodeToClassList(this NodeView view, Node node)
        {
            switch (node)
            {
                default:
                    view.AddToClassList("node");
                    break;
            }
        }
        
        #region 节点创建与删除

        /// <summary>
        /// 在节点树里创建新的节点
        /// </summary>
        /// <param name="nodeContainer">节点容器</param>
        /// <param name="type">Node的类型</param>
        public static Node CreateNode(this INodeContainer nodeContainer, Type type)
        {
            //创建节点
            Node node = ScriptableObject.CreateInstance(type) as Node;
            if (node == null)
            {
                KFrame.Editor.KEditorUtility.ErrorDialog($"创建节点的时候发生了错误，尝试创建的Type类型为{type.FullName}");
                return null;
            }
            node.name = type.GetNiceName();

            //记录Undo
            string undoKey = "Node Tree(Create Node)";
            Undo.RecordObject(nodeContainer.Asset, undoKey);
            
            //设置节点数据
            node.tree = nodeContainer.LinkedTree;
            node.guid = GUID.Generate().ToString();
            nodeContainer.Nodes.Add(node);
            
            //设置默认起始和结束节点
            if (nodeContainer.StartNode == null)
            {
                nodeContainer.StartNode = node;
            }
            
            //保存数据
            AssetDatabase.AddObjectToAsset(node, nodeContainer.LinkedTree);
            Undo.RegisterCreatedObjectUndo(node, undoKey);
            
            EditorUtility.SetDirty(nodeContainer.Asset);
            nodeContainer.LinkedTree.SaveAsset();
            
            //返回新建的节点
            return node;
        }
        /// <summary>
        /// 在节点树里创建新的节点
        /// </summary>
        /// <param name="tree">创建节点的树</param>
        public static Node CreateNode<T>(this NodeTree tree) where T : Node
        {
            return CreateNode(tree, typeof(T));
        }
        /// <summary>
        /// 从节点树里删除节点
        /// </summary>
        /// <param name="nodeContainer">节点容器</param>
        /// <param name="nodeToDelete">要删除的节点</param>
        public static void DeleteNode(this INodeContainer nodeContainer, Node nodeToDelete)
        {
            //记录Undo
            string undoKey = "Node Tree(Delete Node)";
            Undo.RecordObject(nodeContainer.Asset, undoKey);

            //删除数据
            nodeContainer.Nodes.Remove(nodeToDelete);
            //如果要删除的节点是开始节点，那就设置新的起始节点
            if (nodeContainer.StartNode == nodeToDelete)
            {
                //找到另一个节点设为开始节点
                foreach (var node in nodeContainer.Nodes)
                {
                    if (node != nodeToDelete)
                    {
                        nodeContainer.StartNode = node;
                    }
                }
            }
            
            //保存数据
            Undo.DestroyObjectImmediate(nodeToDelete);
            nodeContainer.Asset.SaveAsset();
        }

        /// <summary>
        /// 向Node添加子节点
        /// </summary>
        /// <param name="node">父节点</param>
        /// <param name="child">子节点</param>
        public static void AddChildNode(this Node node, Node child)
        {
            if(child == null) return;
            
            //记录undo
            Undo.RecordObject(node, "Node (Add Child)");
            
            //添加Child然后保存
            node.transitions.Add(new NodeTransition(child));
            node.SaveAsset();
        }
        /// <summary>
        /// 从Node删除子节点
        /// </summary>
        /// <param name="node">父节点</param>
        /// <param name="child">子节点</param>
        public static void RemoveChildNode(this Node node, Node child)
        {
            if(child == null) return;
            
            //记录undo
            Undo.RecordObject(node, "Node (Remove Child)");
            
            //添加Child然后保存
            for (int i = node.transitions.Count -1; i >= 0; i--)
            {
                if (node.transitions[i].node == child)
                {
                    node.transitions.RemoveAt(i);
                }
            }
            
            node.SaveAsset();
        }
        
        #endregion
    }
}

