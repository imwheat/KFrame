//****************** 代码文件申明 ***********************
//* 文件：Node
//* 作者：wheat
//* 创建时间：2024/10/16 09:15:28 星期三
//* 描述：节点的基类
//*******************************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KFrame.Systems.NodeSystem
{
    public class Node : ConfigBase
    {
        #region 属性参数

        /// <summary>
        /// 节点状态
        /// </summary>
        public enum State
        {
            /// <summary>
            /// 等待状态
            /// </summary>
            Waiting = 0,
            /// <summary>
            /// 运行状态
            /// </summary>
            Running = 1,
        }
        /// <summary>
        /// 所属节点树
        /// </summary>
        public NodeTree tree;
        /// <summary>
        /// 节点GUID
        /// </summary>
        public string guid;
        /// <summary>
        /// 节点昵称
        /// </summary>
        public string NodeNickName = "DefaultName";
        /// <summary>
        /// 节点状态
        /// </summary>
        public State state;
        /// <summary>
        /// 节点坐标位置
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// 是否为单一节点
        /// </summary>
        public virtual bool IsSingle=>false;
        /// <summary>
        /// 节点切换
        /// </summary>
        public List<NodeTransition> transitions = new();

        #endregion

        #region 初始化

        public virtual void Init()
        {
            state = State.Waiting;
        }

        #endregion
    }
}

