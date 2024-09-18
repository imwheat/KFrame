//****************** 代码文件声明 ***********************
//* 文件：TMP_ListPool
//* 作者：wheat
//* 创建时间：2024/09/18 13:20:00 星期三
//* 描述：TMP会用到的ListPool
//*******************************************************
using System;
using System.Collections.Generic;
using KFrame;
using KFrame.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KFrame.UI
{
    internal static class TMP_ListPool<T>
    {      
        // Object pool to avoid allocations.
        private static readonly TMP_ObjectPool<List<T>> s_ListPool = new TMP_ObjectPool<List<T>>(null, l => l.Clear());

        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }
}
