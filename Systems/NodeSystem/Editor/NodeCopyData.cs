//****************** 代码文件申明 ***********************
//* 文件：NodeCopyData
//* 作者：wheat
//* 创建时间：2024/10/16 10:19:58 星期三
//* 描述：节点复制信息
//*******************************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KFrame.Systems.NodeSystem.Editor
{
    [System.Serializable]
    public class NodeCopyData : ISerializationCallbackReceiver
    {
        [System.Serializable]
        public class NodeCopyDataBase
        {
            /// <summary>
            /// 绑定节点
            /// </summary>
            [NonSerialized]
            public Node Node;
            /// <summary>
            /// 节点GUID
            /// </summary>
            [NonSerialized]
            public string Guid;
            /// <summary>
            /// 节点昵称
            /// </summary>
            public string nickname;
            /// <summary>
            /// 节点的子集
            /// </summary>
            public List<int> children = new();
            /// <summary>
            /// 节点的UI位置
            /// </summary>
            public Vector2 position;
            /// <summary>
            /// 节点的类型
            /// </summary>
            [NonSerialized]
            public Type NodeType;
            /// <summary>
            /// 节点名称用于序列化保存节点类型
            /// </summary>
            public string typeName;
            public NodeCopyDataBase(Node node)
            {
                Node = node;
                Guid = node.guid;
                nickname = node.NodeNickName;
                position = node.position;
                NodeType = node.GetType();
                typeName = NodeType.AssemblyQualifiedName;
            }
            /// <summary>
            /// 粘贴数据
            /// </summary>
            /// <param name="node">拷贝数据的节点</param>
            public void PasteData(Node node)
            {
                node.NodeNickName = nickname;
            }
        }
        public List<NodeCopyDataBase> copyData = new();
        /// <summary>
        /// 添加节点的拷贝数据
        /// </summary>
        /// <param name="node"></param>
        public void AddCopyData(Node node)
        {
            //如果为空那就返回
            if(node == null) return;

            copyData.Add(new NodeCopyDataBase(node));
        }

        public void OnBeforeSerialize()
        {
            //记录当前每个Node在哪个下标
            Dictionary<string, int> nodeDic = new();
            for (int i = 0; i < copyData.Count; i++)
            {
                nodeDic[copyData[i].Guid] = i;
            }
            
            //遍历每个节点，把它们的child的下标存入
            foreach (var dataBase in copyData)
            {
                dataBase.children = new();
                foreach (var child in dataBase.Node.transitions)
                {
                    dataBase.children.Add(nodeDic[child.node.guid]);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            //获取每个Node的Type类型
            foreach (var dataBase in copyData)
            {
                dataBase.NodeType = Type.GetType(dataBase.typeName);
            }
        }
    }
}

