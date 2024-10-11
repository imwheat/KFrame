#if ENABLE_ADDRESSABLES
using System;
using System.Collections.Generic;
using System.ComponentModel;
using KFrame.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace KFrame.Systems
{
    public static class ResSystem
    {
        #region 普通class对象

        /// <summary>
        /// 获取实例-普通Class
        /// 如果类型需要缓存，会从对象池中获取
        /// 如果对象池没有或new一个返回
        /// </summary>
        public static T GetOrNew<T>() where T : class, new()
        {
            var obj = PoolSystem.GetObject<T>() ?? new T();
            return obj;
        }


        /// <summary>
        /// 获取实例-普通Class
        /// 如果类型需要缓存，会从对象池中获取
        /// 如果对象池没有或new一个返回
        /// </summary>
        /// <param name="keyName">对象池中的名称</param>
        public static T GetOrNew<T>(string keyName) where T : class, new()
        {
            var obj = PoolSystem.GetObject<T>(keyName) ?? new T();
            return obj;
        }

        /// <summary>
        /// 卸载普通对象，这里是使用对象池的方式
        /// </summary>
        public static void PushObjectInPool(System.Object obj)
        {
            PoolSystem.PushObject(obj);
        }

        /// <summary>
        /// 普通对象（非GameObject）放置对象池中
        /// 基于KeyName存放
        /// </summary>
        public static void PushObjectInPool(object obj, string keyName)
        {
            PoolSystem.PushObject(obj, keyName);
        }

        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="keyName">资源名称</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        public static void InitObjectPool<T>(string keyName, int maxCapacity = -1, int defaultQuantity = 0)
            where T : new()
        {
            PoolSystem.InitObjectPool<T>(keyName, maxCapacity, defaultQuantity);
        }

        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        public static void InitObjectPool<T>(int maxCapacity = -1, int defaultQuantity = 0) where T : new()
        {
            PoolSystem.InitObjectPool<T>(maxCapacity, defaultQuantity);
        }

        /// <summary>
        /// 初始化一个普通C#对象池类型
        /// </summary>
        /// <param name="keyName">keyName</param>
        /// <param name="maxCapacity">容量，超出时会丢弃而不是进入对象池，-1代表无限</param>
        public static void InitObjectPool(string keyName, int maxCapacity = -1)
        {
            PoolSystem.InitObjectPool(keyName, maxCapacity);
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        public static void InitObjectPool(System.Type type, int maxCapacity = -1)
        {
            PoolSystem.InitObjectPool(type, maxCapacity);
        }

        #endregion

        #region 游戏物体

        /// <summary>
        /// 自动释放资源事件，基于事件工具
        /// </summary>
        private static void AutomaticReleaseAssetAction(GameObject obj)
        {
            Addressables.ReleaseInstance(obj);
        }
        
        /// <summary>
        /// 卸载游戏对象，这里是使用对象池的方式
        /// </summary>
        public static void PushGameObjectInPool(GameObject gameObject)
        {
            PoolSystem.PushGameObject(gameObject);
        }

        /// <summary>
        /// 游戏物体放置对象池中
        /// </summary>
        /// <param name="keyName">对象池中的key</param>
        /// <param name="gameObject">放入的物体</param>
        public static void PushGameObjectInPool(string keyName, GameObject gameObject)
        {
            PoolSystem.PushGameObject(keyName, gameObject);
        }

        /// <summary>
        /// 加载游戏物体
        /// 会自动检查对象池中是否包含，如果包含则返回对象池中的
        /// </summary>
        /// <param name="keyName">对象池中的名称</param>
        /// <param name="parent">父物体</param>
        /// <param name="autoRelease">物体销毁时，会自动去调用一次Addressables.Release</param>
        public static GameObject InstantiateGameObject(Transform parent, string keyName, bool autoRelease = true)
        {
            //先尝试从对象池中获取GameObject
            GameObject go = PoolSystem.GetGameObject(keyName, parent);
            if (go) return go;
            else
            {
                //如果没有那就加载预制体后生成
                GameObject prefab = LoadAsset<GameObject>(keyName);
                if (prefab == null)
                {
                    //找不到预制体就返回null
                    return null;
                }

                go = Object.Instantiate(prefab, parent);
                if (autoRelease)
                {
                    go.transform.OnReleaseAddressableAsset(AutomaticReleaseAssetAction);
                }

                go.name = keyName;
            }

            return go;
        }

        /// <summary>
        /// 异步加载游戏物体
        /// 会自动检查对象池中是否包含，如果包含则返回对象池中的
        /// </summary>
        /// <param name="keyName">对象池中的分组名称</param>
        /// <param name="parent">父物体</param>
        /// <param name="callback">加载完成后的回调</param>
        /// <param name="autoRelease">物体销毁时，会自动去调用一次Addressables.Release</param>
        public static void InstantiateGameObjectAsync(Transform parent, string keyName, Action<GameObject> callback,
            bool autoRelease = true)
        {
            GameObject go = PoolSystem.GetGameObject(keyName, parent);
            
            if (go) callback?.Invoke(go);
            else
            {
                //如果没有那就加载预制体后生成
                LoadAssetAsync<GameObject>(keyName, (prefab) =>
                {
                    if (prefab == null)
                    {
                        //找不到预制体就返回
                        return;
                    }

                    go = Object.Instantiate(prefab, parent);
                    if (autoRelease)
                    {
                        go.transform.OnReleaseAddressableAsset(AutomaticReleaseAssetAction);
                    }

                    go.name = keyName;
                    callback?.Invoke(go);
                });

            }
        }


        /// <summary>
        /// 加载游戏物体
        /// 会自动检查对象池中是否包含，如果包含则返回对象池中的
        /// </summary>
        /// <param name="assetName">AB资源名称</param>
        /// <param name="keyName">对象池中的分组名称，可为Null</param>
        /// <param name="parent">父物体</param>
        /// <param name="autoRelease">物体销毁时，会自动去调用一次Addressables.Release</param>
        public static GameObject InstantiateGameObject(string assetName, Transform parent = null, string keyName = null,
            bool autoRelease = true)
        {
            //先尝试从对象池中获取GameObject
            GameObject go = PoolSystem.GetGameObject(keyName, parent);
            if (go) return go;
            else
            {
                //如果没有那就加载预制体后生成
                GameObject prefab = LoadAsset<GameObject>(assetName);
                if (prefab == null)
                {
                    //找不到预制体就返回null
                    return null;
                }

                go = Object.Instantiate(prefab, parent);
                if (autoRelease)
                {
                    go.transform.OnReleaseAddressableAsset(AutomaticReleaseAssetAction);
                }

                go.name = string.IsNullOrEmpty(keyName) ? assetName : keyName;
            }

            return go;
        }

        /// <summary>
        /// 异步加载游戏物体
        /// 会自动检查对象池中是否包含，如果包含则返回对象池中的
        /// </summary>
        /// <param name="assetName">AB资源名称</param>
        /// <param name="callback">加载完成后的回调</param>
        /// <param name="keyName">对象池中的分组名称，可为Null</param>
        /// <param name="parent">父物体</param>
        /// <param name="autoRelease">物体销毁时，会自动去调用一次Addressables.Release</param>
        public static void InstantiateGameObjectAsync(string assetName, Action<GameObject> callback,
            Transform parent = null, string keyName = null, bool autoRelease = true)
        {
            GameObject go;
            if (string.IsNullOrEmpty(keyName)) go = PoolSystem.GetGameObject(assetName, parent);
            else go = PoolSystem.GetGameObject(keyName, parent);

            if (go) callback?.Invoke(go);
            else
            {
                //如果没有那就加载预制体后生成
                LoadAssetAsync<GameObject>(keyName, (prefab) =>
                {
                    if (prefab == null)
                    {
                        //找不到预制体就返回
                        return;
                    }

                    go = Object.Instantiate(prefab, parent);
                    if (autoRelease)
                    {
                        go.transform.OnReleaseAddressableAsset(AutomaticReleaseAssetAction);
                    }

                    go.name = string.IsNullOrEmpty(keyName) ? assetName : keyName;
                    callback?.Invoke(go);
                });
                
            }
        }

        /// <summary>
        /// 加载游戏物体并获取组件
        /// 会自动检查对象池中是否包含，如果包含则返回对象池中的
        /// </summary>
        /// <param name="keyName">对象池中的分组名称</param>
        /// <param name="parent">父物体</param>
        /// <param name="autoRelease">物体销毁时，会自动去调用一次Addressables.Release</param>
        public static T InstantiateGameObject<T>(Transform parent, string keyName, bool autoRelease = true)
            where T : Component
        {
            GameObject go = InstantiateGameObject(parent, keyName, autoRelease);
            if (go) return go.GetComponent<T>();
            else return null;
        }

        /// <summary>
        /// 加载游戏物体并获取组件
        /// 会自动检查对象池中是否包含，如果包含则返回对象池中的
        /// </summary>
        /// <param name="assetName">AB资源名称</param>
        /// <param name="keyName">对象池中的分组名称，可为Null</param>
        /// <param name="parent">父物体</param>
        /// <param name="autoRelease">物体销毁时，会自动去调用一次Addressables.Release</param>
        public static T InstantiateGameObject<T>(string assetName, Transform parent = null, string keyName = null,
            bool autoRelease = true) where T : Component
        {
            GameObject go = InstantiateGameObject(assetName, parent, keyName, autoRelease);
            if (go) return go.GetComponent<T>();
            else return null;
        }

        /// <summary>
        /// 异步加载游戏物体并获取组件
        /// </summary>
        /// <typeparam name="T">物体身上的组件</typeparam>
        /// <param name="assetName">AB资源名称</param>
        /// <param name="callback">实例化后的回调函数</param>
        /// <param name="parent">父物体</param>
        /// <param name="keyName">对象池中的分组名称，可为Null</param>
        /// <param name="autoRelease">自动释放资源</param>
        public static void InstantiateGameObjectAsync<T>(string assetName, Action<T> callback = null,
            Transform parent = null, string keyName = null, bool autoRelease = true) where T : Component
        {
            GameObject go;
            if (keyName == null) go = PoolSystem.GetGameObject(assetName, parent);
            else go = PoolSystem.GetGameObject(keyName, parent);
            // 对象池中有
            if (go)
            {
                if (autoRelease) go.transform.OnReleaseAddressableAsset(AutomaticReleaseAssetAction);
                callback?.Invoke(go.GetComponent<T>());
                return;
            }
            
            //如果没有那就加载预制体后生成
            LoadAssetAsync<GameObject>(keyName, (prefab) =>
            {
                if (prefab == null)
                {
                    //找不到预制体就返回
                    return;
                }

                go = Object.Instantiate(prefab, parent);
                if (autoRelease)
                {
                    go.transform.OnReleaseAddressableAsset(AutomaticReleaseAssetAction);
                }

                go.name = string.IsNullOrEmpty(keyName) ? assetName : keyName;
                callback?.Invoke(go.GetComponent<T>());
            });

        }

        #endregion

        #region 游戏Asset
        
        /// <summary>
        /// 异步加载资源地址
        /// </summary>
        /// <param name="key">资源的key</param>
        /// <param name="type">资源类型</param>
        /// <returns>资源的地址的异步handle</returns>
        private static AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationsAsync(object key,
            Type type = null)
        {
            return Addressables.LoadResourceLocationsAsync(key, type);
        }
        /// <summary>
        /// 同步加载资源地址
        /// </summary>
        /// <param name="key">资源的key</param>
        /// <param name="type">资源类型</param>
        /// <returns>资源的地址</returns>
        private static IList<IResourceLocation> LoadResourceLocations(object key,
            Type type = null)
        {
            return Addressables.LoadResourceLocationsAsync(key, type).WaitForCompletion();
        }
        /// <summary>
        /// 加载Unity资源  如AudioClip Sprite 预制体
        /// 要注意，资源不在使用时候，需要调用一次Release
        /// </summary>
        /// <param name="assetName">AB资源名称</param>
        public static T LoadAsset<T>(string assetName) where T : UnityEngine.Object
        {
            var locationHandle = LoadResourceLocations(assetName);
            if (locationHandle.Count > 0)
            {
                var result = Addressables.LoadAssetAsync<T>(locationHandle[0]).WaitForCompletion();

                return result;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 异步加载Unity资源 AudioClip Sprite GameObject(预制体)
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">AB资源名称</param>
        /// <param name="callback">回调函数</param>
        public static async void LoadAssetAsync<T>(string assetName, Action<T> callback = null) where T : UnityEngine.Object
        {
            var locationHandle = await LoadResourceLocationsAsync(assetName).Task;
            
            if (locationHandle.Count > 0)
            {
                var handle = await Addressables.LoadAssetAsync<T>(locationHandle[0]).Task;
                
                callback?.Invoke(handle);
            }

        }
        
        /// <summary>
        /// 加载指定Key的所有资源
        /// 注意:批量加载时，如果释放资源要释放掉handle，直接去释放资源是无效的
        /// </summary>
        /// <typeparam name="T">加载类型</typeparam>
        /// <param name="keyName">一般是label</param>
        /// <param name="handle">用来Release时使用</param>
        /// <param name="callBackOnEveryOne">注意这里是针对每一个资源的回调</param>
        /// <returns>所有资源</returns>
        public static IList<T> LoadAssets<T>(string keyName, out AsyncOperationHandle<IList<T>> handle,
            Action<T> callBackOnEveryOne = null) where T : UnityEngine.Object
        {
            handle = Addressables.LoadAssetsAsync<T>(keyName, callBackOnEveryOne, true);
            return handle.WaitForCompletion();
        }

        /// <summary>
        /// 异步加载指定Key的所有资源
        /// 注意1:批量加载时，如果释放资源要释放掉handle，直接去释放资源是无效的
        /// 注意2:回调后使用callBack中的参数使用(.Result)即可访问资源列表
        /// </summary>
        /// <typeparam name="T">加载类型</typeparam>
        /// <param name="keyName">一般是label</param>
        /// <param name="callBack">所有资源列表的统一回调，注意这是很必要的，因为Release时需要这个handle</param>
        /// <param name="callBackOnEveryOne">注意这里是针对每一个资源的回调,可以是Null</param>
        public static void LoadAssetsAsync<T>(string keyName, Action<AsyncOperationHandle<IList<T>>> callBack,
            Action<T> callBackOnEveryOne = null) where T : UnityEngine.Object
        {
            Addressables.LoadAssetsAsync<T>(keyName, callBackOnEveryOne).Completed += callBack;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">具体对象</param>
        public static void UnloadAsset<T>(T obj)
        {
            Addressables.Release(obj);
        }

        /// <summary>
        /// 卸载因为批量加载而产生的handle
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="handle"></param>
        public static void UnLoadAssetsHandle<TObject>(AsyncOperationHandle<TObject> handle)
        {
            Addressables.Release(handle);
        }

        /// <summary>
        /// 销毁游戏物体并释放资源
        /// </summary>
        public static bool UnloadInstance(GameObject obj)
        {
            return Addressables.ReleaseInstance(obj);
        }

        #endregion

        #region 场景资源加载
        /// <summary>
        /// 从Addressables里异步加载场景资源
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="mode">加载模式</param>
        /// <returns>场景加载的异步handle</returns>
        public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(string sceneName,
            LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperationHandle<SceneInstance> sceneHandle =
                Addressables.LoadSceneAsync(sceneName, mode);

            return sceneHandle;
        }
        /// <summary>
        /// 从Addressables里同步加载场景资源
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="mode">加载模式</param>
        public static void LoadScene(string sceneName,
            LoadSceneMode mode = LoadSceneMode.Single)
        {
            Addressables.LoadSceneAsync(sceneName, mode).WaitForCompletion().ActivateAsync();
        }

        #endregion
    }
}
#endif