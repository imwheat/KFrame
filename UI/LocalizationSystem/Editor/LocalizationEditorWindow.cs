//****************** 代码文件申明 ***********************
//* 文件：LocalizationEditorWindow
//* 作者：wheat
//* 创建时间：2024/09/21 16:04:39 星期六
//* 描述：浏览、编辑本地化配置的编辑器
//*******************************************************

using UnityEngine;
using KFrame;
using KFrame.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using KFrame.Editor;
using UnityEditor;

namespace KFrame.UI
{
    public class LocalizationEditorWindow : KEditorWindow
    {

        #region 数据引用
        
        /// <summary>
        /// 本地化配置的数据
        /// </summary>
        private LocalizationConfig config => LocalizationConfig.Instance;

        #endregion
        
        #region GUI绘制相关
        
        /// <summary>
        /// 选择绘制类型
        /// </summary>
        private enum SelectType
        {
            StringData = 0,
            ImageData = 1,
        }
        
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
            /// <summary>
            /// 筛选类型的宽度
            /// </summary>
            internal static readonly float filterTypeWidth = 80f;
        }
        /// <summary>
        /// 当前选择的绘制类型
        /// </summary>
        private SelectType curSelectType;
        /// <summary>
        /// 当前语言种类
        /// </summary>
        private LanguageType[] languageTypes;
        /// <summary>
        /// 语言筛选
        /// </summary>
        private Dictionary<LanguageType, bool> languageFilter;
        
        #endregion

        #region 生命周期

        private void Awake()
        {
            Init();
            AssemblyReloadEvents.afterAssemblyReload += Init;
        }

        private void OnDestroy()
        {
            AssemblyReloadEvents.afterAssemblyReload -= Init;
        }

        #endregion
        
        #region 初始化

        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void ShowWindow()
        {
            LocalizationEditorWindow window = EditorWindow.GetWindow<LocalizationEditorWindow>();
            window.titleContent = new GUIContent("本地化编辑器");
        }

        private void Init()
        {
            
        }
        
        #endregion

        #region 绘制GUI

        protected override void OnGUI()
        {
            
            
            base.OnGUI();
        }

        #endregion
        
    }
}

