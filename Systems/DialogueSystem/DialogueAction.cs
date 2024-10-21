//****************** 代码文件申明 ***********************
//* 文件：DialogueAction
//* 作者：wheat
//* 创建时间：2024/10/21 14:13:34 星期一
//* 描述：对话事件
//*******************************************************

using System;
using UnityEngine;

namespace KFrame.Systems.DialogueSystem
{
    [System.Serializable]
    public class DialogueAction
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public DialogueActionType type;
        /// <summary>
        /// 传输数据
        /// </summary>
        public string data;
        /// <summary>
        /// 调用事件
        /// </summary>
        public void Invoke()
        {
            switch (type)
            {
                case DialogueActionType.GiveItem:
                    break;
                case DialogueActionType.DebugLog:
                    Debug.Log(data);
                    break;
            }
        }
    }
}

