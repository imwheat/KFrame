//****************** 代码文件申明 ***********************
//* 文件：MonoExtensions
//* 作者：wheat
//* 创建时间：2024/09/23 08:51:17 星期一
//* 描述：Mono的一些拓展工具
//*******************************************************

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using KFrame.Systems;

namespace GameBuild
{
    public static class MonoExtensions
    {
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

    }
}

