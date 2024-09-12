using System;
using System.Collections;
using UnityEngine;
using KFrame.Systems;

namespace KFrame
{
	public static class FrameExtension
	{
		#region 通用



		#endregion

		#region GameObject
		/// <summary>
		/// 扩展方法，用于检查一个 GameObject 是否为空。
		/// </summary>
		/// <param name="obj">要检查空值的 GameObject。</param>
		/// <returns>如果 GameObject 为空则返回 true，否则返回 false。</returns>
		public static bool IsNull(this GameObject obj)
		{
			// 使用 ReferenceEquals 方法来比较 obj 和 null 是否引用同一对象。
			// 如果 obj 和 null 引用同一对象，说明 obj 是空的，返回 true。
			// 否则，返回 false，表示 obj 不为空。
			return ReferenceEquals(obj, null);
		}

		#endregion

		#region 资源管理

		/// <summary>
		/// GameObject放入对象池
		/// </summary>
		public static void GameObjectPushPool(this GameObject go)
		{
			if (go.IsNull())
			{
				Debug.Log("将空物体放入对象池");
			}
			else
			{
				PoolSystem.PushGameObject(go);
			}
		}

		/// <summary>
		/// GameObject放入对象池
		/// </summary>
		public static void GameObjectPushPool(this Component com)
		{
			GameObjectPushPool(com.gameObject);
		}

		/// <summary>
		/// 普通类放进池子
		/// </summary>
		public static void ObjectPushPool(this object obj)
		{
			PoolSystem.PushObject(obj);
		}

		#endregion

		#region Mono

		/// <summary>
		/// 添加Update监听
		/// </summary>
		public static void AddUpdate(this object obj, Action action)
		{
			MonoSystem.AddUpdateListener(action);
		}

		/// <summary>
		/// 移除Update监听
		/// </summary>
		public static void RemoveUpdate(this object obj, Action action)
		{
			MonoSystem.RemoveUpdateListener(action);
		}

		/// <summary>
		/// 添加LateUpdate监听
		/// </summary>
		public static void AddLateUpdate(this object obj, Action action)
		{
			MonoSystem.AddLateUpdateListener(action);
		}

		/// <summary>
		/// 移除LateUpdate监听
		/// </summary>
		public static void RemoveLateUpdate(this object obj, Action action)
		{
			MonoSystem.RemoveLateUpdateListener(action);
		}

		/// <summary>
		/// 添加FixedUpdate监听
		/// </summary>
		public static void AddFixedUpdate(this object obj, Action action)
		{
			MonoSystem.AddFixedUpdateListener(action);
		}

		/// <summary>
		/// 移除Update监听
		/// </summary>
		public static void RemoveFixedUpdate(this object obj, Action action)
		{
			MonoSystem.RemoveFixedUpdateListener(action);
		}

		public static Coroutine StartCoroutine(this object obj, IEnumerator routine)
		{
			return MonoSystem.Start_Coroutine(obj, routine);
		}

		public static void StopCoroutine(this object obj, Coroutine routine)
		{
			MonoSystem.Stop_Coroutine(obj, routine);
		}

		/// <summary>
		/// 关闭全部协程，注意只会关闭调用对象所属的协程
		/// </summary>
		/// <param name="obj"></param>
		public static void StopAllCoroutine(this object obj)
		{
			MonoSystem.StopAllCoroutine(obj);
		}

		#endregion
	}
}