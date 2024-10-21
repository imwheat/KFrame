//****************** 代码文件申明 ***********************
//* 文件：NodeViewerLayer
//* 作者：wheat
//* 创建时间：2024/10/18 13:38:54 星期五
//* 描述：查看节点树的时候的层级
//*******************************************************

using System.Collections.Generic;
using UnityEngine.UIElements;

namespace KFrame.Systems.NodeSystem.Editor
{
    public class NodeViewerLayer
    {
        private readonly NodeTreeViewer _viewer;
        public readonly INodeContainer Container;
        public readonly Button LayerButton;
        public List<NodeView> NodeViews;

        public NodeViewerLayer(NodeTreeViewer viewer ,INodeContainer container)
        {
            _viewer = viewer;
            Container = container;
            NodeViews = new();
            LayerButton = new Button();
            LayerButton.RemoveFromClassList("unity-button");
            LayerButton.AddToClassList("LayerButton");
            LayerButton.clicked += OnClickButton;
        }
        /// <summary>
        /// 按钮点击事件
        /// </summary>
        private void OnClickButton()
        {
            _viewer.OpenLayer(Container);
        }
    }
}

