#if UNITY_EDITOR

//****************** 代码文件申明 ***********************
//* 文件：FrameClass
//* 作者：wheat
//* 创建时间：2024/09/23 09:16:17 星期一
//* 描述：只能在编辑器里使用，辅助的一些编辑器功能
//*******************************************************

using UnityEngine;
using KFrame;
using KFrame.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace KFrame
{
    /// <summary>
    /// 只能在编辑器里使用
    /// </summary>
    public class KEditorUtility
    {
        /// <summary>
        /// 增加编辑器宏
        /// </summary>
        public static void AddScriptCompilationSymbol(string name)
        {
            BuildTargetGroup buildTargetGroup = UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup;
            string group = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (!group.Contains(name))
            {
                UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, group + ";" + name);
            }
        }
        /// <summary>
        /// 移除编辑器宏
        /// </summary>
        public static void RemoveScriptCompilationSymbol(string name)
        {
            BuildTargetGroup buildTargetGroup = UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup;
            string group = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (group.Contains(name))
            {
                UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup,
                    group.Replace(";" + name, string.Empty));
            }
        }
    }
}


#endif
