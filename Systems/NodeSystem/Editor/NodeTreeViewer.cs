//****************** 代码文件申明 ************************
//* 文件：NodeTreeViewer                                       
//* 作者：wheat
//* 创建时间：2024/10/16 09:09:22 星期三
//* 功能：节点树查看器
//*****************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KFrame.Editor;
using KFrame.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = KFrame.Systems.NodeSystem.Node;

namespace KFrame.Systems.NodeSystem.Editor
{
    public class NodeTreeViewer : GraphView
    {
        public new class UxmlFactory : UxmlFactory<NodeTreeViewer, GraphView.UxmlTraits> { }
        
        #region 属性参数
        
        /// <summary>
        /// 节点选中时候的Action
        /// </summary>
        public Action<List<NodeView>> OnNodeSelected;
        /// <summary>
        /// 层级字典
        /// </summary>
        private readonly Dictionary<INodeContainer, NodeViewerLayer> _layerDic = new();
        /// <summary>
        /// 层级的父级
        /// </summary>
        public VisualElement LayerParent;
        /// <summary>
        /// 层级列表
        /// </summary>
        private List<NodeViewerLayer> Layers;
        /// <summary>
        /// 当前所在的层级
        /// </summary>
        private NodeViewerLayer CurLayer;
        /// <summary>
        /// 当前查看的节点树
        /// </summary>
        public NodeTree Tree;
        /// <summary>
        /// 当前的节点容器
        /// </summary>
        private INodeContainer NodeContainer => CurLayer.Container;
        /// <summary>
        /// 是否正在复制
        /// </summary>
        private bool _isCopying;
        /// <summary>
        /// 视图元素发生了变化
        /// </summary>
        private bool _graphElementChanged;
        /// <summary>
        /// 鼠标在视图里的位置
        /// </summary>
        private Vector2 _windowGraphPos = Vector2.zero;

        #endregion

        #region 初始化
        
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public NodeTreeViewer()
        {
            //添加网格图背景
            Insert(0, new GridBackground());
            //添加 Manipulator(操控器) 视图缩放
            this.AddManipulator(new ContentZoomer());
            //添加双击点击
            this.AddManipulator(new NodeTreeDoubleClickManipulator());
            //添加视图拖拽
            this.AddManipulator(new ContentDragger());
            //添加选中对象拖拽
            this.AddManipulator(new SelectionDragger());
            //添加框选
            this.AddManipulator(new RectangleSelector());

            //加载样式表
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    KFrameAssetsPath.GetPath(GlobalPathType.Frame)+ "Systems/NodeSystem/Editor/NodeTreeViewer.uss");
            styleSheets.Add(styleSheet);
            //注册UndoRedo事件
            Undo.undoRedoPerformed += OnUndoRedo;

            //打开树的节点视图
            OpenTreeView(Tree);

            //添加鼠标事件监听器
            this.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }
        /// <summary>
        /// 打开层级
        /// </summary>
        /// <param name="container">要打开层级的container</param>
        public void OpenLayer(INodeContainer container)
        {
            //如果有先前的Layer的话那就设置Layer的按钮可以点击
            CurLayer?.LayerButton.SetEnabled(true);
            
            //如果为空那就直接新建一个空的Layer
            if (container is null || container.Equals(null))
            {
                CurLayer = new NodeViewerLayer(this,null);
            }
            else
            {
                //寻找当前的Layers是否有目标选项
                int find = -1;
                for (int i = 0; i < Layers.Count; i++)
                {
                    //找到了
                    if (Layers[i].Container == container)
                    {
                        //那就设置当前layer为对应layer
                        find = i;
                        CurLayer = Layers[i];
                        break;
                    }
                }

                //如果找到了
                if (find != -1)
                {
                    //把后面的layer都移除
                    for (int i = Layers.Count - 1; i > find; i--)
                    {
                        //移除按钮选项
                        LayerParent.contentContainer.Remove(Layers[i].LayerButton);
                        Layers.RemoveAt(i);
                    }
                }
                //如果没找到
                else
                {
                    //尝试从字典里面获取缓存的layer，没有就创建
                    if (!_layerDic.TryGetValue(container, out CurLayer))
                    {
                        CurLayer = new NodeViewerLayer(this, container);
                        _layerDic[container] = CurLayer;
                    }
                
                    //加入队列
                    Layers.Add(CurLayer);
                    //放置按钮元素到UI父级上面
                    LayerParent.contentContainer.Add(CurLayer.LayerButton);
                }

            }
            
            //当前层级的按钮设置为不能点击
            if (CurLayer != null)
            {
                CurLayer.LayerButton.SetEnabled(false);
                CurLayer.LayerButton.text = Layers.Count == 1 ? "节点树 >" : "层级" + Layers.Count + " >";
            }
            
            //刷新视图
            RefreshView();
        }
        /// <summary>
        /// 刷新视图
        /// </summary>
        private void RefreshView()
        {
            //清除视图发生变化标记
            _graphElementChanged = false;
            
            //在节点树视图重新绘制之前需要取消视图变更方法OnGraphViewChanged的订阅
            //以防止视图变更记录方法中的信息是上一个节点树的变更信息
            graphViewChanged -= OnGraphViewChanged;
            //清除之前渲染的graphElements图层元素
            DeleteElements(graphElements);
            //在清除节点树视图所有的元素之后重新订阅视图变更方法
            graphViewChanged += OnGraphViewChanged;
            
            //如果没有那就返回
            if(NodeContainer == null) return;
            
            //设置默认起始节点
            if (NodeContainer.StartNode == null && NodeContainer.Nodes.Count > 0)
            {
                NodeContainer.StartNode = NodeContainer.Nodes[0];
            }
            
            //遍历创建节点UI
            NodeContainer.Nodes.ForEach(node =>
            {
                CreateNodeView(node);
            });
            
            //更新节点父子关系
            NodeContainer.Nodes.ForEach(node =>
            {
                //获取每个节点的子节点
                NodeView parentView = FindNodeView(node);
                node.transitions.ForEach(transition =>
                {
                    //然后连接父节点和子节点
                    NodeView childView = FindNodeView(transition.node);
                    Edge edge = parentView.Output.ConnectTo(childView.Input);
                    AddElement(edge);
                });
            });

            //等待元素创建之后调整视窗位置
            EditorCoroutineSystem.StartCoroutine(
                CoroutineExtensions.WaitForSecondsRealtimeCallback(0.1f, LookAllElement));
        }
        /// <summary>
        /// 打开节点树视图
        /// </summary>
        /// <param name="tree">要打开的节点树</param>
        public void OpenTreeView(NodeTree tree)
        {
            //先清空之前的节点树缓存
            _layerDic.Clear();
            Layers = new();
            LayerParent?.Clear();
            
            //绑定树
            this.Tree = tree;
            
            //然后再用当前的树刷新视图
            OpenLayer(tree);
        }

        #endregion

        #region 操作菜单

        /// <summary>
        /// 复制节点
        /// </summary>
        private void CopyNode(List<NodeView> copyNodes)
        {
            //得到所有选中的节点
            NodeCopyData copyData = new();
            foreach (var nodeView in copyNodes)
            {
                copyData.AddCopyData(nodeView.Node);
            }
            
            //然后转为jsonData储存
            string jsonData = JsonUtility.ToJson(copyData);
            KFrame.Editor.KEditorUtility.SetCopyText(jsonData);
            _isCopying = true;
        }
        /// <summary>
        /// 粘贴节点
        /// </summary>
        private void PasteNode(Vector2 pos)
        {
            Undo.RecordObject(Tree, "Node Tree(CopyNode)");
            if (_isCopying == false)
            {
                return;
            }

            string jsonData = KFrame.Editor.KEditorUtility.GetCopyText();
            NodeCopyData data = JsonUtility.FromJson<NodeCopyData>(jsonData);
            
            if(data == null || data.copyData.Count == 0) return;
            //寻找这些节点的参考原点，y取最小值，x取平均值
            float originX = 0;
            float originY = data.copyData[0].position.y;
            foreach (var copyData in data.copyData)
            {
                originY = Mathf.Min(originY, copyData.position.y);
                originX += copyData.position.x;
            }
            //创建参考原点
            Vector2 origin = new Vector2(originX / data.copyData.Count, originY);
            
            //遍历copyData逐个开始创建Node
            List<NodeView> nodeViews = new();
            foreach (var copyData in data.copyData)
            {
                nodeViews.Add(CreateNodeView(pos + copyData.position - origin, copyData.NodeType));
            }
            
            //在所有的Node创建完毕以后再赋值并设置父子关系
            for (var i = 0; i < data.copyData.Count; i++)
            {
                var copyData = data.copyData[i];
                NodeView parentNode = nodeViews[i];
                foreach (var childId in copyData.children)
                {
                    NodeView childNode = nodeViews[childId];
                    AddElement(parentNode.Output.ConnectTo(childNode.Input));
                    parentNode.Node.transitions.Add(new NodeTransition(childNode.Node));
                }
                
                //粘贴数据，然后更新UI
                copyData.PasteData(parentNode.Node);
                parentNode.UpdateUI();
                
                //保存
                EditorUtility.SetDirty(parentNode.Node);
            }
            
        }
        /// <summary>
        /// 自动矫正视野
        /// </summary>
        private void LookAllElement()
        {
            //在所有元素周围创建一个新的矩形，并给元素一些填充
            Rect boundingBox = CalculateRectToFitAll(contentViewContainer);

            CalculateFrameTransform(boundingBox, layout, 50, out var frameTranslation, out var frameScaling);
            Matrix4x4.TRS(frameTranslation, Quaternion.identity, frameScaling);
            UpdateViewTransform(frameTranslation, frameScaling);
        }
        /// <summary>
        /// 自动对齐排列
        /// </summary>
        private void AutoAlign()
        {
            List<NodeView> needPos = new();
            //得到所有选中的节点
            foreach (ISelectable selectable in selection)
            {
                if (selectable is NodeView nodeView)
                {
                    needPos.Add(nodeView);
                }
            }
            
            //如果没有那就返回
            if(needPos.Count == 0) return;
            
            var first = needPos[0];

            if (first != null)
            {
                needPos.Sort((x, y) =>
                {
                    if (x.GetPosition().y < y.GetPosition().y)
                    {
                        return -1;
                    }
                    else if (x.GetPosition().y > y.GetPosition().y)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                });


                for (var index = 0; index < needPos.Count; index++)
                {
                    var selectable = needPos[index];
                    NodeView nodeview = selectable;
                    nodeview.SetPosition(new Rect(first.GetPosition().x,
                        first.GetPosition().y + ((nodeview.GetPosition().height + 10f) * index), 0, 0));
                }
            }
        }
        /// <summary>
        /// 创建节点的MenuItem
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="viewPos">创建位置</param>
        protected virtual void CreateNodeMenuItem(ContextualMenuPopulateEvent evt, Vector2 viewPos)
        {
            evt.menu.AppendAction("新建节点", (a) => { CreateNodeView<Node>(viewPos); });
            evt.menu.AppendAction("新建复合节点", (a) => { CreateNodeView<CompositeNode>(viewPos); });
        }
        /// <summary>
        /// 右键菜单
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //如果当前没有正在编辑的节点树那就返回
            if(Tree == null) return;
            
            Vector2 viewPos = ConvertGUIPosToGraph(evt.mousePosition);
            List<NodeView> nodeViews = new List<NodeView>();

            foreach (var selectable in selection)
            {
                switch (selectable)
                {
                    case NodeView nodeView:
                        nodeViews.Add(nodeView);
                        break;
                }
            }
            
            //如果有选择的element
            if (selection.Count > 0)
            {
                evt.menu.AppendAction("删除", (a) => { DeleteAction(); });
                if (nodeViews.Count > 0)
                {
                    evt.menu.AppendAction("自动对齐排列", (a) => { AutoAlign(); });
                    evt.menu.AppendAction("复制节点", (a) => { CopyNode(nodeViews); });
                }

                if (nodeViews.Count == 1)
                {
                    evt.menu.AppendAction("设为起始节点", (a) => { SetStartNode(nodeViews[0]); });
                }
            }
            //什么都没有选择
            else
            {
                CreateNodeMenuItem(evt, viewPos);
                evt.menu.AppendAction("粘贴节点", (a) => { PasteNode(viewPos); },
                    _isCopying ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }

            evt.menu.AppendAction("恢复视图位置", (a) => { LookAllElement(); });
        }

        #endregion

        #region 用户操作
        
        /// <summary>
        /// 当双击点击了节点
        /// </summary>
        /// <param name="nodeView"></param>
        public void OnDoubleClickNode(NodeView nodeView)
        {
            //如果是composite节点，那就打开它的树
            if (nodeView.Node is INodeContainer container)
            {
                OpenLayer(container);
            }
        }
        /// <summary>
        /// 设置开始节点
        /// </summary>
        /// <param name="nodeView"></param>
        private void SetStartNode(NodeView nodeView)
        {
            //记录Undo
            Undo.RecordObject(NodeContainer.Asset, "Node Tree(Set StartNode)");
            
            //取消之前的StartNode的class
            if (NodeContainer.StartNode != null)
            {
                NodeView startNodeView = FindNodeView(NodeContainer.StartNode);
                startNodeView.RemoveFromClassList("start");
            }
            
            //设置class并设置值
            nodeView.AddToClassList("start");
            NodeContainer.StartNode = nodeView.Node;
            
            //保存
            NodeContainer.Asset.SaveAsset();
        }
        /// <summary>
        /// 将GUI位置转为视图位置
        /// </summary>
        /// <param name="guiPosition">GUI位置</param>
        /// <returns>视图位置</returns>
        private Vector2 ConvertGUIPosToGraph(Vector2 guiPosition)
        {
            return contentViewContainer.WorldToLocal(guiPosition);
        }
        /// <summary>
        /// 当鼠标点击
        /// </summary>
        /// <param name="evt"></param>
        private void OnMouseDown(MouseDownEvent evt)
        {
            //获取鼠标在GraphView中的位置
            _windowGraphPos = ConvertGUIPosToGraph(evt.mousePosition);
        }
        /// <summary>
        /// 撤回操作
        /// </summary>
        private void OnUndoRedo()
        {
            //刷新视图
            if (_graphElementChanged)
            {
                RefreshView();
            }
            //保存资源
            AssetDatabase.SaveAssets();
        }
        /// <summary>
        /// 创建节点
        /// </summary>
        private NodeView CreateNodeView(Vector2 pos, Type type)
        {
            //在节点树里场景节点，然后创建UI
            var nodeView = CreateNodeView(NodeContainer.CreateNode(type));
            nodeView.SetPosition(new Rect(pos, Vector2.zero));
            
            return nodeView;
        }
        /// <summary>
        /// 创建节点
        /// </summary>
        protected NodeView CreateNodeView<T>(Vector2 pos) where T : Node
        {
            return CreateNodeView(pos, typeof(T));
        }

        /// <summary>
        /// 创建节点UI
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>节点UI</returns>
        protected virtual NodeView CreateNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node)
            {
                OnNodeSelected = OnNodeSelected,
                Type = node.GetType(),
            };
            nodeView.AddToClassList(nodeView.Type.Name.Contains("Composite")?"composite":"normal");
            if (nodeView.Node == NodeContainer.StartNode)
            {
                nodeView.AddToClassList("start");
            }

            //将对应节点UI添加到节点树视图上
            AddElement(nodeView);
            return nodeView;
        }
        /// <summary>
        /// 删除操作
        /// </summary>
        protected virtual void DeleteAction()
        {
            //记录视图变化事件
            GraphViewChange viewChanged = new GraphViewChange
            {
                elementsToRemove = new List<GraphElement>()
            };

            //存储要删除的element
            List<GraphElement> elementToRemove = new List<GraphElement>();
            HashSet<Edge> edgeToRemove = new HashSet<Edge>();
            
            //遍历所选项
            foreach (var selectable in selection)
            {
                //只要是GraphElement那就删除
                if (selectable is GraphElement element and not Edge)
                {
                    viewChanged.elementsToRemove.Add(element);
                    elementToRemove.Add(element);
                }
                
                switch (selectable)
                {
                    case NodeView nodeView:
                        //如果是节点UI要把它的连线也删除
                        foreach (var inputEdge in nodeView.Input.connections)
                        {
                            edgeToRemove.Add(inputEdge);
                        }
                        foreach (var outputEdge in nodeView.Output.connections)
                        {
                            edgeToRemove.Add(outputEdge);
                        }
                        
                        break;
                    case Edge edge:
                        edgeToRemove.Add(edge);
                        break;
                }
            }

            //遍历处理要删除的线
            foreach (var edge in edgeToRemove)
            {
                //然后添加进要删除的列表
                viewChanged.elementsToRemove.Add(edge);
                elementToRemove.Add(edge);
            }
            //颠倒一下把edge放在前面先处理
            viewChanged.elementsToRemove.Reverse();
            
            //调用视图更新事件
            graphViewChanged?.Invoke(viewChanged);
            
            //断开接口连接
            foreach (var edge in edgeToRemove)
            {
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
            }
            
            //然后逐个删除
            elementToRemove.ForEach(RemoveElement);
        }

        #endregion

        #region 视图界面操作

        /// <summary>
        /// 查找节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }
        /// <summary>
        /// 当节点树视图发生变化的时候调用
        /// 对项目资源元素变化进行操作处理
        /// </summary>
        /// <param name="graphViewChange">视图变化事件</param>
        /// <returns></returns>
        protected virtual GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            //标记视图发生了变化
            _graphElementChanged = true;
            
            //如果有元素删除了，那就遍历进行操作
            graphViewChange.elementsToRemove?.ForEach(graphElement =>
            {
                switch (graphElement)
                {
                    //如果删除的是节点UI
                    case NodeView nodeView:
                        //那就要删除节点
                        NodeContainer.DeleteNode(nodeView.Node);
                        break;
                    //如果删除的是连接线
                    case Edge edge:
                        //那就要取消线连接的两节点的父子关系
                        if (edge.output.node is NodeView parentView && edge.input.node is NodeView childView)
                        {
                            parentView.Node.RemoveChildNode(childView.Node);
                        }
                        break;
                }
            });

            //如果创建线条了，那就遍历进行操作
            graphViewChange.edgesToCreate?.ForEach(edge =>
            {
                //设置节点父子关系
                if (edge.output.node is NodeView parentView && edge.input.node is NodeView childView)
                {
                    parentView.Node.AddChildNode(childView.Node);
                }
            });

            //重新配置开始节点的class
            if (NodeContainer.StartNode != null)
            {
                NodeView view = FindNodeView(NodeContainer.StartNode);
                view.RemoveFromClassList("start");
                view.AddToClassList("start");
            }
            
            //返回操作
            return graphViewChange;
        }


        /// <summary>
        /// 适配端口 那些可以进行链接
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            
            //遍历所有端口，过滤一部分接口
            List<Port> result = new List<Port>();
            foreach (var port in ports)
            {
                //方向相同和自身排除
                if (port.direction == startPort.direction) continue;
                if (port.node == startPort.node) continue;
                result.Add(port);
            }

            return result;
        }

        /// <summary>
        /// 更新节点状态
        /// </summary>
        public void UpdateNodeStates()
        {
            nodes.ForEach(node =>
            {
                NodeView view = node as NodeView;
                view?.SetNodeState();
            });
        }
        /// <summary>
        /// 设置视图位置到鼠标位置
        /// </summary>
        /// <param name="nodeView"></param>
        protected void SetNodeViewPosToMousePos(NodeView nodeView)
        {
            nodeView.SetPosition(new Rect(_windowGraphPos, new Vector2()));
        }

        #endregion
        
    }
}