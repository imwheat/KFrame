//****************** 代码文件申明 ***********************
//* 文件：DialogueNode
//* 作者：wheat
//* 创建时间：2024/10/19 09:32:27 星期六
//* 描述：对话节点
//*******************************************************

using System;
using System.Collections.Generic;
using KFrame.Systems.NodeSystem;
using KFrame.UI;

namespace KFrame.Systems.DialogueSystem
{
    public class DialogueNode : Node
    {
        /// <summary>
        /// 文本key
        /// </summary>
        [LocalizedTextKey]
        public string textKey;
        /// <summary>
        /// 进入节点的时候触发的事件
        /// </summary>
        [DialogueActionEdit]
        public List<DialogueAction> actionOnEnter;
        /// <summary>
        /// 退出节点的时候触发的事件
        /// </summary>
        [DialogueActionEdit]
        public List<DialogueAction> actionOnExit;
        
        /// <summary>
        /// 开始运作
        /// </summary>
        public virtual void StartRunning()
        {
            //如果已经在运作了那就返回
            if(state is State.Running) return;
            
            state = State.Running;
            //遍历执行事件
            foreach (var action in actionOnEnter)
            {
                action.Invoke();
            }
        }
        /// <summary>
        /// 获取下一个节点
        /// </summary>
        /// <returns>如果没有就返回null</returns>
        public virtual DialogueNode NextNode()
        {
            //遍历执行事件
            foreach (var action in actionOnExit)
            {
                action.Invoke();
            }
            
            //切换到waiting状态
            state = State.Waiting;
            
            //遍历每个切换，如果有一个满足条件那就返回
            foreach (var nodeTransition in transitions)
            {
                if (nodeTransition.CanTranslate())
                {
                    return nodeTransition.node as DialogueNode;
                }
            }

            return null;
        }
        /// <summary>
        /// 获取文本
        /// </summary>
        /// <returns></returns>
        public virtual string GetText()
        {
            return LocalizationDic.GetLocalizedText(textKey);
        }
    }
}

