//****************** 代码文件申明 ***********************
//* 文件：PoolSystem
//* 作者：wheat
//* 创建时间：2024/09/24 13:03:22 星期二
//* 描述：对象池系统
//*******************************************************
using System;
using System.Collections.Generic;
using KFrame.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace KFrame.Systems
{
    public static class PoolSystem
    {
        #region 对象池系统数据及静态构造方法

        /// <summary>
        /// 对象池层物体的游戏物体名称，用于将层物体也放进对象池
        /// </summary>
        public const string PoolLayerGameObjectName = "PoolLayerGameObjectName";
        /// <summary>
        /// GameObject对象池Module
        /// </summary>
        private static GameObjectPoolModule gameObjectPoolModule;
        /// <summary>
        /// Object对象池Module
        /// </summary>
        private static ObjectPoolModule objectPoolModule;
        /// <summary>
        /// 对象池根节点
        /// </summary>
        private static Transform poolRootTransform;
        /// <summary>
        /// 初始化对象池
        /// </summary>
        public static void Init()
        {
            gameObjectPoolModule = new GameObjectPoolModule();
            objectPoolModule = new ObjectPoolModule();
            poolRootTransform = new GameObject("PoolRoot").transform;
            poolRootTransform.position = Vector3.zero;
            poolRootTransform.SetParent(FrameRoot.RootTransform);
            gameObjectPoolModule.Init(poolRootTransform);
        }

        #endregion

        #region GameObject  对象池相关API(初始化、取出、放入、清空)

        /// <summary>
        /// 初始化一个GameObject类型的对象池类型
        /// </summary>
        /// <param name="keyName">资源名称</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        /// <param name="prefab">填写默认容量时预先放入的对象</param>
        public static void InitGameObjectPool(string keyName, int maxCapacity = -1, GameObject prefab = null,
            int defaultQuantity = 0)
        {
            gameObjectPoolModule.InitGameObjectPool(keyName, maxCapacity, prefab, defaultQuantity);
#if UNITY_EDITOR
            if (FrameRoot.EditorEventModule != null)
                FrameRoot.EditorEventModule.EventTrigger("OnInitGameObjectPool", keyName, defaultQuantity);
#endif
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="maxCapacity">最大容量，-1代表无限</param>
        /// <param name="gameObjects">默认要放进来的对象数组</param>
        public static void InitGameObjectPool(string keyName, int maxCapacity, GameObject[] gameObjects = null)
        {
            gameObjectPoolModule.InitGameObjectPool(keyName, maxCapacity, gameObjects);
#if UNITY_EDITOR
            if (FrameRoot.EditorEventModule == null) return;
            if (gameObjects != null)
                FrameRoot.EditorEventModule.EventTrigger("OnInitGameObjectPool", keyName,
                    gameObjects.Length);
#endif
        }


        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        /// <param name="prefab">填写默认容量时预先放入的对象</param>
        public static void InitGameObjectPool(GameObject prefab, int maxCapacity = -1, int defaultQuantity = 0)
        {
            InitGameObjectPool(prefab.name, maxCapacity, prefab, defaultQuantity);
        }

        /// <summary>
        /// 通过AB资源名从对象池拿东西，如果对象池没初始化，则直接使用资源中心New
        /// 注意：这会导致计数器值为负数 不推荐使用
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="isActiveStart">是否在一开始激活</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="parent">父物体</param>
        /// <returns>取出的GO</returns>
        public static GameObject GetOrNewGameObject(string assetName, Transform parent = null,
            bool isActiveStart = true,
            UnityAction<GameObject> callBack = null)
        {
            GameObject go =
                gameObjectPoolModule.GetOrNewGameObject(assetName, parent, isActiveStart, callBack);
#if UNITY_EDITOR
            if (go != null && FrameRoot.EditorEventModule != null)
                FrameRoot.EditorEventModule.EventTrigger("OnGetGameObject", assetName, 1);
#endif
            return go;
        }

        /// <summary>
        /// 通过AB资源名从对象池拿东西，如果对象池没初始化，则直接使用资源中心New
        /// 注意：这会导致计数器值为负数 不推荐使用
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="isActiveStart">是否在一开始激活</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="parent">父物体</param>
        /// <returns>取出的GO</returns>
        public static T GetOrNewGameObject<T>(string assetName, Transform parent = null,
            bool isActiveStart = true,
            UnityAction<T> callBack = null) where T : Component
        {
            T go =
                gameObjectPoolModule.GetOrNewGameObject(assetName, parent, isActiveStart, callBack);
#if UNITY_EDITOR
            if (go != null && FrameRoot.EditorEventModule != null)
                FrameRoot.EditorEventModule.EventTrigger("OnGetGameObject", assetName, 1);
#endif
            return go;
        }


        /// <summary>
        /// 通过预制体从对象池拿东西，如果对象池没初始化，则直接新实例化一个
        /// 注意：这里会自动调用资源中心 更建议直接使用资源中心去加载对象 资源中心也会自动检测对象池
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <param name="parent">父物体</param>
        /// <param name="isActiveStart">是否在一开始激活</param>
        /// <param name="callBack">回调函数</param>
        /// <returns>取出的GO</returns>
        public static GameObject GetOrNewGameObject(GameObject prefab, Transform parent = null,
            bool isActiveStart = true,
            UnityAction<GameObject> callBack = null)
        {
            GameObject go = gameObjectPoolModule.GetOrNewGameObject(prefab, parent, isActiveStart, callBack);
#if UNITY_EDITOR
            if (go && FrameRoot.EditorEventModule != null)
                FrameRoot.EditorEventModule.EventTrigger("OnGetGameObject", prefab.name, 1);
#endif
            return go;
        }

        public static T GetOrNewGameObject<T>(GameObject prefab, Transform parent = null,
            bool isActiveStart = true,
            UnityAction<T> callBack = null) where T : Component
        {
            T go = gameObjectPoolModule.GetOrNewGameObject(prefab, parent, isActiveStart, callBack);

            return go;
        }


        /// <summary>
        /// 获取GameObject，没有则返回Null
        /// </summary>
        public static GameObject GetGameObject(string keyName, Transform parent = null, bool isActiveStart = true,
            UnityAction<GameObject> callBack = null)
        {
            GameObject go = gameObjectPoolModule.GetGameObject(keyName, parent, isActiveStart, callBack);
#if UNITY_EDITOR
            if (go && FrameRoot.EditorEventModule != null)
                FrameRoot.EditorEventModule.EventTrigger("OnGetGameObject", keyName, 1);
#endif
            return go;
        }

        /// <summary>
        /// 根据预制体获取GameObject，没有则返回Null
        /// T:组件
        /// </summary>
        public static GameObject GetGameObject(GameObject prefab, Transform parent = null, bool isActiveStart = true,
            UnityAction<GameObject> callBack = null)
        {
            Debug.Log(prefab.name);
            GameObject go = gameObjectPoolModule.GetGameObject(prefab.name, parent, isActiveStart, callBack);

#if UNITY_EDITOR
            if (go != null && FrameRoot.EditorEventModule != null)
                FrameRoot.EditorEventModule.EventTrigger("OnGetGameObject", prefab.name, 1);
#endif
            return go;
        }


        /// <summary>
        /// 获取GameObject，没有则返回Null
        /// T:组件
        /// </summary>
        public static T GetGameObject<T>(string keyName, Transform parent = null) where T : Component
        {
            GameObject go = GetGameObject(keyName, parent);
#if UNITY_EDITOR
            if (go != null && FrameRoot.EditorEventModule != null)
                FrameRoot.EditorEventModule.EventTrigger("OnGetGameObject", keyName, 1);
#endif
            return go != null ? go.GetComponent<T>() : null;
        }

        /// <summary>
        /// 游戏物体放置对象池中
        /// </summary>
        /// <param name="keyName">对象池中的key</param>
        /// <param name="obj">放入的物体</param>
        public static bool PushGameObject(string keyName, GameObject obj)
        {
            if (!obj.IsNull())
            {
                bool res = gameObjectPoolModule.PushGameObject(keyName, obj);
#if UNITY_EDITOR
                if (FrameRoot.EditorEventModule != null && res)
                    FrameRoot.EditorEventModule.EventTrigger("OnPushGameObject", keyName, 1);
#endif
                return res;
            }

            Debug.LogWarning("不能将Null放置对象池");
            return false;
        }

        /// <summary>
        /// 游戏物体放置对象池中
        /// </summary>
        /// <param name="obj">放入的物体,并且基于它的name来确定它是什么物体</param>
        public static bool PushGameObject(GameObject obj) { return PushGameObject(obj.name, obj); }

        /// <summary>
        /// 清除某个游戏物体在对象池中的所有数据
        /// </summary>
        public static void ClearGameObject(string keyName)
        {
            gameObjectPoolModule.Clear(keyName);
#if UNITY_EDITOR
            if (FrameRoot.EditorEventModule != null)
                FrameRoot.EditorEventModule.EventTrigger("OnClearGameObject", keyName);
#endif
        }

        #endregion

        #region Object      对象池相关API(初始化、取出、放入、清空)

        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="keyName">资源名称</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        public static void InitObjectPool<T>(string keyName, int maxCapacity = -1, int defaultQuantity = 0)
            where T : new()
        {
            objectPoolModule.InitObjectPool<T>(keyName, maxCapacity, defaultQuantity);
#if UNITY_EDITOR
            if (FrameRoot.EditorEventModule != null)
                FrameRoot.EditorEventModule.EventTrigger("OnInitObjectPool", keyName, defaultQuantity);
#endif
        }

        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        public static void InitObjectPool<T>(int maxCapacity = -1, int defaultQuantity = 0) where T : new()
        {
            InitObjectPool<T>(typeof(T).FullName, maxCapacity, defaultQuantity);
        }

        /// <summary>
        /// 初始化一个普通C#对象池类型
        /// </summary>
        /// <param name="keyName">keyName</param>
        /// <param name="maxCapacity">容量，超出时会丢弃而不是进入对象池，-1代表无限</param>
        public static void InitObjectPool(string keyName, int maxCapacity = -1)
        {
            objectPoolModule.InitObjectPool(keyName, maxCapacity);
#if UNITY_EDITOR
            if (FrameRoot.EditorEventModule != null)
                FrameRoot.EditorEventModule.EventTrigger("OnInitObjectPool", keyName, 0);
#endif
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        public static void InitObjectPool(Type type, int maxCapacity = -1)
        {
            objectPoolModule.InitObjectPool(type, maxCapacity);
#if UNITY_EDITOR
            if (FrameRoot.EditorEventModule != null)
                FrameRoot.EditorEventModule.EventTrigger("OnInitObjectPool", type.FullName, 0);
#endif
        }

        /// <summary>
        /// 获取或新New一个Object
        /// 注意：这会导致计数器值为负数或混乱 不推荐使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetOrNewObject<T>() where T : new()
        {
            object obj = objectPoolModule.GetOrNewObject<T>();
            return (T)obj;
        }

        /// <summary>
        /// 获取普通对象（非GameObject）
        /// </summary>
        public static T GetObject<T>() where T : class { return GetObject<T>(typeof(T).FullName); }

        /// <summary>
        /// 获取普通对象（非GameObject）
        /// </summary>
        public static T GetObject<T>(string keyName) where T : class
        {
            object obj = GetObject(keyName);
            if (obj == null) return null;
            return (T)obj;
        }

        /// <summary>
        /// 获取普通对象（非GameObject）
        /// </summary>
        public static object GetObject(Type type) { return GetObject(type.FullName); }

        /// <summary>
        /// 获取普通对象（非GameObject）
        /// </summary>
        public static object GetObject(string keyName)
        {
            object obj = objectPoolModule.GetObject(keyName);
#if UNITY_EDITOR
            if (obj != null)
            {
                if (FrameRoot.EditorEventModule != null)
                    FrameRoot.EditorEventModule.EventTrigger("OnGetObject", keyName, 1);
            }
#endif

            return obj;
        }

        /// <summary>
        /// 普通对象（非GameObject）放置对象池中
        /// 基于类型存放
        /// </summary>
        public static bool PushObject(object obj) { return PushObject(obj, obj.GetType().FullName); }

        /// <summary>
        /// 普通对象（非GameObject）放置对象池中
        /// 基于KeyName存放
        /// </summary>
        public static bool PushObject(object obj, string keyName)
        {
            if (obj == null)
            {
                Debug.LogWarning("您正在将Null放置对象池");
                return false;
            }

            bool res = objectPoolModule.PushObject(obj, keyName);
#if UNITY_EDITOR
            if (FrameRoot.EditorEventModule != null && res)
            {
                FrameRoot.EditorEventModule.EventTrigger("OnPushObject", keyName, 1);
            }
#endif
            return res;
        }

        /// <summary>
        /// 清理某个C#类型数据
        /// </summary>
        public static void ClearObject<T>() { ClearObject(typeof(T).FullName); }

        /// <summary>
        /// 清理某个C#类型数据
        /// </summary>
        public static void ClearObject(Type type) { ClearObject(type.FullName); }

        /// <summary>
        /// 清理某个C#类型数据
        /// </summary>
        public static void ClearObject(string keyName)
        {
#if UNITY_EDITOR
            if (FrameRoot.EditorEventModule != null)
            {
                FrameRoot.EditorEventModule.EventTrigger("OnClearnObject", keyName);
            }
#endif
            objectPoolModule.ClearObject(keyName);
        }

        #endregion

        #region Editor

#if UNITY_EDITOR
        public static Dictionary<string, GameObjectPoolData> GetGameObjectLayerDatas()
        {
            return gameObjectPoolModule.GameObjectPoolDataDic;
        }

        public static Dictionary<string, ObjectPoolData> GetObjectLayerDatas()
        {
            return objectPoolModule.ObjectPoolDataDic;
        }
#endif

        #endregion
    }
}