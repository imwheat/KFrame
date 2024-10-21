//****************** 代码文件申明 ***********************
//* 文件：NodeTree
//* 作者：wheat
//* 创建时间：2024/10/16 09:05:36 星期三
//* 描述：节点树
//*******************************************************

using System.Collections.Generic;
using UnityEngine;

namespace KFrame.Systems.NodeSystem
{
    [CreateAssetMenu(menuName = "NodeTree/Tree",fileName = "Tree")]
    public class NodeTree : ConfigBase, INodeContainer
    {
        public List<Node> nodes = new();
        public Node startNode;

        public Node StartNode
        {
            get => startNode;
            set => startNode = value;
        }
        public List<Node> Nodes => nodes;
        public NodeTree LinkedTree => this;
        public Object Asset => this;
    }
}

