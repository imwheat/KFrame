//****************** 代码文件申明 ***********************
//* 文件：DialogueTree
//* 作者：wheat
//* 创建时间：2024/10/19 09:30:46 星期六
//* 描述：对话树
//*******************************************************

using KFrame.Systems.NodeSystem;
using UnityEngine;

namespace KFrame.Systems.DialogueSystem
{
    [CreateAssetMenu(menuName = "NodeTree/DialogueTree",fileName = "DialogueTree")]
    public class DialogueTree : NodeTree
    {
        public void InitTree()
        {
            foreach (var node in nodes)
            {
                node.Init();
            }
        }
    }
}

