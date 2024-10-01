//****************** 代码文件申明 ***********************
//* 文件：GameObjectPoolModule
//* 作者：wheat
//* 创建时间：2024/09/24 13:11:25 星期二
//* 描述：GameObject对象池的管理器
//*******************************************************
using System.Collections.Generic;
using KFrame.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace KFrame.Systems
{
    public class GameObjectPoolModule
    {
        #region GameObjectPoolModule持有的数据及初始化方法

        /// <summary>
        /// 根节点
        /// </summary>
        private Transform poolRootTransform;

        /// <summary>
        /// 默认的父物体
        /// </summary>
        private static GameObject defaultParentInGameScene;

        /// <summary>
        /// 默认的父物体
        /// </summary>
        public static GameObject DefaultDefaultParentInGameScene
        {
            get
            {
                //如果为空那就新建一个
                if (defaultParentInGameScene == null)
                {
                    defaultParentInGameScene = new GameObject("GameObjectPool");
                }

                //之前的如果失活了 ，并且当前激活的场景和之前那个不一样
                if (!defaultParentInGameScene.activeSelf && defaultParentInGameScene.scene.name != SceneManager.GetActiveScene().name)
                {
                    //那就新建一个
                    defaultParentInGameScene = new GameObject("GameObjectPool");
                }

                return defaultParentInGameScene;
            }
        }

        /// <summary>
        /// 对象池数据字典
        /// </summary>
        public readonly Dictionary<string, GameObjectPoolData> GameObjectPoolDataDic =
            new Dictionary<string, GameObjectPoolData>();
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="rootTransform">对象池根节点</param>
        public void Init(Transform rootTransform)
        {
            poolRootTransform = rootTransform;
        }

        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="keyName">资源名称</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        /// <param name="prefab">填写默认容量时预先放入的对象</param>
        public void InitGameObjectPool(string keyName, int maxCapacity = -1, GameObject prefab = null,
            int defaultQuantity = 0)
        {
            if (defaultQuantity > maxCapacity && maxCapacity != -1)
            {
                Debug.LogWarning("默认容量超出最大容量限制");
                return;
            }
            
            //先获取对象池数据
            GameObjectPoolData poolData;
            lock (GameObjectPoolDataDic)
            {
                GameObjectPoolDataDic.TryGetValue(keyName, out poolData);
            }
            
            //设置的对象池已经存在
            if (poolData != null)
            {
                //更新容量限制
                poolData.MaxCapacity = maxCapacity;

            }
            //设置的对象池不存在
            else
            {
                //创建对象池
                poolData = CreateGameObjectPoolData(keyName, maxCapacity);
            }
            
            
            //如果要新建几个对象放入
            if (defaultQuantity <= 0) return;
            
            if (prefab != null)
            {
                int nowCapacity = poolData.PoolQueue.Count;
                // 生成差值容量个数的物体放入对象池
                for (int i = 0; i < defaultQuantity - nowCapacity; i++)
                {
                    GameObject go = Object.Instantiate(prefab);
                    go.name = prefab.name;
                    poolData.PushObj(go);
                }
            }
            else
            {
                Debug.LogWarning("默认对象未指定");
            }
        }

        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        /// <param name="prefab">填写默认容量时预先放入的对象</param>
        public void InitGameObjectPool(GameObject prefab, int maxCapacity = -1, int defaultQuantity = 0)
        {
            InitGameObjectPool(prefab.name, maxCapacity, prefab, defaultQuantity);
        }


        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="maxCapacity">最大容量，-1代表无限</param>
        /// <param name="gameObjects">默认要放进来的对象数组</param>
        public void InitGameObjectPool(string keyName, int maxCapacity = -1, GameObject[] gameObjects = null)
        {
            if (gameObjects != null && gameObjects.Length > maxCapacity && maxCapacity != -1)
            {
                Debug.LogWarning("默认容量超出最大容量限制");
                return;
            }
            
            //先获取对象池数据
            GameObjectPoolData poolData;
            lock (GameObjectPoolDataDic)
            {
                GameObjectPoolDataDic.TryGetValue(keyName, out poolData);
            }

            //设置的对象池已经存在
            if (poolData != null)
            {
                //更新容量限制
                poolData.MaxCapacity = maxCapacity;
            }
            //设置的对象池不存在
            else
            {
                //创建对象池
                poolData = CreateGameObjectPoolData(keyName, maxCapacity);
            }

            //如果要新建几个对象放入对象池
            if (gameObjects is not { Length: > 0 }) return;
            
            int nowCapacity = poolData.PoolQueue.Count;
            // 生成差值容量个数的物体放入对象池
            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (i < gameObjects.Length - nowCapacity)
                {
                    gameObjects[i].gameObject.name = keyName;
                    poolData.PushObj(gameObjects[i].gameObject);
                }
                else
                {
                    Object.Destroy(gameObjects[i].gameObject);
                }
            }
        }

        /// <summary>
        /// 创建一条新的对象池数据
        /// </summary>
        private GameObjectPoolData CreateGameObjectPoolData(string keyName, int maxCapacity = -1)
        {
            //交由Object对象池拿到poolData的类
            GameObjectPoolData poolData = PoolSystem.GetObject<GameObjectPoolData>();

            //Object对象池中没有再new
            if (poolData == null) poolData = new GameObjectPoolData(maxCapacity);

            //对拿到的poolData副本进行初始化（覆盖之前的数据）
            poolData.Init(keyName, poolRootTransform, maxCapacity);
            lock (GameObjectPoolDataDic)
            {
                GameObjectPoolDataDic.Add(keyName, poolData);
            }
            return poolData;
        }

        #endregion

        #region GameObjectPool相关功能

        /// <summary>
        /// 从对象池拿取GameObject
        /// </summary>
        /// <param name="keyName">字典Key</param>
        /// <param name="parent">父物体</param>
        /// <param name="isActiveStart">刚生成的时候是否激活</param>
        /// <param name="callBack">对这个物品进行的额外操作</param>
        /// <returns>如果池子里没有了那就返回null</returns>
        public GameObject GetGameObject(string keyName, Transform parent = null, bool isActiveStart = true,
            UnityAction<GameObject> callBack = null)
        {
            GameObject obj = null;
            GameObjectPoolData poolData;
            
            //先尝试获取PoolData
            lock (GameObjectPoolDataDic)
            {
                GameObjectPoolDataDic.TryGetValue(keyName, out poolData);
            }
            //获取到PoolData后从中获取GameObject
            if (poolData != null)
            {
                lock (poolData)
                {
                    if (poolData.PoolQueue.Count > 0)
                    {
                        //如果有并且池的里有预制体，那就获取
                        obj = poolData.GetObj(parent, isActiveStart);
                    }
                }

            }
            
            //触发回调函数，然后返回结果
            callBack?.Invoke(obj); 
            return obj;
        }

        /// <summary>
        /// 使用assetName取东西 如果为null直接使用资源中心实例化一个GO
        /// <remarks>
        /// 推荐直接使用ResSystem的API ResSystem 也方便选择同步异步
        /// </remarks>
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="parent">父物体</param>
        /// <param name="isActiveStart">是否立即激活</param>
        /// <param name="callBack">回调函数</param>
        /// <returns></returns>
        public GameObject GetOrNewGameObject(string assetName, Transform parent = null, bool isActiveStart = true,
            UnityAction<GameObject> callBack = null)
        {
            GameObject obj = null;
            GameObjectPoolData poolData;
            
            //先尝试获取PoolData
            lock (GameObjectPoolDataDic)
            {
                GameObjectPoolDataDic.TryGetValue(assetName, out poolData);
            }
            //如果池子中有GameObject那就处理后返回GameObject
            if (poolData != null)
            {
                lock (poolData)
                {
                    if (poolData.PoolQueue.Count > 0)
                    {
                        //如果有并且池的里有预制体，那就获取
                        obj = poolData.GetObj(parent, isActiveStart);

                        callBack?.Invoke(obj);
                        return obj;
                    }
                }
            }
            
            //如果没有那就生成一个
                
            //加载资源 创建对象给外部使用
            var objPrefab = ResSystem.LoadAsset<GameObject>(assetName);
            obj = Object.Instantiate(objPrefab, !parent ? DefaultDefaultParentInGameScene.transform : parent);
            obj.SetActive(isActiveStart);
            obj.name = assetName;
            callBack?.Invoke(obj);
            
            return obj;
        }

        /// <summary>
        /// 使用assetName取东西 如果为null直接使用资源中心实例化一个GO
        /// <remarks>
        /// 推荐直接使用ResSystem的API ResSystem 也方便选择同步异步
        /// </remarks>
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="parent">父物体</param>
        /// <param name="isActiveStart">是否立即激活</param>
        /// <param name="callBack">回调函数</param>
        /// <returns>如果池子里没有了那就返回null</returns>
        public T GetOrNewGameObject<T>(string assetName, Transform parent = null, bool isActiveStart = true,
            UnityAction<T> callBack = null) where T : Component
        {
            //先生成GameObject
            GameObject obj = GetOrNewGameObject(assetName, parent, isActiveStart, null);
            
            //如果没有那就返回null
            if (obj == null)
            {
                callBack?.Invoke(null);
                return null;
            }
            else
            {
                //不为空那就获取component然后调用回调函数
                T component = obj.GetComponent<T>();
                callBack?.Invoke(component);

                return component;
            }

        }

        /// <summary>
        /// 通过预制体取GameObject
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <param name="parent">父级</param>
        /// <param name="simplyNameType">简化物品名称方法 默认0 为完全简化物品名称 0 只简化(Clone) 1 不进行简化</param>
        /// <param name="isActiveStart">初始是否为激活状态</param>
        /// <param name="callBack">回调</param>
        /// <returns>如果池子里没有了那就返回null</returns>
        public GameObject GetOrNewGameObject(GameObject prefab, Transform parent = null, bool isActiveStart = true,
            UnityAction<GameObject> callBack = null, int simplyNameType = 0)
        {
            GameObject obj = GetOrNewGameObject(prefab.name, parent, isActiveStart);
            
            //如果obj不为空
            if (obj)
            {
                //简化预制体名称
                if (simplyNameType == -1)
                {
                    obj.CompleteSimplyPrefabName();
                }
                else if (simplyNameType == 0)
                {
                    obj.SimplyPrefabName();
                }
            }

            //回调函数
            callBack?.Invoke(obj);

            return obj;
        }

        /// <summary>
        /// 通过预制体取GameObject
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <param name="parent">父级</param>
        /// <param name="simplyNameType">简化物品名称方法 默认0 为完全简化物品名称 0 只简化(Clone) 1 不进行简化</param>
        /// <param name="isActiveStart">初始是否为激活状态</param>
        /// <param name="callBack">回调函数</param>
        /// <returns>如果池子里没有了那就返回null</returns>
        public T GetOrNewGameObject<T>(GameObject prefab, Transform parent = null, bool isActiveStart = true,
            UnityAction<T> callBack = null, int simplyNameType = 0) where T : Component
        {
            GameObject obj = GetOrNewGameObject(prefab.name, parent, isActiveStart);
            T component = null;
            
            //如果obj不为空
            if (obj)
            {
                //简化预制体名称
                if (simplyNameType == -1)
                {
                    obj.CompleteSimplyPrefabName();
                }
                else if (simplyNameType == 0)
                {
                    obj.SimplyPrefabName();
                }

                component = obj.GetComponent<T>();
            }

            //回调函数
            callBack?.Invoke(component);

            return component;
        }

        /// <summary>
        /// 把GameObject推进对象池
        /// </summary>
        /// <param name="keyName">对象池的key</param>
        /// <param name="obj">要推入对象池的GameObject</param>
        public bool PushGameObject(string keyName, GameObject obj)
        {
            //如果GameObject为空或者key为空那就推入失败
            if (!obj || string.IsNullOrEmpty(keyName)) return false;

            GameObjectPoolData poolData;
            
            lock (GameObjectPoolDataDic)
            {
                //获取池子然后推入
                if (GameObjectPoolDataDic.TryGetValue(keyName, out poolData))
                {
                    return poolData.PushObj(obj);
                }
            }

            //如果还没有池子
            //那就创建池子然后推入
            poolData = CreateGameObjectPoolData(keyName);
            return poolData.PushObj(obj);
        }

        /// <summary>
        /// 把GameObject推进对象池
        /// </summary>
        public void PushGameObject(GameObject go)
        {
            //如果GameObject为空那就不能推入
            if(go == null) return;
            PushGameObject(go.name, go);
        }
        /// <summary>
        /// 清空指定的对象池
        /// </summary>
        /// <param name="keyName">要清空的对象池的key</param>
        public void Clear(string keyName)
        {
            //获取对象池，如果有的话那就清空
            lock (GameObjectPoolDataDic)
            {
                if (GameObjectPoolDataDic.TryGetValue(keyName, out GameObjectPoolData gameObjectPoolData))
                {
                    //释放数据 并把自己也推入对象池
                    gameObjectPoolData.Dispose(true);
                    GameObjectPoolDataDic.Remove(keyName);
                }
            }

        }
        /// <summary>
        /// 清空所有对象池
        /// </summary>
        public void ClearAll()
        {
            //逐个遍历清空
            lock (GameObjectPoolDataDic)
            {
                var enumerator = GameObjectPoolDataDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Value.Dispose();
                }
                //清空字典
                GameObjectPoolDataDic.Clear();
            }

        }

        #endregion
    }
}