//****************** 代码文件申明 ***********************
//* 文件：AudioEditor
//* 作者：wheat
//* 创建时间：2024/04/27 15:59:59 星期六
//* 描述：音效编辑器
//*******************************************************

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using KFrame.Systems;
using System;

namespace KFrame.Editor
{
    public partial class AudioEditor : KEditorWindow
    {
        /// <summary>
        /// 音效库
        /// </summary>
        public static AudioLibrary AudioLibrary => AudioLibrary.Instance;

        #region GUI

        /// <summary>
        /// 编辑模式
        /// 0.音效编辑
        /// 1.BGM编辑
        /// 2.Clip编辑
        /// 3.分组编辑
        /// </summary>
        private int editMode = 0;
        private List<GUIContent> audioGUI;

        private Vector2 scrollPositionAudio;
        private Vector2 scrollPositionBGM;
        private Vector2 scrollPositionClip;

        private SerializedObject serializedObject;
        private List<SerializedProperty> clipList;

        #endregion

        #region 初始化

        /// <summary>
        /// 打开面板
        /// </summary>
        public static AudioEditor ShowWindow()
        {
            AudioEditor window = GetWindow<AudioEditor>();
            window.titleContent = new GUIContent("音效编辑器");
            window.Show();

            return window;
        }
        private void OnEnable()
        {
            Init();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            audioGUI = new List<GUIContent>();

            for (int i = 0; i < AudioLibrary.Audioes.Count; i++)
            {
                audioGUI.Add(new GUIContent(AudioLibrary.Audioes[i].AudioName));
            }

            serializedObject = new SerializedObject(AudioLibrary);
            clipList = new List<SerializedProperty>() { KEditorGUI.GetSerializedProperty(serializedObject, nameof(AudioLibrary.AudioClips)) };

            searchGroupText = "";
        }
        #endregion
        /// <summary>
        /// 保存音效库
        /// </summary>
        public static void SaveAudioLibrary()
        {
            KEditorUtility.SaveAsset(AudioLibrary);
        }

        #region 绘制GUI

        protected override void OnGUI()
        {
            if(AudioLibrary == null)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.LabelField("音效库为空");

                EditorGUILayout.EndVertical();
            }
            else
            {

                EditorGUILayout.BeginVertical();

                DrawTopGUI();

                EditorGUILayout.EndVertical();

                //普通GUI
                if (editMode < 3)
                {
                    DrawCommonGUI();
                }
                //编辑分组GUI
                else if (editMode == 3)
                {
                    DrawEditGroupGUI();
                }

            }

            base.OnGUI();
        }
        /// <summary>
        /// 绘制顶部GUI
        /// </summary>
        private void DrawTopGUI()
        {
            EditorGUILayout.BeginHorizontal();

            if(GUILayout.Button("音效模式"))
            {
                editMode = 0;
            }

            if ((GUILayout.Button("BGM编辑")))
            {
                editMode = 1;
            }

            if ((GUILayout.Button("Clip编辑")))
            {
                editMode = 2;
            }

            if ((GUILayout.Button("分组编辑")))
            {
                editMode = 3;
            }

            EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 绘制普通模式GUI
        /// </summary>
        private void DrawCommonGUI()
        {
            EditorGUILayout.BeginVertical();

            switch (editMode)
            {
                case 0:
                    //绘制音效列表GUI
                    DrawAudioListGUI();
                    break;
                case 1:
                    //绘制BGM列表GUI
                    DrawBGMListGUI();
                    break;
                case 2:
                    //绘制Clip列表GUI
                    DrawClipListGUI();
                    break;
            }

            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制音效列表GUI
        /// </summary>
        private void DrawAudioListGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("音效库");

            scrollPositionAudio = EditorGUILayout.BeginScrollView(scrollPositionAudio);

            for (int i = 0; i < audioGUI.Count; i++)
            {
                if (GUILayout.Button(audioGUI[i]))
                {
                    AudioStackEditor.ShowWindow(AudioLibrary.Audioes[i]);
                }
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

        }
        /// <summary>
        /// 绘制BGM列表GUI
        /// </summary>
        private void DrawBGMListGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("BGM库");

            scrollPositionBGM = EditorGUILayout.BeginScrollView(scrollPositionBGM);

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制Clip列表GUI
        /// </summary>
        private void DrawClipListGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Clip库");

            scrollPositionClip = EditorGUILayout.BeginScrollView(scrollPositionClip);

            KEditor.DrawProperties(serializedObject, clipList);

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }


        #endregion
    }

    #region GroupGUI
    public partial class AudioEditor
    {
        #region GUI字段

        /// <summary>
        /// 滚轮位置
        /// </summary>
        private Vector2 scrollPositionGroup;
        /// <summary>
        /// 搜索group
        /// </summary>
        private string searchGroupText;
        /// <summary>
        /// 创建新的Group
        /// </summary>
        private bool createNewGroup;
        /// <summary>
        /// 新的Group名称
        /// </summary>
        private string newGroupName;
        /// <summary>
        /// Group的GUI
        /// </summary>
        private List<GUIContent> groupGUI;

        #endregion

        /// <summary>
        /// 绘制编辑分组GUI
        /// </summary>

        private void DrawEditGroupGUI()
        {
            //如果为空的话那就初始化刷新
            if(groupGUI == null)
            {
                InitGroupGUI();
            }


            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            if(createNewGroup == false)
            {
                if (GUILayout.Button("新建Group", GUILayout.Height(16f)))
                {
                    createNewGroup = true;
                    newGroupName = "";
                }
            }
            else
            {
                newGroupName = EditorGUILayout.TextField(newGroupName, GUILayout.Height(16f));

                if (GUILayout.Button("新建", GUILayout.Height(16f)))
                {
                    //新建添加Group
                    AudioGroup newGroup = new AudioGroup(newGroupName, GetGroupViableIndex());
                    AudioLibrary.AudioGroups.Add(newGroup);
                    AddGroupGUI(newGroup.GroupName);
                    SaveAudioLibrary();

                    createNewGroup = false;
                    newGroupName = "";
                }

                if (GUILayout.Button("取消", GUILayout.Height(16f)))
                {
                    createNewGroup = false;
                    newGroupName = "";
                }
            }


            //更新搜索栏
            Rect searchRect = GUILayoutUtility.GetRect(0, 16f, GUILayout.ExpandWidth(true));
            string newSearchText = KEditorGUI.SearchTextField(searchRect, searchGroupText);
            if(newSearchText != searchGroupText)
            {
                searchGroupText = newSearchText;
                Repaint();
            }
            

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Group库");

            scrollPositionGroup = EditorGUILayout.BeginScrollView(scrollPositionGroup);

            //如果数量为0
            if(groupGUI.Count == 0)
            {
                EditorGUILayout.LabelField("目前还没有Group");
            }
            //数量不为0
            else
            {
                //遍历绘制每个Group选项
                for (int i = 0; i < groupGUI.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button(groupGUI[i]))
                    {

                    }

                    if (GUILayout.Button("删除", GUILayout.Width(64f)))
                    {

                    }

                    EditorGUILayout.EndHorizontal();
                }

            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 初始化GroupGUI
        /// </summary>
        private void InitGroupGUI()
        {
            groupGUI = new List<GUIContent>();
            //遍历添加GUI选项
            foreach (var group in AudioLibrary.AudioGroups)
            {
                AddGroupGUI(group.GroupName);
            }
        }
        /// <summary>
        /// 添加GroupGUI
        /// </summary>
        private void AddGroupGUI(string label)
        {
            groupGUI.Add(new GUIContent(label));
        }
        /// <summary>
        /// 获取一个未被使用的有效的Group下标
        /// </summary>
        /// <returns></returns>
        internal int GetGroupViableIndex()
        {
            //寻找最大id
            int max = -1;

            //遍历Group寻找最大的id
            foreach (var group in AudioLibrary.AudioGroups)
            {
                max = Math.Max(group.GroupIndex, max);
            }

            //返回比最大的id还要大1的下标
            return max + 1;
        }
    }

    #endregion
}

