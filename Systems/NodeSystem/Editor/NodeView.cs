//****************** 代码文件申明 ************************
//* 文件：NodeView                                       
//* 作者：wheat
//* 创建时间：2024/10/16 09:06:54 星期三
//* 功能：节点编辑器内可视化的编辑节点
//*****************************************************

using System;
using System.Collections.Generic;
using KFrame.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace KFrame.Systems.NodeSystem.Editor
{
	public class NodeView : UnityEditor.Experimental.GraphView.Node
	{

		#region 属性参数
		
		/// <summary>
		/// 节点选择事件
		/// </summary>
		public Action<List<NodeView>> OnNodeSelected;
		/// <summary>
		/// 节点名称的输入栏
		/// </summary>
		public TextField NodeNameField;
		/// <summary>
		/// 绑定对象节点
		/// </summary>
		public Node Node;
		/// <summary>
		/// 输入口
		/// </summary>
		public Port Input;
		/// <summary>
		/// 输出口
		/// </summary>
		public Port Output;
		/// <summary>
		/// 节点类型
		/// </summary>
		public Type Type;

		#endregion

		#region 初始化

		public NodeView(Node node) : base( KFrameAssetsPath.GetPath(GlobalPathType.Frame)+ "Systems/NodeSystem/Editor/NodeView.uxml")
		{
			this.Node = node;
			this.title = node.name;

			//获取名称输入栏
			NodeNameField = this.Q<TextField>("NodeName");
			
			//注册名称输入栏事件
			NodeNameField.RegisterValueChangedCallback((value) =>
			{
				//同步更新Node数据并保存
				this.Node.NodeNickName = value.newValue;
				EditorUtility.SetDirty(this.Node);
			});


			this.viewDataKey = node.guid; //guid作为Node类中的viewDataKey关联 进行后续的视图层管理
			style.left = node.position.x;
			style.top = node.position.y;

			//更新UI
			UpdateUI();

			CreateInputPorts();
			CreateOutputPorts();
			SetNodeViewClass();
		}
		/// <summary>
		/// 设置节点样式
		/// </summary>
		private void SetNodeViewClass()
		{
			this.AddNodeToClassList(Node);
		}
		/// <summary>
		/// 创建一个输入接口
		/// </summary>
		private void CreateInputPorts()
		{
			/*
			 * 节点入口设置
			 * 接口链接方向 横向Orientation.Vertical 竖向Orientation.Horizontal
			 * 接口可链接数量 port.Capacity.Single
			 * 接口类型 typeof(bool)
			 */

			// 默认所有节点为多入口类型
			//如果是单向节点 节点端口类型为Single
			Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));

			if (Input != null)
			{
				//将端口名设置为空
				Input.portName = "▲";

				inputContainer.Add(Input);
			}
		}

		/// <summary>
		/// 创建输出端口
		/// </summary>
		private void CreateOutputPorts()
		{
			//如果是单向节点 节点端口类型为Single
			Output = InstantiatePort(Orientation.Vertical, Direction.Output,
				Node.IsSingle ? Port.Capacity.Single : Port.Capacity.Multi, typeof(bool));

			if (Output != null)
			{
				Output.portName = "▼";
				outputContainer.Add(Output);
			}
		}

		
		#endregion

		#region 节点操作

		/// <summary>
		/// 设置节点在节点树视图中的位置
		/// </summary>
		/// <param name="newPos">新的位置</param>
		public override void SetPosition(Rect newPos)
		{
			Undo.RecordObject(Node, "Node (Set Position)");
			//视图中节点位置设置为最新位置newPos
			base.SetPosition(newPos);
			//将最新位置记录到运行时节点树中持久化存储
			Node.position.x = newPos.xMin;
			Node.position.y = newPos.yMin;

			//设置脏数据 方便撤回
			EditorUtility.SetDirty(Node);
		}

		/// <summary>
		/// 重写选中方法
		/// </summary>
		public override void OnSelected()
		{
			base.OnSelected();
			NodeEditor.AllSelectNodeView.Add(this);
			//当选中的时候  treeViewer中的OnNodeSelected被传递触发
			OnNodeSelected?.Invoke(NodeEditor.AllSelectNodeView);
			//("当前节点位置" + GetPosition().position).Log();
		}

		public override void OnUnselected()
		{
			base.OnUnselected();
			NodeEditor.AllSelectNodeView.Remove(this);
			//当选中的时候  treeViewer中的OnNodeSelected被传递触发
			OnNodeSelected?.Invoke(NodeEditor.AllSelectNodeView);
		}


		/// <summary>
		/// 设置节点的状态
		/// </summary>
		public void SetNodeState()
		{
			RemoveFromClassList("running");
			if (Application.isPlaying)
			{
				switch (Node.state)
				{
					case NodeSystem.Node.State.Running:
						AddToClassList("running");
						break;
				}
			}
		}
		/// <summary>
		/// 更新UI
		/// </summary>
		public void UpdateUI()
		{
			//如果昵称不为默认，那就更新显示名称栏的名称
			if (this.Node.NodeNickName != "DefaultName")
			{
				NodeNameField.SetValueWithoutNotify(this.Node.NodeNickName);
			}
		}

		#endregion
		
		
	}
}