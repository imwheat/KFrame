//****************** 代码文件申明 ***********************
//* 文件：ObjectPoolModule
//* 作者：wheat
//* 创建时间：2024/09/24 13:23:22 星期二
//* 描述：Object对象池的管理器
//*******************************************************
using System;
using System.Collections.Generic;
using KFrame.Utilities;

namespace KFrame.Systems
{
    public class ObjectPoolModule
    {
        #region ObjectPoolModule持有的数据及初始化方法

        /// <summary>
        /// 普通类 对象容器
        /// </summary>
        public readonly Dictionary<string, ObjectPoolData> ObjectPoolDataDic =
            new Dictionary<string, ObjectPoolData>();

        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="keyName">对象池key</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        public void InitObjectPool<T>(string keyName, int maxCapacity = -1, int defaultQuantity = 0) where T : new()
        {
            ObjectPoolData poolData;

            lock (ObjectPoolDataDic)
            {
                //尝试获取对象池
                ObjectPoolDataDic.TryGetValue(keyName, out poolData);
            }
            
            //如果无法获取到
            if (poolData == null)
            {
                //那就创建新的对象池
                poolData = CreateObjectPoolData(keyName, maxCapacity);
            }
            //获取到了
            else
            {
                //那就更新一下容量限制
                poolData.maxCapacity = maxCapacity;
            }
            
            //如果要在初始化的时候生成几个对象
            if (defaultQuantity != 0)
            {
                int nowCapacity = poolData.PoolQueue.Count;
                // 生成差值容量个数的物体放入对象池
                for (int i = 0; i < defaultQuantity - nowCapacity; i++)
                {
                    T obj = new T();
                    PushObject(obj, keyName);
                }
            }
        }

        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        public void InitObjectPool<T>(int maxCapacity = -1, int defaultQuantity = 0) where T : new()
        {
            InitObjectPool<T>(typeof(T).GetNiceName(), maxCapacity, defaultQuantity);
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="keyName">资源名称</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        public void InitObjectPool(string keyName, int maxCapacity = -1)
        {
            ObjectPoolData poolData;
            
            lock (ObjectPoolDataDic)
            {
                //设置的对象池已经存在
                if (ObjectPoolDataDic.TryGetValue(keyName, out poolData))
                {
                    //更新容量限制
                    poolData.maxCapacity = maxCapacity;
                }
            }

            //设置的对象池不存在
            if(poolData == null)
            {
                //那就创建对象池
                CreateObjectPoolData(keyName, maxCapacity);
            }
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        public void InitObjectPool(Type type, int maxCapacity = -1)
        {
            InitObjectPool(type.GetNiceName(), maxCapacity);
        }

        /// <summary>
        /// 创建一条新的对象池数据
        /// </summary>
        private ObjectPoolData CreateObjectPoolData(string keyName, int capacity = -1)
        {
            // 交由Object对象池拿到poolData的类
            ObjectPoolData poolData = GetObject<ObjectPoolData>();

            //Object对象池中没有再new
            if (poolData == null)
            {
                poolData = new ObjectPoolData(capacity);
            }

            //对拿到的poolData副本进行初始化（覆盖之前的数据）
            poolData.maxCapacity = capacity;
            lock (ObjectPoolDataDic)
            {
                ObjectPoolDataDic.Add(keyName, poolData);
            }
            return poolData;
        }

        #endregion

        #region ObjectPool相关功能

        /// <summary>
        /// 尝试获取对象，如果没有那就直接创建一个
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        public T GetOrNewObject<T>() where T : new()
        {
            object obj = null;
            var keyName = typeof(T).GetNiceName();
            ObjectPoolData poolData;
            
            //获取数据
            lock (ObjectPoolDataDic)
            {
                ObjectPoolDataDic.TryGetValue(keyName, out poolData);
            }
            //如果获取到了对象池数据
            if (poolData != null)
            {
                lock (poolData)
                {
                    //并且对象池里面还有对象，那就获取
                    if (poolData.PoolQueue.Count > 0)
                    {
                        obj = poolData.GetObj();
#if UNITY_EDITOR
                        if (FrameRoot.EditorEventModule != null)
                            FrameRoot.EditorEventModule.EventTrigger("OnGetObject", typeof(T).GetNiceName(), 1);
#endif
                    }
                }
            }
            
            //如果为空那就新建
            if (obj == null)
            {
                obj = new T();
            }

            //返回结果
            return (T)obj;
        }
        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <param name="keyName">对象的key</param>
        /// <returns>找不到就返回null</returns>
        public object GetObject(string keyName)
        {
            object obj = null;
            ObjectPoolData poolData;
            
            //获取数据
            lock (ObjectPoolDataDic)
            {
                ObjectPoolDataDic.TryGetValue(keyName, out poolData);
            }
            //如果获取到了对象池数据
            if (poolData != null)
            {
                lock (poolData)
                {
                    //并且对象池里面还有对象，那就获取
                    if (poolData.PoolQueue.Count > 0)
                    {
                        obj = poolData.GetObj();
#if UNITY_EDITOR
                        if (FrameRoot.EditorEventModule != null)
                            FrameRoot.EditorEventModule.EventTrigger("OnGetObject", keyName, 1);
#endif
                    }
                }
            }

            return obj;
        }
        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <returns>如果池子中没有就返回null</returns>
        public object GetObject(Type type)
        {
            return GetObject(type.GetNiceName());
        }
        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>如果池子中没有就返回null</returns>
        public T GetObject<T>() where T : class
        {
            return (T)GetObject(typeof(T));
        }
        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <param name="keyName">对象池key</param>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>如果池子中没有就返回null</returns>
        public T GetObject<T>(string keyName) where T : class
        {
            return (T)GetObject(keyName);
        }
        /// <summary>
        /// 把object推进池子
        /// </summary>
        /// <param name="obj">要推入池子中的对象</param>
        /// <returns>成功推入就返回true</returns>
        public bool PushObject(object obj)
        {
            return PushObject(obj, obj.GetType().GetNiceName());
        }

        /// <summary>
        /// 把object推进池子
        /// </summary>
        /// <param name="obj">要推入池子中的对象</param>
        /// <param name="keyName">对象池key</param>
        /// <returns>成功推入就返回true</returns>
        public bool PushObject(object obj, string keyName)
        {
            //获取对象池数据
            ObjectPoolData poolData;
            
            lock (ObjectPoolDataDic)
            {
                //如果还没有对象池的话那就创建
                if (ObjectPoolDataDic.TryGetValue(keyName, out poolData) == false)
                {
                    poolData = CreateObjectPoolData(keyName);
                }
            }
            
            //然后把对象推进池子
            lock (poolData)
            {
                return poolData.PushObj(obj);
            }
            
        }

        /// <summary>
        /// 清理所有对象池
        /// </summary>
        public void ClearAll()
        {
            lock (ObjectPoolDataDic)
            {
                //遍历逐个清理对象池
                var enumerator = ObjectPoolDataDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Value.Destroy();
                }
                
                //清空字典
                ObjectPoolDataDic.Clear();
            }

        }
        /// <summary>
        /// 清空某个类型的对象池
        /// </summary>
        /// <param name="keyName">对象池key</param>
        public void ClearObject(string keyName)
        {
            lock (ObjectPoolDataDic)
            {
                //如果没有数据那就返回
                if (!ObjectPoolDataDic.TryGetValue(keyName, out ObjectPoolData objectPoolData)) return;
                
                objectPoolData.Destroy(true);
                ObjectPoolDataDic.Remove(keyName);
            }
            
        }
        /// <summary>
        /// 清空某个类型的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        public void ClearObject<T>()
        {
            ClearObject(typeof(T).GetNiceName());
        }
        /// <summary>
        /// 清空某个类型的对象池
        /// </summary>
        /// <param name="type">对象类型</param>
        public void ClearObject(Type type)
        {
            ClearObject(type.GetNiceName());
        }

        #endregion

    }
}