//****************** 代码文件申明 ***********************
//* 文件：DialogueTreeViewer
//* 作者：wheat
//* 创建时间：2024/10/19 10:04:55 星期六
//* 描述：
//*******************************************************

using KFrame.Systems.NodeSystem;
using KFrame.Systems.NodeSystem.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace KFrame.Systems.DialogueSystem.Editor
{
    public class DialogueTreeViewer : NodeTreeViewer
    {
        #region 操作菜单

        /// <summary>
        /// 创建节点的MenuItem
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="viewPos">创建位置</param>
        protected override void CreateNodeMenuItem(ContextualMenuPopulateEvent evt, Vector2 viewPos)
        {
            evt.menu.AppendAction("新建节点", (a) => { CreateNodeView<DialogueNode>(viewPos); });
            evt.menu.AppendAction("新建复合节点", (a) => { CreateNodeView<DialogueCompositeNode>(viewPos); });
        }

        #endregion
    }
}

