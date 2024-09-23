using System;
using System.Collections.Generic;
using KFrame.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace KFrame.Systems
{
    public class GameObjectPoolModule
    {
        #region GameObjectPoolModule持有的数据及初始化方法

        // 根节点
        private Transform poolRootTransform;

        /// <summary>
        /// 游戏世界的父物体
        /// </summary>
        private GameObject parentInGameScene;

        /// <summary>
        /// 外部的调用
        /// </summary>
        public GameObject ParentInGameScene
        {
            get
            {
                if (parentInGameScene == null)
                {
                    //先在当前激活的场景里面找 激活的对象
                    parentInGameScene = GameObject.Find("GameObjectPool");
                    //没找到
                    if (parentInGameScene == null)
                    {
                        parentInGameScene = new GameObject("GameObjectPool");
                    }
                }

                //之前的如果失活了 
                if (parentInGameScene.activeSelf == false)
                {
                    //判断一下当前激活的场景
                    if (parentInGameScene.scene.name != SceneManager.GetActiveScene().name)
                    {
                        //New一个新的
                        parentInGameScene = new GameObject("GameObjectPool");
                    }
                }

                return parentInGameScene;
            }
        }

        /// <summary>
        /// GameObject对象容器
        /// </summary>
        public Dictionary<string, GameObjectPoolData> GameObjectPoolDataDic { get; private set; } =
            new Dictionary<string, GameObjectPoolData>();

        public void Init(Transform poolRootTransform)
        {
            this.poolRootTransform = poolRootTransform;
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

            //设置的对象池已经存在
            if (GameObjectPoolDataDic.TryGetValue(keyName, out GameObjectPoolData poolData))
            {
                //更新容量限制
                poolData.maxCapacity = maxCapacity;
                //底层Queue自动扩容这里不管

                //在指定默认容量和默认对象时才有意义
                if (defaultQuantity > 0)
                {
                    if (prefab.IsNull() == false)
                    {
                        int nowCapacity = poolData.PoolQueue.Count;
                        // 生成差值容量个数的物体放入对象池
                        for (int i = 0; i < defaultQuantity - nowCapacity; i++)
                        {
                            GameObject go = GameObject.Instantiate(prefab);
                            go.name = prefab.name;
                            poolData.PushObj(go);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("默认对象未指定");
                    }
                }
            }
            //设置的对象池不存在
            else
            {
                //创建对象池
                poolData = CreateGameObjectPoolData(keyName, maxCapacity);

                //在指定默认容量和默认对象时才有意义
                if (defaultQuantity != 0)
                {
                    if (prefab.IsNull() == false)
                    {
                        // 生成容量个数的物体放入对象池
                        for (int i = 0; i < defaultQuantity; i++)
                        {
                            GameObject go = GameObject.Instantiate(prefab);
                            go.name = prefab.name;
                            poolData.PushObj(go);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("默认容量或默认对象未指定");
                    }
                }
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
            if (gameObjects.Length > maxCapacity && maxCapacity != -1)
            {
                Debug.LogWarning("默认容量超出最大容量限制");
                return;
            }

            //设置的对象池已经存在
            if (GameObjectPoolDataDic.TryGetValue(keyName, out GameObjectPoolData poolData))
            {
                //更新容量限制
                poolData.maxCapacity = maxCapacity;
            }
            //设置的对象池不存在
            else
            {
                //创建对象池
                poolData = CreateGameObjectPoolData(keyName, maxCapacity);
            }

            //在指定默认容量和默认对象时才有意义
            if (gameObjects.Length > 0)
            {
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
                        GameObject.Destroy(gameObjects[i].gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// 创建一条新的对象池数据
        /// </summary>
        private GameObjectPoolData CreateGameObjectPoolData(string layerName, int maxCapacity = -1)
        {
            //交由Object对象池拿到poolData的类
            GameObjectPoolData poolData = PoolSystem.GetObject<GameObjectPoolData>();

            //Object对象池中没有再new
            if (poolData == null) poolData = new GameObjectPoolData(maxCapacity);

            //对拿到的poolData副本进行初始化（覆盖之前的数据）
            poolData.Init(layerName, poolRootTransform, maxCapacity);
            GameObjectPoolDataDic.Add(layerName, poolData);
            return poolData;
        }

        #endregion

        #region GameObjectPool相关功能

        /// <summary>
        /// 从对象池拿去物品 如果没Init 返回Null
        /// </summary>
        /// <param name="keyName">字典Key</param>
        /// <param name="parent">父物体</param>
        /// <param name="callBack">对这个物品进行的额外操作</param>
        /// <returns>处理完成的GO</returns>
        public GameObject GetGameObject(string keyName, Transform parent = null, bool isActiveStart = true,
            UnityAction<GameObject> callBack = null)
        {
            GameObject obj = null;
            // 先尝试获取PoolData
            if (GameObjectPoolDataDic.TryGetValue(keyName, out GameObjectPoolData poolData) &&
                poolData.PoolQueue.Count > 0)
            {
                //如果有并且池的里有预制体，那就获取
                obj = poolData.GetObj(parent, isActiveStart);
                if (parent == null) obj.transform.SetParent(ParentInGameScene.transform);
            }

            callBack?.Invoke(obj); //触发回调方法
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
        /// <param name="isAsync">是否异步</param>
        /// <returns></returns>
        public GameObject GetOrNewGameObject(string assetName, Transform parent = null, bool isActiveStart = true,
            UnityAction<GameObject> callBack = null, bool isAsync = true)
        {
            GameObject obj = null;
            // 检查有没有这一层
            if (GameObjectPoolDataDic.TryGetValue(assetName, out GameObjectPoolData poolData) &&
                poolData.PoolQueue.Count > 0)
            {
                obj = poolData.GetObj(parent, isActiveStart);
                if (parent == null)
                {
                    obj.transform.SetParent(ParentInGameScene.transform);
                }

                callBack?.Invoke(obj);
            }
            else
            {
                if (isAsync)
                {
                    //使用异步加载资源 创建对象给外部使用
                    ResSystem.LoadAssetAsync<GameObject>(assetName, (objLA) =>
                    {
                        obj = Object.Instantiate(objLA);
                        GameObjectHandle();
                    });
                }
                else
                {
                    //使用异步加载资源 创建对象给外部使用
                    var objLA = ResSystem.LoadAsset<GameObject>(assetName);
                    obj = Object.Instantiate(objLA);
                    GameObjectHandle();
                }
            }


            void GameObjectHandle()
            {
                obj.SetActive(isActiveStart);
                obj.name = assetName;
                obj.transform.SetParent(parent == null ? ParentInGameScene.transform : parent);
                callBack?.Invoke(obj);
            }

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
        /// <param name="isAsync">是否异步</param>
        /// <returns></returns>
        public T GetOrNewGameObject<T>(string assetName, Transform parent = null, bool isActiveStart = true,
            UnityAction<T> callBack = null, bool isAsync = true) where T : class
        {
            GameObject obj = null;
            // 检查有没有这一层
            if (GameObjectPoolDataDic.TryGetValue(assetName, out GameObjectPoolData poolData) &&
                poolData.PoolQueue.Count > 0)
            {
                obj = poolData.GetObj(parent, isActiveStart);
                if (parent == null)
                {
                    obj.transform.SetParent(ParentInGameScene.transform);
                }

                callBack?.Invoke(obj.GetComponent<T>());
            }
            else
            {
                if (isAsync)
                {
                    //使用异步加载资源 创建对象给外部使用 todo 还有问题
                    ResSystem.LoadAssetAsync<GameObject>(assetName, (objLA) =>
                    {
                        obj = Object.Instantiate(objLA);
                        GameObjectHandle(obj);
                    });
                }
                else
                {
                    //使用同步加载资源 创建对象给外部使用
                    var objLA = ResSystem.LoadAsset<GameObject>(assetName);
                    obj = Object.Instantiate(objLA);
                    GameObjectHandle(obj);
                }
            }


            void GameObjectHandle(GameObject gameObject)
            {
                gameObject.SetActive(isActiveStart);
                gameObject.name = assetName;
                gameObject.transform.SetParent(parent == null ? ParentInGameScene.transform : parent);
                callBack?.Invoke(gameObject.GetComponent<T>());
            }

            return obj.GetComponent<T>();
        }

        /// <summary>
        /// 通过预制体取东西 如果为null直接使用资源New一个GO
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="simplyNameType">简化物品名称方法 默认0 为完全简化物品名称 0 只简化(Clone) 1 不进行简化</param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public GameObject GetOrNewGameObject(GameObject prefab, Transform parent = null, bool isActiveStart = true,
            UnityAction<GameObject> callBack = null, int simplyNameType = 0)
        {
            GameObject obj = null;

            // 检查有没有这一层
            if (GameObjectPoolDataDic.TryGetValue(prefab.name, out GameObjectPoolData poolData)
                && poolData.PoolQueue.Count > 0)
            {
                //如果队列中有，那就从队列中获取
                obj = poolData.GetObj(parent, isActiveStart);
                //如果父物体为空 放进默认父物体中
                if (parent == null) obj.transform.SetParent(ParentInGameScene.transform);
            }
            else //没有对象那就新生成一个
            {
                obj = Object.Instantiate(prefab, parent, false);
                //如果父物体为空 放进默认父物体中
                if (parent == null) obj.transform.SetParent(ParentInGameScene.transform);
            }

            obj.SetActive(isActiveStart);

            //简化预制体名称
            if (simplyNameType == -1)
            {
                obj.KooCompleteSimplyPrefabName();
            }
            else if (simplyNameType == 0)
            {
                obj.KooSimplyPrefabName();
            }

            //回调函数
            callBack?.Invoke(obj);


            return obj;
        }

        /// <summary>
        /// 通过预制体取东西 如果为null直接使用资源New一个GO
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="simplyNameType">简化物品名称方法 默认0 为完全简化物品名称 0 只简化(Clone) 1 不进行简化</param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public T GetOrNewGameObject<T>(GameObject prefab, Transform parent = null, bool isActiveStart = true,
            UnityAction<T> callBack = null, int simplyNameType = 0)
        {
            GameObject obj = null;

            // 检查有没有这一层
            if (GameObjectPoolDataDic.TryGetValue(prefab.name, out GameObjectPoolData poolData)
                && poolData.PoolQueue.Count > 0)
            {
                //如果队列中有，那就从队列中获取
                obj = poolData.GetObj(parent, isActiveStart);
                //如果父物体为空 放进默认父物体中
                if (parent == null) obj.transform.SetParent(ParentInGameScene.transform);
            }
            else //没有对象那就新生成一个
            {
                obj = Object.Instantiate(prefab, parent, false);
                //如果父物体为空 放进默认父物体中
                if (parent == null) obj.transform.SetParent(ParentInGameScene.transform);
            }

            obj.SetActive(isActiveStart);

            //简化预制体名称
            if (simplyNameType == -1)
            {
                obj.KooCompleteSimplyPrefabName();
            }
            else if (simplyNameType == 0)
            {
                obj.KooSimplyPrefabName();
            }

            //回调函数
            callBack?.Invoke(obj.GetComponent<T>());


            return obj.GetComponent<T>();
        }


        public void PushGameObject(GameObject go)
        {
            PushGameObject(go.name, go);
        }

        public bool PushGameObject(string keyName, GameObject obj)
        {
            // 现在有没有这一层
            if (GameObjectPoolDataDic.TryGetValue(keyName, out GameObjectPoolData poolData))
            {
                return poolData.PushObj(obj);
            }
            else
            {
                poolData = CreateGameObjectPoolData(keyName);
                return poolData.PushObj(obj);
            }
        }

        public void Clear(string keyName)
        {
            if (GameObjectPoolDataDic.TryGetValue(keyName, out GameObjectPoolData gameObjectPoolData))
            {
                gameObjectPoolData.Desotry(true); //摧毁数据 并把自己也推入对象池
                GameObjectPoolDataDic.Remove(keyName);
            }
        }

        public void ClearAll()
        {
            var enumerator = GameObjectPoolDataDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.Desotry(false);
            }

            GameObjectPoolDataDic.Clear();
        }

        #endregion
    }
}