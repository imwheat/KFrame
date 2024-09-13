//****************** 代码文件申明 ***********************
//* 文件：BGMEditor
//* 作者：wheat
//* 创建时间：2024/09/13 11:19:48 星期五
//* 描述：
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame;
using KFrame.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using KFrame.Editor;

namespace KFrame.Systems
{
    public class BGMEditor : KEditorWindow
    {
        #region GUI相关

        /// <summary>
        /// 标准化统一当前编辑器的绘制Style
        /// </summary>
        private static class MStyle
        {
#if UNITY_2018_3_OR_NEWER
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
#else
            internal static readonly float spacing = 2.0f;
#endif
            /// <summary>
            /// 按钮宽度
            /// </summary>
            internal static readonly float btnWidth = 40f;
            /// <summary>
            /// Label高度
            /// </summary>
            internal static readonly float labelHeight = 20f;
        }

        #endregion
        
    }
}

