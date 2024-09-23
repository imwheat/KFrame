//****************** 代码文件声明 ***********************
//* 文件：UISetPropertyUtility
//* 作者：wheat
//* 创建时间：2024/09/15 19:00:22 星期日
//* 描述：提供给UI使用的配置属性
//*******************************************************
using System;
using System.Collections.Generic;
using KFrame;
using KFrame.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KFrame.UI
{
    /// <summary>
    /// 提供给UI使用的配置属性
    /// </summary>
    internal static class UISetPropertyUtility
    {
        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="currentValue">当前值</param>
        /// <param name="newValue">新的值</param>
        /// <returns>设置成功返回true</returns>
        public static bool SetColor(ref Color currentValue, Color newValue)
        {
            if (Mathf.Approximately(currentValue.r, newValue.r) && Mathf.Approximately(currentValue.g, newValue.g) 
                                                                && Mathf.Approximately(currentValue.b, newValue.b)&& Mathf.Approximately(currentValue.a, newValue.a))
                return false;

            currentValue = newValue;
            return true;
        }
        /// <summary>
        /// 给Struct赋值
        /// </summary>
        /// <param name="currentValue">当前值</param>
        /// <param name="newValue">新的值</param>
        /// <returns>设置成功返回true</returns>
        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;

            currentValue = newValue;
            return true;
        }
        /// <summary>
        /// 给Class赋值
        /// </summary>
        /// <param name="currentValue">当前值</param>
        /// <param name="newValue">新的值</param>
        /// <returns>设置成功返回true</returns>
        public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false;

            currentValue = newValue;
            return true;
        }
    }
}
