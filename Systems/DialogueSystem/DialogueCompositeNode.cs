//****************** 代码文件申明 ***********************
//* 文件：DialogueCompositeNode
//* 作者：wheat
//* 创建时间：2024/10/19 09:36:21 星期六
//* 描述：对话复合节点
//*******************************************************

using System.Collections.Generic;
using KFrame.Systems.NodeSystem;
using KFrame.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KFrame.Systems.DialogueSystem
{
    public class DialogueCompositeNode : DialogueNode, INodeContainer
    {
        #region 属性参数

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

        #endregion

        #region 逻辑

        /// <summary>
        /// 当前节点
        /// </summary>
        private DialogueNode _curNode;

        #endregion

        #region 方法重写

        public override void Init()
        {
            base.Init();
            
            //初始化包含的节点
            foreach (var node in nodes)
            {
                node.Init();
            }
        }

        public override void StartRunning()
        {
            //如果已经在运作了那就返回
            if(state is State.Running) return;
            
            //进入节点
            NextNode();
            
            base.StartRunning();

        }

        public override DialogueNode NextNode()
        {
            //初始节点为等待状态
            if (state is State.Waiting)
            {
                //那就选取复合节点的开始节点
                _curNode = startNode as DialogueNode;
                _curNode?.StartRunning();
                
                return this;
            }
            //当前节点不为空
            else
            {
                //那就继续迭代这个节点
                _curNode = _curNode.NextNode();
                _curNode?.StartRunning();

                //这个包含的节点遍历完了，那就遍历自身下一个节点
                return _curNode == null ? base.NextNode() : this;
            }
        }

        public override string GetText()
        {
            return _curNode.GetText();
        }

        #endregion


    }
}

