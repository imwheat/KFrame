//****************** 代码文件申明 ************************
//* 文件：NodeEditor                                       
//* 作者：wheat
//* 创建时间：2024/10/16 09:08:22 星期三
//* 功能：节点编辑器
//*****************************************************
using System.Collections.Generic;
using KFrame.Systems.DialogueSystem;
using KFrame.Systems.DialogueSystem.Editor;
using KFrame.Utilities;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace KFrame.Systems.NodeSystem.Editor
{
	public class NodeEditor : EditorWindow
	{
		#region 参数属性

		/// <summary>
		/// 当前正在编辑的树
		/// </summary>
		private NodeTree EditTree
		{
			get => _nodeTreeViewer.Tree;
			set => _nodeTreeViewer.Tree = value;
		}
		/// <summary>
		/// 正在编辑的对话树的Field
		/// </summary>
		private ObjectField _editTreeField;
		/// <summary>
		/// 右侧面板
		/// </summary>
		private VisualElement _rightPanel;
		/// <summary>
		/// 层级父级
		/// </summary>
		private VisualElement _layerPanel;
		/// <summary>
		/// 节点树查看器
		/// </summary>
		private NodeTreeViewer _nodeTreeViewer;
		/// <summary>
		/// 节点属性查看器
		/// </summary>
		private NodeInspectorViewer _nodeInspectorViewer;
		
		public static readonly List<NodeView> AllSelectNodeView = new();

		#endregion

		#region 初始化

		/// <summary>
		/// 打开编辑器
		/// </summary>
		/// <param name="instanceID"></param>
		/// <returns></returns>
		[OnOpenAsset]
		private static bool OpenAsset(int instanceID)
		{
			//获取Object
			Object obj = EditorUtility.InstanceIDToObject(instanceID);
			
			//如果是节点树那就打开编辑器
			if (obj is NodeTree tree)
			{
				ShowWindow(tree);
			}
			
			return false;
		}
		
		[MenuItem("Window/节点树编辑器")]
		private static void ShowWindow()
		{
			NodeEditor editor = GetWindow<NodeEditor>();
			editor.titleContent = new GUIContent("节点树编辑器");
		}
		/// <summary>
		/// 打开一个节点树开始编辑
		/// </summary>
		/// <param name="tree">要编辑的节点树</param>
		private static void ShowWindow(NodeTree tree)
		{
			NodeEditor editor = GetWindow<NodeEditor>();
			editor.titleContent = new GUIContent("节点树编辑器");
			editor._editTreeField.value = tree;
		}
		/// <summary>
		/// 创建GUI
		/// </summary>
		public void CreateGUI()
		{
			VisualElement root = rootVisualElement;
			
			//加载编辑器uxml
			var nodeTree =
				AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
					KFrameAssetsPath.GetPath(GlobalPathType.Frame)+ "Systems/NodeSystem/Editor/NodeEditor.uxml");
			//此处不使用visualTree.Instantiate() 为了保证行为树的单例防止重复实例化，以及需要将此root作为传参实时更新编辑器状态
			nodeTree.CloneTree(root);
			
			//加载编辑器style
			var styleSheet =
				AssetDatabase.LoadAssetAtPath<StyleSheet>(
					KFrameAssetsPath.GetPath(GlobalPathType.Frame)+ "Systems/NodeSystem/Editor/NodeEditor.uss");
			root.styleSheets.Add(styleSheet);

			//获取面板
			_rightPanel = root.Q<VisualElement>("RightPanel");
			_layerPanel = root.Q<VisualElement>("Layers");

			//将节点树视图添加到节点编辑器中
			_nodeTreeViewer = root.Q<NodeTreeViewer>();
			InitTreeViewer();

			//将节属性面板视图添加到节点编辑器中
			_nodeInspectorViewer = root.Q<NodeInspectorViewer>();

			//获取正在编辑树的Field
			_editTreeField = root.Q<ObjectField>("EditNodeTree");
			_editTreeField.objectType = typeof(NodeTree);
			_editTreeField.allowSceneObjects = false;
			_editTreeField.RegisterValueChangedCallback((v) =>
			{
				NodeTree tree = v.newValue as NodeTree;
				if (tree != null)
				{
					if (tree.GetType() == typeof(DialogueTree) && _nodeTreeViewer.GetType() != typeof(DialogueTreeViewer))
					{
						//先删除旧的
						_rightPanel.Remove(_nodeTreeViewer);
						//然后创建新的，然后添加
						_nodeTreeViewer = new DialogueTreeViewer();
						InitTreeViewer();
						_rightPanel.Add(_nodeTreeViewer);
					}
					else if (tree.GetType() == typeof(NodeTree) && _nodeTreeViewer.GetType() != typeof(NodeTreeViewer))
					{
						//先删除旧的
						_rightPanel.Remove(_nodeTreeViewer);
						//然后创建新的，然后添加
						_nodeTreeViewer = new NodeTreeViewer();
						InitTreeViewer();
						_rightPanel.Add(_nodeTreeViewer);
					}
				}
				
				_nodeTreeViewer.OpenTreeView(tree);
				UpdateFieldUI();
			});
			UpdateFieldUI();
		}

		#endregion

		#region 界面更新
		
		/// <summary>
		/// Inspector更新
		/// </summary>
		protected virtual void OnInspectorUpdate()
		{
			_nodeTreeViewer?.UpdateNodeStates();
		}
		/// <summary>
		/// 更新Field的UI
		/// </summary>
		private void UpdateFieldUI()
		{
			_editTreeField.label = EditTree == null ? "请选择一个节点树" : "正在编辑：";
		}
		/// <summary>
		/// 当选择的节点发生了变化时调用
		/// </summary>
		/// <param name="views">选择的节点</param>
		protected virtual void OnNodeSelectionChanged(List<NodeView> views)
		{
			//当节点树视图选择了节点的时候，更新Inspector
			_nodeInspectorViewer.UpdateSelection(views);
		}

		/// <summary>
		/// 初始化TreeViewer
		/// </summary>
		private void InitTreeViewer()
		{
			//设置节点树UI配置
			_nodeTreeViewer.LayerParent = _layerPanel;
			_nodeTreeViewer.AddToClassList("TreeViewer");
			
			//设置节点树节点选择事件
			_nodeTreeViewer.OnNodeSelected = OnNodeSelectionChanged;
		}
		#endregion
	}
}