//****************** 代码文件申明 ***********************
//* 文件：AudioStackEditor
//* 作者：wheat
//* 创建时间：2024/04/28 19:11:44 星期日
//* 描述：
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame;
using KFrame.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using KFrame.Systems;

namespace KFrame.Editor
{
    public class AudioStackEditor : KEditorWindow
    {
        #region 资源

        [SerializeField]
        private AudioStack curAudio;
        private SerializedPropertyPack audioProperties;
        private SerializedObject serializedObject;

        #endregion

        #region 编辑和GUI

        /// <summary>
        /// 编辑模式
        /// 0.普通编辑模式
        /// 1.新建模式
        /// </summary>
        private int editMode;
        private Vector2 scrollPosition;

        #endregion

        #region 初始化

        public static AudioStackEditor ShowWindow(AudioStack audioStack)
        {
            AudioStackEditor window = GetWindow<AudioStackEditor>();
            window.titleContent = new GUIContent("音效编辑器");
            window.Show();
            window.curAudio = audioStack;
            window.Init();

            return window;
        }
        private void Init()
        {
            //如果当前的AudioStack为空
            if(curAudio == null)
            {
                //那就是新建模式
                editMode = 1;
                curAudio = new AudioStack();
            }
            else
            {
                //普通编辑模式
                editMode = 0;
            }
            
            serializedObject = new SerializedObject(this);
            audioProperties = KEditorGUI.GetSPPackIncludeChildren(serializedObject, nameof(curAudio));
        }

        #endregion
        protected override void OnGUI()
        {

            EditorGUILayout.BeginVertical();

            DrawTopGUI();

            GUILayout.Space(5);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            KEditor.DrawProperties(serializedObject, audioProperties);

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            DrawBottomGUI();

            GUILayout.Space(5);

            EditorGUILayout.EndVertical();

            base.OnGUI();
        }
        /// <summary>
        /// 绘制顶部GUI
        /// </summary>
        private void DrawTopGUI()
        {

        }
        /// <summary>
        /// 绘制底部GUI
        /// </summary>
        private void DrawBottomGUI()
        {
            //编辑模式
            if(editMode == 0)
            {
                if(GUILayout.Button("保存"))
                {
                    AudioEditor.SaveAudioLibrary();
                }
            }
            //新建模式
            else if (editMode == 1)
            {

            }
        }
    }
}

