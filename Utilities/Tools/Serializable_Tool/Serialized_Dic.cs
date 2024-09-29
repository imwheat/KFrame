using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace KFrame.Utilities
{
	/// <summary>
	/// 可序列化的字典，支持二进制和JsonUtility
	/// </summary>
	/// <typeparam name="K">字典Key</typeparam>
	/// <typeparam name="V">字典Value</typeparam>
	[System.Serializable]
	public class Serialized_Dic<K, V> : ISerializationCallbackReceiver, IDictionary<K,V>
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

		#region 操作字典方法

		public bool TryGetValue(K key, out V value)
		{
			return dictionary.TryGetValue(key, out value);
		}

		public V GetValueOrDefault(K key)
		{
			return dictionary.GetValueOrDefault(key);
		}

		public V this[K key]
		{
			get => dictionary[key];
			set => dictionary[key] = value;
		}

		public ICollection<K> Keys { get=>dictionary.Keys; }
		public ICollection<V> Values { get=>dictionary.Values; }

		public bool TryAdd(K key, V value)
		{
			return dictionary.TryAdd(key, value);
		}

		public bool ContainsKey(K key)
		{
			return dictionary.ContainsKey(key);
		}

		public void Add(KeyValuePair<K, V> item)
		{
			dictionary[item.Key] = item.Value;
		}

		public void Clear()
		{
			dictionary.Clear();
		}

		public bool Contains(KeyValuePair<K, V> item)
		{
			return dictionary.ContainsKey(item.Key) && Equals(dictionary[item.Key], item.Value);
		}
		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			K key = array[arrayIndex].Key;
			if (dictionary.ContainsKey(key))
			{
				array[arrayIndex] = new KeyValuePair<K, V>(key, dictionary[key]);
			}
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			return dictionary.Remove(item.Key);
		}

		public int Count { get=>dictionary.Count; }
		public bool IsReadOnly { get => false; }

		public bool ContainsValue(V value)
		{
			return dictionary.ContainsValue(value);
		}

		public void Add(K key, V value)
		{
			dictionary.Add(key, value);
		}
		public bool Remove(K key)
		{
			return dictionary.Remove(key);
		}
		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
		
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
			if (dictionary == null)
			{
				Debug.Log(1);
			}
			for (int i = 0; i < keyList.Count; i++)
			{
				if (keyList[i] == null)
				{
					Debug.Log("2");
				}
				else if (valueList[i] == null)
				{
					Debug.Log("3");
				}
				dictionary.Add(keyList[i], valueList[i]);
			}

			keyList.Clear();
			valueList.Clear();
		}
		
	}
}
