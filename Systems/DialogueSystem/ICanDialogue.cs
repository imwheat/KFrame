//****************** 代码文件申明 ***********************
//* 文件：ICanDialogue
//* 作者：wheat
//* 创建时间：2024/10/20 15:58:47 星期日
//* 描述：能够对话的接口
//*******************************************************

namespace KFrame.Systems.DialogueSystem
{
    public interface ICanDialogue
    {
        /// <summary>
        /// 对话树
        /// </summary>
        public DialogueTree Tree { get; }
        /// <summary>
        /// 开始对话
        /// </summary>
        public void StartDialogue();
    }
}

