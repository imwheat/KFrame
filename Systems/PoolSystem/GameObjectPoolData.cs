using System.Collections.Generic;
using KFrame.Utilities;
using UnityEngine;

namespace KFrame.Systems
{
	/// <summary>
	/// 对象池数据
	/// </summary>
	public class GameObjectPoolData
	{
		#region GameObjectPoolData持有的数据及初始化方法

		/// <summary>
		/// 这一层物体的父节点
		/// </summary>
		public Transform RootTransform;

		/// <summary>
		/// 对象容器
		/// </summary>
		public readonly Queue<GameObject> PoolQueue;

		/// <summary>
		/// 容量限制
		/// -1 为无限
		/// </summary>
		public int MaxCapacity = -1;
		public GameObjectPoolData(int capacity = -1)
		{
			if (capacity == -1)
			{
				PoolQueue = new Queue<GameObject>();
			}
			else
			{
				PoolQueue = new Queue<GameObject>(capacity);
			}
		}
		
		/// <summary>
		/// 初始化对象池
		/// </summary>
		/// <param name="keyName">对象池key</param>
		/// <param name="poolRootObj">对象池的父级</param>
		/// <param name="capacity">对象池容量(-1为无上限)</param>
		public void Init(string keyName, Transform poolRootObj, int capacity = -1)
		{
			// 创建父节点 并设置到对象池根节点下方
			GameObject go = PoolSystem.GetGameObject(PoolSystem.PoolLayerGameObjectName, poolRootObj);
			if (!go)
			{
				go = new GameObject(PoolSystem.PoolLayerGameObjectName);
				go.transform.SetParent(poolRootObj);
			}

			RootTransform = go.transform;
			RootTransform.name = keyName;
			MaxCapacity = capacity;
		}

		#endregion

		#region GameObjectPool数据相关操作

		/// <summary>
		/// 将对象放进对象池
		/// </summary>
		public bool PushObj(GameObject obj)
		{
			// 检测是不是超过容量
			if (MaxCapacity != -1 && PoolQueue.Count >= MaxCapacity)
			{
				Object.Destroy(obj);
				return false;
			}

			// 对象进容器
			PoolQueue.Enqueue(obj);

			// 设置父物体
			obj.transform.SetParent(RootTransform);

			// 设置隐藏
			obj.SetActive(false);

			return true;
		}

		/// <summary>
		/// 从对象池中获取对象
		/// </summary>
		public GameObject GetObj(Transform parent = null, bool isActiveStart = true)
		{
			//从队列里面获取obj
			GameObject obj = null;
			lock (PoolQueue)
			{
				obj = PoolQueue.Dequeue();
			}
			//如果没有就返回null
			if (!obj) return null;
			//调整激活状态
			obj.SetActive(isActiveStart);
			//设置父物体
			//如果没有父物体，那就调整到场景默认父级
			obj.transform.SetParent(!parent ? GameObjectPoolModule.DefaultDefaultParentInGameScene.transform : parent);
			//返回obj
			return obj;
		}

		/// <summary>
		/// 销毁层数据
		/// </summary>
		/// <param name="pushThisToPool">将对象池层级挂接点也推送进对象池</param>
		public void Dispose(bool pushThisToPool = false)
		{
			MaxCapacity = -1;
			if (!pushThisToPool)
			{
				// 真实销毁 这里由于删除层级根物体 会导致下方所有对象都被删除，所以不需要单独删除PoolQueue
				Object.Destroy(RootTransform.gameObject);
			}
			else
			{
				// 销毁队列中的全部游戏物体
				foreach (GameObject item in PoolQueue)
				{
					Object.Destroy(item);
				}

				// 扔进对象池
				RootTransform.gameObject.name = PoolSystem.PoolLayerGameObjectName;
				PoolSystem.PushGameObject(RootTransform.gameObject);
				PoolSystem.PushObject(this);
			}

			// 队列清理
			PoolQueue.Clear();
			RootTransform = null;
		}

		#endregion
	}
}