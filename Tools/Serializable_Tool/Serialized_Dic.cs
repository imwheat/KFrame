using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace KFrame.Tools
{
	/// <summary>
	/// 可序列化的字典，支持二进制和JsonUtility
	/// </summary>
	/// <typeparam name="K">字典Key</typeparam>
	/// <typeparam name="V">字典Value</typeparam>
	[System.Serializable]
	public class Serialized_Dic<K, V> : ISerializationCallbackReceiver
	{
		[SerializeField] private List<K> keyList;
		[SerializeField] private List<V> valueList;

		[NonSerialized] // 不序列化 避免报错
		private Dictionary<K, V> dictionary;

		public Dictionary<K, V> Dictionary
		{
			get => dictionary;
		}
		public Serialized_Dic()
		{
			dictionary = new Dictionary<K, V>();
		}

		public Serialized_Dic(Dictionary<K, V> dictionary)
		{
			this.dictionary = dictionary;
		}

		// 序列化的时候把字典里面的内容放进list
		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			OnBeforeSerialize();
		}

		// 反序列化时候自动完成字典的初始化
		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			OnAfterDeserialize();
		}

		/// <summary>
		/// Unity序列化前调用
		/// </summary>
		public void OnBeforeSerialize()
		{
			keyList = new List<K>(dictionary.Keys);
			valueList = new List<V>(dictionary.Values);
		}

		/// <summary>
		/// Unity反序列化后调用
		/// </summary>
		public void OnAfterDeserialize()
		{
			dictionary = new Dictionary<K, V>();
			for (int i = 0; i < keyList.Count; i++)
				dictionary.Add(keyList[i], valueList[i]);

			keyList.Clear();
			valueList.Clear();
		}
	}
}
