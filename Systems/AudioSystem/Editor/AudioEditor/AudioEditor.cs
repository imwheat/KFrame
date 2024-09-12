//****************** 代码文件申明 ************************
//* 文件：AudioEditor                                       
//* 作者：wheat
//* 创建时间：2024/09/01 12:10:25 星期日
//* 描述：音效编辑器窗口
//*****************************************************
using UnityEngine;
using KFrame.Systems;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using System.Threading.Tasks;
using KFrame;
using KFrame.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;


namespace KFrame.Systems
{
    public class AudioEditor : EditorWindow
    {
        #region 参数引用
        
        private static AudioLibrary _audioLibrary;
        /// <summary>
        /// 音效库
        /// </summary>
        public static AudioLibrary Library
        {
            get
            {
                if (_audioLibrary == null)
                {
                    _audioLibrary = AudioLibrary.Instance;
                }

                return _audioLibrary;
            }
        }

        #endregion

        #region GUI操作相关

        /// <summary>
        /// 正在播放的音效下标
        /// </summary>
        private int playingAudioIndex = -1;
        /// <summary>
        /// 搜索的音效名称
        /// </summary>
        private string searchText;
        /// <summary>
        /// 筛选类型的字典
        /// </summary>
        private Dictionary<AudioesType, bool> filterBoolDic = new Dictionary<AudioesType, bool>();
        /// <summary>
        /// 音效类型
        /// </summary>
        private AudioesType[] filterTypes;
        /// <summary>
        /// 列表的滚动位置
        /// </summary>
        private Vector2 listScrollPosition;

        #endregion

        #region GUI显示相关
        
        internal abstract class GUIBase
        {
            /// <summary>
            /// 显示的GUI内容
            /// </summary>
            public GUIContent GUIContent;
            /// <summary>
            /// 符合筛选文本
            /// </summary>
            public bool MatchFilterText = true;
            /// <summary>
            /// 符合筛选类型
            /// </summary>
            public bool MatchFilterType = true;
            /// <summary>
            /// 符合筛选结果
            /// </summary>
            public bool MatchFilter => MatchFilterText && MatchFilterType;
            /// <summary>
            /// 获取音效类型
            /// </summary>
            public abstract AudioesType GetSFXType();
            /// <summary>
            /// 获取名称
            /// </summary>
            public abstract string GetName();

        }
        
        /// <summary>
        /// 音效GUI的类
        /// </summary>
        internal class AudioGUI : GUIBase
        {
            public AudioStack Stack;

            public AudioGUI(AudioStack stack)
            {
                Stack = stack;
                GUIContent = new GUIContent(Stack.AudioName);
            }
            /// <summary>
            /// 获取音效类型
            /// </summary>
            public override AudioesType GetSFXType()
            {
                return Stack.AudioesType;
            }
            /// <summary>
            /// 获取名称
            /// </summary>
            public override string GetName()
            {
                return Stack.AudioName;
            }
        }
        /// <summary>
        /// BGMGUI的类
        /// </summary>
        internal class BGMGUI : GUIBase
        {
            public BGMStack Stack;

            public BGMGUI(BGMStack stack)
            {
                Stack = stack;
                GUIContent = new GUIContent(Stack.BGMName);
            }
            /// <summary>
            /// 获取音效类型
            /// </summary>
            public override AudioesType GetSFXType()
            {
                return AudioesType.BGM;
            }
            /// <summary>
            /// 获取名称
            /// </summary>
            public override string GetName()
            {
                return Stack.BGMName;
            }
        }
        
        /// <summary>
        /// GUI显示选项
        /// </summary>
        private List<GUIBase> guiOptions;
        
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
            internal static readonly float labelHeight = 15f;
            /// <summary>
            /// 筛选类型的宽度
            /// </summary>
            internal static readonly float filterTypeWidth = 80f;
        }

        /// <summary>
        /// 显示筛选选项
        /// </summary>
        private bool showFilter = true;

        #endregion
        
        #region 生命周期

        private void Awake()
        {
            AssemblyReloadEvents.afterAssemblyReload += InitWindow;

            //初始化
            InitWindow();
        }

        private void OnEnable()
        {
            //垃圾回收导致GUI没了，那就重新生成
            if (guiOptions == null)
            {
                InitGUI();
            }

        }

        private void OnDestroy()
        {
            AssemblyReloadEvents.afterAssemblyReload -= InitWindow;
        }

        #endregion

        #region 初始化相关
        
        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void ShowWindow()
        {
            AudioEditor window = EditorWindow.GetWindow<AudioEditor>();
            window.titleContent = new GUIContent("音效编辑器");
        }
        /// <summary>
        /// 初始化编辑器窗口
        /// </summary>
        private void InitWindow()
        {
            //初始化GUI
            InitGUI();
        }
        /// <summary>
        /// 初始化GUI
        /// </summary>
        private void InitGUI()
        {
            //参数初始化
            filterBoolDic = new Dictionary<AudioesType, bool>();
            filterTypes = (AudioesType[])Enum.GetValues(typeof(AudioesType));
            foreach (AudioesType type in filterTypes)
            {
                filterBoolDic[type] = true;
            }
            guiOptions = new List<GUIBase>();
            
            //多线程初始化GUI
            int midAudioIndex = Library.Audioes.Count / 2;
            Task[] tasks = new Task[]
            {
                Task.Run(() =>
                {
                    //初始化获取BGM
                    foreach (BGMStack bgmStack in Library.BGMs)
                    {
                        AddBGMStackGUI(bgmStack, false);
                    }
                }),
                Task.Run(() =>
                {
                    //初始化获取音效
                    for (int i = 0; i < midAudioIndex; i++)
                    {
                        AddAudioStackGUI(Library.Audioes[i], false);
                    }
                }),
                Task.Run(() =>
                {
                    //初始化获取音效
                    for (int i = midAudioIndex; i < Library.Audioes.Count; i++)
                    {
                        AddAudioStackGUI(Library.Audioes[i], false);
                    }
                }),
            };

            Task.WaitAll(tasks);

        }
        /// <summary>
        /// 添加BGM的选项GUI
        /// </summary>
        private void AddBGMStackGUI(BGMStack bgmStack, bool checkFilter)
        {
            BGMGUI gui = new BGMGUI(bgmStack);
            lock (guiOptions)
            {
                guiOptions.Add(gui);
            }
            
            if (checkFilter)
            {
                CheckFilterText(gui);
                CheckFilterType(gui);
            }
        }
        /// <summary>
        /// 添加音效的选项GUI
        /// </summary>
        private void AddAudioStackGUI(AudioStack audioStack, bool checkFilter)
        {
            AudioGUI gui = new AudioGUI(audioStack);
            lock (guiOptions)
            {
                guiOptions.Add(gui);
            }

            if (checkFilter)
            {
                CheckFilterText(gui);
                CheckFilterType(gui);
            }
        }

        #endregion

        #region GUI绘制
        
        /// <summary>
        /// GUI调用、绘制
        /// </summary>
        private void OnGUI()
        {
            
            //绘制顶部选项GUI
            DrawTopGUI();
            
            //绘制选项列表
            DrawOptionList();
            
        }
        /// <summary>
        /// 绘制单个筛选类型选项
        /// </summary>
        private void DrawTypeFilter(AudioesType type)
        {
            float toggleWdith = Mathf.Min(MStyle.filterTypeWidth / 3f, 20f);
            float labelWidth = MStyle.filterTypeWidth - toggleWdith - 5f;
            
            EditorGUILayout.LabelField(type.ToString(),
                GUILayout.Width(labelWidth), GUILayout.Height(MStyle.labelHeight));
            
            GUILayout.Space(5f);
            
            filterBoolDic[type] = EditorGUILayout.Toggle(filterBoolDic[type],
                GUILayout.Width(toggleWdith), GUILayout.Height(MStyle.labelHeight));

        }
        /// <summary>
        /// 检测名称是否符合筛选结果
        /// </summary>
        private void CheckFilterText(GUIBase gui)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                gui.MatchFilterText = true;
            }
            else
            {
                gui.MatchFilterText = gui.GetName().Contains(searchText);
            }
        }
        /// <summary>
        /// 检测类型是否符合筛选结果
        /// </summary>
        private void CheckFilterType(GUIBase gui)
        {
            gui.MatchFilterType = filterBoolDic[gui.GetSFXType()];
        }
        /// <summary>
        /// 绘制顶部选项GUI
        /// </summary>
        private void DrawTopGUI()
        {
            EditorGUILayout.BeginVertical();
            
            //搜索栏
            EditorGUI.BeginChangeCheck();
            Rect serachRect = EditorGUILayout.GetControlRect(GUILayout.Height(MStyle.labelHeight));
            searchText = SirenixEditorGUI.SearchField(serachRect, searchText);
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < guiOptions.Count; i++)
                {
                    //检测名称是否符合筛选结果
                    CheckFilterText(guiOptions[i]);
                }
            }
            
            //筛选选项            
            EditorGUITool.ShowAClearFoldOut(ref showFilter, "筛选");
            EditorGUI.BeginChangeCheck();
            if (showFilter)
            {
                if (position.width <= MStyle.filterTypeWidth)
                {
                    foreach (AudioesType type in filterTypes)
                    {
                        DrawTypeFilter(type);
                    }
                }
                else
                {
                    float x = 0f;
                    EditorGUILayout.BeginHorizontal();
                    foreach (AudioesType type in filterTypes)
                    {
                        x += MStyle.filterTypeWidth;
                        if (x >= position.width)
                        {
                            x = MStyle.filterTypeWidth;
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                        }
                        DrawTypeFilter(type);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.EndHorizontal();
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < guiOptions.Count; i++)
                {
                    //检测类型是否符合筛选结果
                    CheckFilterType(guiOptions[i]);
                }
            }
            
            //创建新的音效资源
            GUILayout.Space(MStyle.spacing);
            if (GUILayout.Button("创建新的音效资源", GUILayout.Height(MStyle.labelHeight)))
            {
                AudioStackCreateEditor.ShowWindow();
            }
            GUILayout.Space(MStyle.spacing);
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制选项列表
        /// </summary>
        private void DrawOptionList()
        {
            
            
            EditorGUILayout.BeginVertical();

            listScrollPosition = EditorGUILayout.BeginScrollView(listScrollPosition);
            
            //遍历绘制每个GUI
            for (int i = 0; i < guiOptions.Count; i++)
            {
                //绘制GUI
                DrawGUIOption(guiOptions[i], i);
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// 绘制单个GUI选项
        /// </summary>
        /// <param name="i">下标</param>
        private void DrawGUIOption(GUIBase option, int i)
        {
            //如果不符合筛选结果那就不进行绘制
            if (!option.MatchFilter)
            {
                return;
            }
            
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField(option.GUIContent);

            GUILayout.FlexibleSpace();

            //显示播放/暂停按钮
            if(playingAudioIndex == i)
            {
                //暂停播放
                if (GUILayout.Button(EditorIcons.Stop.Raw, GUILayout.Height(MStyle.labelHeight), GUILayout.Width(MStyle.btnWidth)))
                {
                    EditorStopPlayAudio();
                }
            }
            else
            {
                //开始播放
                if (GUILayout.Button(EditorIcons.Play.Raw, GUILayout.Height(MStyle.labelHeight), GUILayout.Width(MStyle.btnWidth)))
                {
                    //更新正在播放的下标
                    playingAudioIndex = i;
                    
                    //获取播放的clip
                    AudioClip clip = null;
                    
                    switch (option)
                    {
                        case BGMGUI bgm:
                            BGMStack bgmStack = bgm.Stack;

                            if(bgmStack != null)
                            {
                                BGMClipStack clipStack = bgmStack.Clips[UnityEngine.Random.Range(0, bgmStack.Clips.Count)];
                                clip = clipStack.Clips[UnityEngine.Random.Range(0, clipStack.Clips.Count)];
                            }
                            break;
                        case AudioGUI audio:
                            AudioStack audioStack = audio.Stack;

                            if (audioStack != null)
                            {
                                clip = audioStack.GetRandomAudio();
                            }
                            break;
                    }
                    
                    //播放音效
                    EditorPlayAudio(clip);
                }
            }
            
            //点击打开编辑
            if(GUILayout.Button("编辑",GUILayout.Height(MStyle.labelHeight), GUILayout.Width(MStyle.btnWidth)))
            {
                
                switch (option)
                {
                    case BGMGUI bgm:
                        break;
                    case AudioGUI audio:
                        AudioStackEditor.ShowWindow(audio.Stack);
                        break;
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// 播放音效
        /// </summary>
        private void EditorPlayAudio(AudioClip clip)
        {
            //如果为空就返回
            if (clip == null) return;

            //播放音效
            EditorGUITool.EditorPlayAudio(clip);
        }
        /// <summary>
        /// 停止播放
        /// </summary>
        private void EditorStopPlayAudio()
        {
            //停止播放，然后重置下标
            EditorGUITool.EditorStopPlayAudio();
            playingAudioIndex = -1;
        }

        #endregion

    }
    
}

#endif

