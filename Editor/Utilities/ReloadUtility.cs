//****************** 代码文件申明 ***********************
//* 文件：ReloadUtility
//* 作者：wheat
//* 创建时间：2024/05/27 10:51:32 星期一
//* 描述：在编辑器重新加载的时候调用一些事件
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame;
using KFrame.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KFrame.Editor
{
    /// <summary>
    /// 在编辑器重新加载的时候调用一些事件
    /// </summary>
    public static class ReloadUtility
    {
        /// <summary>
        /// 待清理的UnityObject
        /// </summary>
        private static List<UnityEngine.Object> toCleanUpUnityObjects;
        /// <summary>
        /// 待清理的Disposable
        /// </summary>
        private static List<WeakReference> toCleanUpDisposables;
        static ReloadUtility()
        {
            toCleanUpUnityObjects = new List<UnityEngine.Object>();
            toCleanUpDisposables = new List<WeakReference>();
            AssemblyReloadEvents.beforeAssemblyReload += CleanUp;

        }
        /// <summary>
        /// 添加在Reload的时候Destroy的对象
        /// </summary>
        public static void DestroyObjectOnAssemblyReload(UnityEngine.Object unityObj)
        {
            if (!(unityObj == null))
            {
                toCleanUpUnityObjects.Add(unityObj);
            }
        }
        /// <summary>
        /// 添加在Reload的时候Dispose的对象
        /// </summary>
        public static void DisposeObjectOnAssemblyReload(IDisposable disposable)
        {
            if (disposable != null)
            {
                toCleanUpDisposables.Add(new WeakReference(disposable));
            }
        }
        /// <summary>
        /// 清理
        /// </summary>
        private static void CleanUp()
        {
            foreach (UnityEngine.Object toCleanUpUnityObject in toCleanUpUnityObjects)
            {
                try
                {
                    if (toCleanUpUnityObject != null)
                    {
                        UnityEngine.Object.DestroyImmediate(toCleanUpUnityObject);
                    }
                }
                catch
                {
                }
            }

            foreach (WeakReference toCleanUpDisposable in toCleanUpDisposables)
            {
                try
                {
                    (toCleanUpDisposable.Target as IDisposable)?.Dispose();
                }
                catch
                {
                }
            }

            toCleanUpUnityObjects.Clear();
            toCleanUpDisposables.Clear();
        }

    }
}

