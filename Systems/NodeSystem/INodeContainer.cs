//****************** 代码文件申明 ***********************
//* 文件：INodeContainer
//* 作者：wheat
//* 创建时间：2024/10/18 18:07:23 星期五
//* 描述：节点的容器
//*******************************************************

using System.Collections.Generic;
using UnityEngine;

namespace KFrame.Systems.NodeSystem
{
    public interface INodeContainer
    {
        /// <summary>
        /// 开始的节点
        /// </summary>
        public Node StartNode { get; set; }
        /// <summary>
        /// 存储节点的列表
        /// </summary>
        public List<Node> Nodes { get; }
        /// <summary>
        /// 绑定的节点树
        /// </summary>
        public NodeTree LinkedTree { get; }
        /// <summary>
        /// 保存用的Asset
        /// </summary>
        public Object Asset { get; }
    }
}

