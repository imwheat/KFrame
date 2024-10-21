//****************** 代码文件申明 ***********************
//* 文件：FrameClass
//* 作者：wheat
//* 创建时间：2024/10/18 08:56:05 星期五
//* 描述：
//*******************************************************

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace KFrame.Systems.NodeSystem.Editor
{
    public class NodeTreeDoubleClickManipulator : Manipulator
    {
        #region 初始化

        public NodeTreeDoubleClickManipulator()
        {
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }

        #endregion

        #region 事件

        private void OnMouseDown(MouseDownEvent evt)
        {
            // 检测是否是双击
            if (evt.clickCount == 2)
            {
                //获取点击到的element
                GraphElement clickedElement = evt.target as GraphElement ?? (evt.target as VisualElement)?.GetFirstAncestorOfType<GraphElement>();
                
                //如果是节点
                if (clickedElement is NodeView nodeView)
                {
                    //那就调用处理
                    (target as NodeTreeViewer)?.OnDoubleClickNode(nodeView);
                }
            }
        }

        #endregion
    }
}

