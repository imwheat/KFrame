//****************** 代码文件申明 ***********************
//* 文件：DialogueSystem
//* 作者：wheat
//* 创建时间：2024/10/20 16:47:50 星期日
//* 描述：对话系统
//*******************************************************

using KFrame.UI;
using KFrame.Utilities;
using UnityEngine;

namespace KFrame.Systems.DialogueSystem
{
    public static class DialogueSystem
    {
        /// <summary>
        /// 对话中
        /// </summary>
        private static bool inDialogue;
        /// <summary>
        /// 对话中
        /// </summary>
        public static bool InDialogue => inDialogue;
        /// <summary>
        /// 对话CD
        /// 防止玩家结束对话的时候又按到NPC
        /// </summary>
        private const float DialogueCD = 0.5f;
        /// <summary>
        /// 是否处于对话CD
        /// 防止玩家结束对话的时候又按到NPC
        /// </summary>
        private static bool inDialogueCD;
        /// <summary>
        /// UI对话面板
        /// </summary>
        private static DialogueUIPanel dialogueUIPanel;
        /// <summary>
        /// UI对话面板
        /// </summary>
        public static DialogueUIPanel DialoguePanel => dialogueUIPanel;   
        
        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            
        }

        #endregion

        #region 对话控制
        
        /// <summary>
        /// 开始对话
        /// </summary>
        /// <param name="tree">对话树</param>
        public static void StartDialogue(DialogueTree tree)
        {
            if (tree == null)
            {
                Debug.LogError("开始对话的时候发生错误！使用了空的对话树！");
                return;
            }

            //在CD中返回
            if (inDialogueCD)
            {
                Debug.Log("在对话CD中");
                return;
            }
            
            //标记在对话中
            inDialogue = true;
            
            //禁用玩家输入，并启用UI输入
            //PlayerCenter.DisablePlayerInput(true);
            GameTimeTool.WaitTime(0.2f, KInputSystem.EnableUInput);
            
            //显示对话UI，并进行初始化
            dialogueUIPanel = UISystem.Show<DialogueUIPanel>();
            dialogueUIPanel.Init(tree);
            
        }
        /// <summary>
        /// 结束对话
        /// </summary>
        public static void EndDialogue()
        {
            //标志结束对话
            inDialogue = false;
            //关闭对话面板
            dialogueUIPanel.Close();
            
            //关闭UI控制恢复玩家控制
            KInputSystem.DisableUIInput();
            //GameTimeTool.WaitTime(0.2f, PlayerCenter.EnablePlayerInput);
            
            //设置对话CD
            inDialogueCD = true;
            GameTimeTool.WaitTime(DialogueCD, DialogueCDEnd);
        }
        /// <summary>
        /// 对话CD结束
        /// </summary>
        private static void DialogueCDEnd()
        {
            inDialogueCD = false;
        }

        #endregion
        
    }
}

