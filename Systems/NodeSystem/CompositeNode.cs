//****************** 代码文件申明 ***********************
//* 文件：CompositeNode
//* 作者：wheat
//* 创建时间：2024/10/18 08:35:37 星期五
//* 描述：CompositeNode还是一个节点，但是其内部还有一棵节点树
//*******************************************************


#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;


namespace KFrame.Systems.NodeSystem
{
    public class CompositeNode : Node, INodeContainer
    {
        public List<Node> nodes = new();
        public Node startNode;
        public Node StartNode
        {
            get => startNode;
            set => startNode = value;
        }
        public List<Node> Nodes => nodes;
        public NodeTree LinkedTree => tree;
        public Object Asset => this;

        public override void Init()
        {
            base.Init();
            
            //初始化包含的节点
            foreach (var node in nodes)
            {
                node.Init();
            }
        }
    }
}

