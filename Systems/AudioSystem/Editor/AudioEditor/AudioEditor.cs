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

        private static List<AudioClip> audioClips => Library.AudioClips;
        private static List<AudioClip> bgmClips => Library.BGMClips;

        #endregion

        #region GUI操作相关

        /// <summary>
        /// 0默认
        /// 1选择模式
        /// </summary>
        private int mode;
        /// <summary>
        /// 选择回调
        /// </summary>
        private Action<int> selectCallback;
        /// <summary>
        /// 正在播放的音效下标
        /// </summary>
        private int playingAudioIndex = -1;
        /// <summary>
        /// 搜索的音效名称
        /// </summary>
        private string audioSearchText;
        /// <summary>
        /// 搜索的BGM名称
        /// </summary>
        private string bgmSearchText;
        /// <summary>
        /// 搜索的音效Clip名称
        /// </summary>
        private string audioClipSearchText;
        /// <summary>
        /// 搜索的BGMClip名称
        /// </summary>
        private string bgmClipSearchText;
        /// <summary>
        /// 筛选类型的字典
        /// </summary>
        private Dictionary<AudioesType, bool> filterBoolDic = new Dictionary<AudioesType, bool>();
        /// <summary>
        /// 音效类型
        /// </summary>
        private AudioesType[] filterTypes;
        /// <summary>
        /// Audio列表的滚动位置
        /// </summary>
        private Vector2 audioScrollPosition;
        /// <summary>
        /// BGM列表的滚动位置
        /// </summary>
        private Vector2 bgmScrollPosition;
        /// <summary>
        /// AudioClip列表的滚动位置
        /// </summary>
        private Vector2 audioClipScrollPosition;
        /// <summary>
        /// BGMClip列表的滚动位置
        /// </summary>
        private Vector2 bgmClipScrollPosition;
        /// <summary>
        /// Group列表的滚动位置
        /// </summary>
        private Vector2 groupScrollPosition;

        #endregion

        #region GUI显示相关
        
        
        /// <summary>
        /// 编辑模式
        /// 0.音效编辑
        /// 1.BGM编辑
        /// 2.AudioClip编辑
        /// 3.BGMClip编辑
        /// 4.分组编辑
        /// </summary>
        private enum EditMode
        {
            SFX = 0,
            BGM = 1,
            AudioClip = 2,
            BGMClip = 3,
            Group = 4,
        }
        
        /// <summary>
        /// 编辑模式
        /// 0.音效编辑
        /// 1.BGM编辑
        /// 2.AudioClip编辑
        /// 3.BGMClip编辑
        /// 4.分组编辑
        /// </summary>
        private EditMode editMode = EditMode.SFX;
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
            public AudioesType GetSFXType()
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
            /// 获取名称
            /// </summary>
            public override string GetName()
            {
                return Stack.BGMName;
            }
        }
        
        /// <summary>
        /// 音效GUI选项
        /// </summary>
        private List<AudioGUI> audioGUIs;
        /// <summary>
        /// BGMGUI选项
        /// </summary>
        private List<BGMGUI> bgmGUIs;
        
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
            //初始化
            InitWindow();
        }

        private void OnEnable()
        {
            //垃圾回收导致GUI没了，那就重新生成
            if (audioGUIs == null)
            {
                InitGUI();
            }

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
            window.mode = 0;
            window.selectCallback = null;
        }

        public static void ShowAsSelector(Action<int> callback)
        {
            AudioEditor window = EditorWindow.GetWindow<AudioEditor>();
            window.titleContent = new GUIContent("音效编辑器");
            window.mode = 1;
            window.selectCallback = callback;
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
            audioGUIs = new List<AudioGUI>();
            bgmGUIs = new List<BGMGUI>();
            
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
            lock (bgmGUIs)
            {
                bgmGUIs.Add(gui);
            }
            
            if (checkFilter)
            {
                CheckFilterText(gui, audioSearchText);
            }
        }
        /// <summary>
        /// 添加音效的选项GUI
        /// </summary>
        private void AddAudioStackGUI(AudioStack audioStack, bool checkFilter)
        {
            AudioGUI gui = new AudioGUI(audioStack);
            lock (audioGUIs)
            {
                audioGUIs.Add(gui);
            }

            if (checkFilter)
            {
                CheckFilterText(gui, bgmSearchText);
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

            switch (editMode)
            {
                case EditMode.SFX:
                    //绘制音效选项列表
                    DrawAudioGUI();
                    break;
                case EditMode.BGM:
                    //绘制BGM列表
                    DrawBGMGUI();
                    break;
                case EditMode.AudioClip:
                    //绘制AudioClipGUI
                    DrawAudioClipGUI();
                    break;
                case EditMode.BGMClip:
                    //绘制BGMClipGUI
                    DrawBGMClipGUI();
                    break;
                case EditMode.Group:
                    //绘制Group列表
                    DrawGroupGUI();
                    break;
            }

            
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
        private void CheckFilterText(GUIBase gui, string searchText)
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
        private void CheckFilterType(AudioGUI gui)
        {
            gui.MatchFilterType = filterBoolDic[gui.GetSFXType()];
        }
        /// <summary>
        /// 切换编辑模式
        /// </summary>
        /// <param name="mode"></param>
        private void SwitchEditMode(EditMode mode)
        {
            //切换模式
            editMode = mode;
            
            //如果正在播放音效
            if (playingAudioIndex != -1)
            {
                //那就停止播放音效
                EditorStopPlayAudio();
            }
        }
        /// <summary>
        /// 绘制顶部选项GUI
        /// </summary>
        private void DrawTopGUI()
        {
            EditorGUILayout.BeginVertical();
            
            GUILayout.Space(10f);
            
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("音效"))
            {
                SwitchEditMode(EditMode.SFX);
            }
            if (GUILayout.Button("BGM"))
            {
                SwitchEditMode(EditMode.BGM);
            }
            if (GUILayout.Button("AudioClip"))
            {
                SwitchEditMode(EditMode.AudioClip);
            }
            if (GUILayout.Button("BGMClip"))
            {
                SwitchEditMode(EditMode.BGMClip);
            }
            if (GUILayout.Button("Group"))
            {
                SwitchEditMode(EditMode.Group);
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制搜索栏
        /// </summary>
        /// <param name="searchText">搜索文本</param>
        /// <returns>如果搜索文本发生变化返回true</returns>
        private bool DrawSearchField(ref string searchText)
        {
            GUILayout.Space(5f);
            //搜索栏
            EditorGUI.BeginChangeCheck();
            Rect serachRect = EditorGUILayout.GetControlRect(GUILayout.Height(MStyle.labelHeight));
            searchText = KEditorGUI.SearchTextField(serachRect, searchText);

            GUILayout.Space(5f);
            
            return EditorGUI.EndChangeCheck();
        }
        /// <summary>
        /// 绘制选项列表
        /// </summary>
        private void DrawAudioGUI()
        {
            
            EditorGUILayout.BeginVertical();
            
            //绘制搜索栏
            if (DrawSearchField(ref audioSearchText))
            {            
                //检测名称是否符合筛选结果
                for (int i = 0; i < audioGUIs.Count; i++)
                {
                    CheckFilterText(audioGUIs[i], audioSearchText);
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
                for (int i = 0; i < audioGUIs.Count; i++)
                {
                    //检测类型是否符合筛选结果
                    CheckFilterType(audioGUIs[i]);
                }
                
            }
            
            GUILayout.Space(10f);

            if (mode == 0)
            {
                //创建新的音效资源
                GUILayout.Space(MStyle.spacing);
                if (GUILayout.Button("创建新的音效资源", GUILayout.Height(MStyle.labelHeight)))
                {
                    AudioStackCreateEditor.ShowWindow();
                }
            
                GUILayout.Space(10f);
            }

            
            audioScrollPosition = EditorGUILayout.BeginScrollView(audioScrollPosition);

            //遍历绘制每个GUI
            for (int i = 0; i < audioGUIs.Count; i++)
            {
                //绘制GUI
                DrawGUIOption(audioGUIs[i], i);
            }

            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制BGM列表
        /// </summary>
        private void DrawBGMGUI()
        {
            
            EditorGUILayout.BeginVertical();
            
            //绘制搜索栏
            if (DrawSearchField(ref bgmSearchText))
            {            
                //检测名称是否符合筛选结果
                for (int i = 0; i < bgmGUIs.Count; i++)
                {
                    CheckFilterText(bgmGUIs[i], bgmSearchText);
                }
            }
            
            GUILayout.Space(10f);
            
            //创建新的音效资源
            GUILayout.Space(MStyle.spacing);
            if (GUILayout.Button("创建新的BGM资源", GUILayout.Height(MStyle.labelHeight)))
            {
                BGMEditor.ShowWindowCreateNewStack();
            }
            
            GUILayout.Space(10f);
            
            bgmScrollPosition = EditorGUILayout.BeginScrollView(bgmScrollPosition);

            //遍历绘制每个GUI
            for (int i = 0; i < bgmGUIs.Count; i++)
            {
                //绘制GUI
                DrawGUIOption(bgmGUIs[i], i);
            }

            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制拖拽放入的BOX
        /// </summary>
        private void DrawDragAudioClipBox(List<AudioClip> editClips)
        {
            EditorGUILayout.BeginVertical();
	        
            //获取当前事件
            Event currentEvent = Event.current;
                        
            //画出一个拖拽区域
            Rect dropArea = GUILayoutUtility.GetRect(0f, 50f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "把Clip丢进这里批量添加");
                        
            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    //如果鼠标不在拖拽区域就返回
                    if (!dropArea.Contains(currentEvent.mousePosition))
                        break;
                        
                    //把鼠标的显示改为Copy的样子
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        
                    //如果放下了鼠标
                    if (currentEvent.type == EventType.DragPerform)
                    {
                        //那就完成拖拽
                        DragAndDrop.AcceptDrag();
                        
                        //然后遍历选中的物品
                        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                        {
                            //如果是AudioClip
                            if (draggedObject is AudioClip)
                            {
                                //那就获取添加到列表里面
                                AudioClip clip = (AudioClip)draggedObject;
                                editClips.Add(clip);
								
                            }
                        }
                    }
                    Event.current.Use();
                    break;
            }
	        
            EditorGUILayout.EndVertical();
	        
        }
        /// <summary>
        /// 绘制AudioCLip的GUI
        /// </summary>
        private void DrawAudioClipGUI()
        {
            EditorGUILayout.BeginVertical();
            
            EditorGUITool.BoldLabelField("AudioClip列表");
            GUILayout.Space(10f);

            audioClipScrollPosition = EditorGUILayout.BeginScrollView(audioClipScrollPosition);
            
            //遍历进行绘制
            for (int i = 0; i < audioClips.Count; i++)
            {
                AudioClip clip = audioClips[i];
                if (clip == null) continue;

                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(36f), GUILayout.Height(MStyle.labelHeight));
                
                EditorGUILayout.LabelField(clip.name, GUILayout.Height(MStyle.labelHeight));
                
                //显示播放/暂停按钮
                if(playingAudioIndex == i)
                {
                    //如果播放完了那就更新正在播放的AudioIndex
                    if (!EditorGUITool.IsEditorPlayingAudio())
                    {
                        playingAudioIndex = -1;
                    }
                
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
                    
                        //播放音效
                        EditorPlayAudio(clip);
                    }
                }
                
                //删除
                if (GUILayout.Button("删除", GUILayout.Height(MStyle.labelHeight), GUILayout.Width(MStyle.btnWidth)))
                {
                    audioClips[i] = null;
                    Repaint();
                }

                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
            
            GUILayout.Space(10f);
            //绘制拖拽添加AudioClip的区域
            DrawDragAudioClipBox(audioClips);
            
            GUILayout.Space(10f);
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制BGMClip的GUI
        /// </summary>
        private void DrawBGMClipGUI()
        {
            EditorGUILayout.BeginVertical();
            
            EditorGUITool.BoldLabelField("BGMClip列表");
            GUILayout.Space(10f);

            bgmClipScrollPosition = EditorGUILayout.BeginScrollView(bgmClipScrollPosition);
            
            //遍历进行绘制
            for (int i = 0; i < bgmClips.Count; i++)
            {
                AudioClip clip = bgmClips[i];
                if (clip == null) continue;

                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(36f), GUILayout.Height(MStyle.labelHeight));
                
                EditorGUILayout.LabelField(clip.name, GUILayout.Height(MStyle.labelHeight));
                
                //显示播放/暂停按钮
                if(playingAudioIndex == i)
                {
                    //如果播放完了那就更新正在播放的AudioIndex
                    if (!EditorGUITool.IsEditorPlayingAudio())
                    {
                        playingAudioIndex = -1;
                    }
                
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
                    
                        //播放音效
                        EditorPlayAudio(clip);
                    }
                }
                
                //删除
                if (GUILayout.Button("删除", GUILayout.Height(MStyle.labelHeight), GUILayout.Width(MStyle.btnWidth)))
                {
                    bgmClips[i] = null;
                    Repaint();
                }

                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
            
            GUILayout.Space(10f);
            //绘制拖拽添加AudioClip的区域
            DrawDragAudioClipBox(bgmClips);
            
            GUILayout.Space(10f);
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制Group列表
        /// </summary>
        private void DrawGroupGUI()
        {
            EditorGUILayout.BeginVertical();
            
            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("打开AudioMixer编辑", GUILayout.Height(MStyle.labelHeight)))
            {
                AssetDatabase.OpenAsset(Library.AudioMixer);
            }
            if (GUILayout.Button("刷新Group", GUILayout.Height(MStyle.labelHeight)))
            {
                AudioLibrary.Instance.UpdateAudioGroups();
                AudioLibrary.Instance.SaveAsset();
            }
            
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10f);

            EditorGUILayout.LabelField("Group列表",EditorStyles.boldLabel);
            
            groupScrollPosition = EditorGUILayout.BeginScrollView(groupScrollPosition);

            for (int i = 0; i < Library.AudioGroups.Count; i++)
            {
                EditorGUILayout.LabelField(Library.AudioGroups[i].GroupName);
                GUILayout.Space(MStyle.spacing);
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
                //如果播放完了那就更新正在播放的AudioIndex
                if (!EditorGUITool.IsEditorPlayingAudio())
                {
                    playingAudioIndex = -1;
                }
                
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
                                clip = bgmStack.GetClip();
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

            if (mode == 0)
            {
                //点击打开编辑
                if(GUILayout.Button("编辑",GUILayout.Height(MStyle.labelHeight), GUILayout.Width(MStyle.btnWidth)))
                {
                
                    switch (option)
                    {
                        case BGMGUI bgm:
                            BGMEditor.ShowWindow(bgm.Stack);
                            break;
                        case AudioGUI audio:
                            AudioStackEditor.ShowWindow(audio.Stack);
                            break;
                    }
                }
            
                //点击删除
                if(GUILayout.Button("删除",GUILayout.Height(MStyle.labelHeight), GUILayout.Width(MStyle.btnWidth)))
                {
                    if (EditorUtility.DisplayDialog("警告", "这是一项危险操作，你确定要删除吗？", "确定", "取消"))
                    {
                        switch (option)
                        {
                            case BGMGUI bgm:
                                AudioLibrary.DeleteBGMStack(bgm.Stack);
                                //然后删除GUI
                                bgmGUIs.RemoveAt(i);
                                Repaint();
                                break;
                            case AudioGUI audio:
                                AudioLibrary.DeleteAudioStack(audio.Stack);
                                //然后删除GUI
                                audioGUIs.RemoveAt(i);
                                Repaint();
                                break;
                        }
                    
                    }

                }
            }
            else if (mode == 1)
            {
                //点击选择
                if(GUILayout.Button("选择",GUILayout.Height(MStyle.labelHeight), GUILayout.Width(MStyle.btnWidth)))
                {
                    //调用回调，然后关闭窗口
                    switch (option)
                    {
                        case BGMGUI bgm:
                            selectCallback?.Invoke(bgm.Stack.BGMIndex);
                            break;
                        case AudioGUI audio:
                            selectCallback?.Invoke(audio.Stack.AudioIndex);
                            break;
                    }
                    Close();
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

