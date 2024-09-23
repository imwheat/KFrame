using System;
using System.Collections.Generic;
using UnityEngine;
using KFrame.Utilities;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KFrame
{
	/// <summary>
	/// 通用工具集
	/// <para>这里希望放一些通用的，常用的方法</para>
	/// </summary>
	public static class KUtility
    {
        public static void OpenLocalPath()
        {
            FileExtensions.OpenLocalPath();
        }

        public static void SetCopyBuffer(string context)
        {
            GUIUtility.systemCopyBuffer = context;
        }

#if UNITY_EDITOR


        #region Editor工具

        private static Dictionary<string, Action<object>> myMessages = new Dictionary<string, Action<object>>();

        public static void RegisterMgs(string mgsName,Action<object> mgs)
        {
            myMessages.Add(mgsName,mgs);
        }

        public static void UnRegisterMgs(string mgsName)
        {
            myMessages.Remove(mgsName);
        }

        public static void SendMgs(string mgsName,object data)
        {
            myMessages[mgsName](data);
        }
        
        /// <summary>
        /// 调用MenuItem命令(有1个默认参数，默认为false)
        /// </summary>
        /// <param name="menuName">Koo框架内的有效命令</param>
        /// <param name="isCommon">是否限定于KFramework</param>
        public static void CallMenuItem(string menuName, bool isCommon = false)
        {
            if (isCommon)
            {
                EditorApplication.ExecuteMenuItem(menuName);
            }

            EditorApplication.ExecuteMenuItem("KFramework/" + menuName);
        }

        #endregion

#endif

    }
}  