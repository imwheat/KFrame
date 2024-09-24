//****************** 代码文件申明 ***********************
//* 文件：SceneSystem
//* 作者：wheat
//* 创建时间：2024/09/24 14:01:18 星期二
//* 描述：场景管理系统
//*******************************************************

using System;
using System.ComponentModel;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

#if ENABLE_ADDRESSABLES

using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

#endif

namespace KFrame.Systems
{
    public static class SceneSystem
    {
        #region 场景加载

        /// <summary>
        /// 同步切换场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="mode">切换模式</param>
        /// <param name="callback">切换后执行的方法</param>
        public static void LoadScene(string sceneName,
            LoadSceneMode mode = LoadSceneMode.Single, Action callback = null)
        {
            //加载场景
            #if ENABLE_ADDRESSABLES
            ResSystem.LoadScene(sceneName, mode);
            #else
            SceneManager.LoadScene(sceneName, mode);
            #endif

            //调用回调
            callback?.Invoke();
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        /// <param name="sceneBuildIndex">场景打包index</param>
        /// <param name="mode">切换模式</param>
        /// <param name="callback">切换后执行的方法</param>
        public static void LoadScene(int sceneBuildIndex,LoadSceneMode mode = LoadSceneMode.Single,
            Action callback = null)
        {
            //加载场景
            SceneManager.LoadScene(sceneBuildIndex, mode);
            //调用回调
            callback?.Invoke();
        }
        /// <summary>
        /// 异步切换场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="mode">切换模式</param>
        public static AsyncOperation LoadSceneAsync(string sceneName,LoadSceneMode mode = LoadSceneMode.Single)
        {
            return SceneManager.LoadSceneAsync(sceneName, mode);
        }

        #if ENABLE_ADDRESSABLES
        
        /// <summary>
        /// 从Addressables中异步切换场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="mode">切换模式</param>
        public static AsyncOperationHandle<SceneInstance> LoadSceneAsyncAdd(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            return ResSystem.LoadSceneAsync(sceneName, mode);
        }
        
        #endif

        /// <summary>
        /// 异步切换场景
        /// </summary>
        /// <param name="sceneBuildIndex">场景打包index</param>
        /// <param name="mode">切换模式</param>
        public static AsyncOperation LoadSceneAsync(int sceneBuildIndex,LoadSceneMode mode = LoadSceneMode.Single)
        {
            return SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
        }

        #endregion

        #region 卸载场景

        /// <summary>
        /// 同步卸载场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void UnloadScene(string sceneName)
        {
            SceneManager.UnloadScene(sceneName);
        }
        /// <summary>
        /// 异步卸载场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="unloadSceneOptions">切换模式</param>
        public static AsyncOperation UnLoadSceneAsync(string sceneName,
            UnloadSceneOptions unloadSceneOptions = UnloadSceneOptions.None)
        {
            return SceneManager.UnloadSceneAsync(sceneName, unloadSceneOptions);
        }

        #endregion


    }
}