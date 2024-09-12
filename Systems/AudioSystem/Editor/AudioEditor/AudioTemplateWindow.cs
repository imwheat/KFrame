//****************** 代码文件申明 ************************
//* 文件：AudioTemplateWindow                                       
//* 作者：wheat
//* 创建时间：2024/09/06 07:24:11 星期五
//* 描述：音效模版选择窗口
//*****************************************************

#if UNITY_EDITOR

using UnityEngine;
using System;
using System.Collections.Generic;
using KFrame;
using KFrame.Editor;
using KFrame.Systems;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace KFrame.Systems
{
    public class AudioTemplateWindow : EditorWindow
    {
        #region 参数引用
        
        /// <summary>
        /// 音效库
        /// </summary>
        private static AudioLibrary library => AudioLibrary.Instance;
        /// <summary>
        /// 模版选项
        /// </summary>
        private List<AudioEditData> templates => AudioEditorLibrary.Instance.AudioTemplates;

        #endregion

        #region 逻辑相关

        /// <summary>
        /// 选择某个模版后的回调
        /// </summary>
        private Action<AudioEditData> selectCallback;
        /// <summary>
        /// 保存音效库
        /// </summary>
        private bool saveLib;

        #endregion

        #region GUI绘制相关

        private Vector2 scrollPos;
        private string searchText;
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
            internal static readonly float btnWidth = 60f;
            internal static readonly float btnSpace= 10f;
            internal static readonly float labelHeight = 20f;
            
        }

        #endregion

        /// <summary>
        /// 显示音效模版选择界面
        /// </summary>
        /// <param name="callback">选择后的回调</param>
        public static void ShowWindow(Action<AudioEditData> callback)
        {
            AudioTemplateWindow window = EditorWindow.GetWindow<AudioTemplateWindow>();
            window.titleContent = new GUIContent("点击选择一个模版");
        }
        
        private void OnDisable()
        {
            //进行删除操作的话，在关闭的时候保存音效库
            if (saveLib)
            {
                library.SaveAsset();
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            searchText = SirenixEditorGUI.SearchField(EditorGUILayout.GetControlRect(
                GUILayout.Height(MStyle.labelHeight), GUILayout.ExpandWidth(true)), searchText);
            GUILayout.Space(MStyle.spacing);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            //遍历绘制每个模版
            for (int i = 0; i < templates.Count; i++)
            {
                string tName = templates[i].AudioName;

                //搜索过滤
                if (!string.IsNullOrEmpty(searchText) && !tName.Contains(searchText))
                {
                    continue;
                }
                
                EditorGUILayout.BeginHorizontal();
                
                //点击选择模版
                if (GUILayout.Button(tName))
                {
                    selectCallback?.Invoke(templates[i]);
                    Close();
                }
                
                //点击删除模版
                if (GUILayout.Button("删除"))
                {
                    saveLib = true;
                    templates.RemoveAt(i);
                    Repaint();
                }
                
                EditorGUILayout.EndHorizontal();
                
                GUILayout.Space(MStyle.spacing);

            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }
    }
}

#endif