//****************** 代码文件申明 ************************
//* 文件：AudioStackCreateEditor                                       
//* 作者：wheat
//* 创建时间：2024/09/04 13:45:10 星期三
//* 描述：创建AudioStack的窗口
//*****************************************************

#if UNITY_EDITOR

using UnityEngine;
using KFrame;
using System;
using System.Collections.Generic;
using GameBuild;
using KFrame.Editor;
using UnityEditor;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace KFrame.Systems
{
    public class AudioStackCreateEditor : EditorWindow
    {

        #region GUI绘制相关

        /// <summary>
        /// 音效Stack创建的GUI
        /// </summary>
        public class AudioCreateGUI
        {
            /// <summary>
            /// 新建音效Stack的数据
            /// </summary>
            public AudioEditData EditData;

            /// <summary>
            /// 新建的音效名称
            /// </summary>
            public string AudioName
            {
                get => EditData.AudioName;
                set => EditData.AudioName = value;
            }
            /// <summary>
            /// 音效的Clip
            /// </summary>
            public List<AudioClip> AudioClips
            {
                get => EditData.Clips;
                set => EditData.Clips = value;
            }
            /// <summary>
            /// 折叠显示Clip
            /// </summary>
            public bool ClipFoldout;
            /// <summary>
            /// 同步更新
            /// </summary>
            public bool SyncUpdate;
            
            /// <summary>
            /// 简化名称
            /// 如果末尾是数字或者'_'结尾的就去掉
            /// </summary>
            private static string SimplifyName(string name)
            {
                while (name.Length > 0 && ((name[^1] >= '0' && name[^1] <= '9') || name[^1] == '_'))
                {
                    name = name.Substring(0, name.Length - 1);
                }

                return name;
            }
            private AudioCreateGUI(AudioEditData baseData, string audioName)
            {
                EditData = new AudioEditData(baseData);
                AudioName = audioName;
                ClipFoldout = true;
                SyncUpdate = true;
            }
            
            public AudioCreateGUI(AudioEditData baseData,AudioClip clip): this(baseData, clip.name)
            {
                AudioClips = new List<AudioClip>() {clip};
            }

            public AudioCreateGUI(AudioEditData baseData,List<AudioClip> clips): this(baseData, SimplifyName(clips[0].name))
            {
                AudioClips = new List<AudioClip>(clips);
            }
            /// <summary>
            /// 同步更新数据
            /// </summary>
            /// <param name="data">要拷贝的数据</param>
            public void SyncUpdateData(AudioEditData data)
            {
                //如果关闭了同步更新那就返回
                if(!SyncUpdate) return;
                
                EditData.CopyParams(data);
            }
        }
        
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
            internal static readonly float labelWidth = 80f;
            internal static readonly float labelHeight = 20f;
            internal static readonly float mainGUIMaxWidth = 350f;
            internal static readonly float sideGUIMinWidth = 250f;
            internal static readonly float lineWidth = 2f;
            
        }

        /// <summary>
        /// 列表滚轮位置
        /// </summary>
        private Vector2 listScrollPosition;
        /// <summary>
        /// 保存模版
        /// </summary>
        private bool saveTemplate;

        /// <summary>
        /// 要创建的AudioStack的GUI
        /// </summary>
        private List<AudioCreateGUI> createGUIs;
        /// <summary>
        /// 新创建的音效的公共主词条
        /// </summary>
        private AudioEditData mainEditData;
        /// <summary>
        /// 主词条编辑界面滚轮位置
        /// </summary>
        private Vector2 mainEditScrollPos;

        #endregion

        #region 初始化调用

        public static void ShowWindow()
        {
            AudioStackCreateEditor window = EditorWindow.GetWindow<AudioStackCreateEditor>();
            window.titleContent = new GUIContent("音效创建面板");
            if (window.position.width < MStyle.sideGUIMinWidth * 2f + MStyle.lineWidth)
            {
                window.position = new Rect(window.position.position, new Vector2(MStyle.sideGUIMinWidth * 2f + MStyle.lineWidth, window.position.height));
            }
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
            createGUIs = new List<AudioCreateGUI>();
            mainEditData = AudioEditData.Default();
        }
        #endregion
        
        #region 单个Filed绘制

        
        /// <summary>
        /// 绘制IntField
        /// </summary>
        /// <param name="v">要更改的值</param>
        private void DrawIntField(ref int v, string label)
        {
            v = EditorGUILayout.IntField(label, v, GUILayout.Height(MStyle.labelHeight));
            GUILayout.Space(MStyle.spacing);
        }
        
        /// <summary>
        /// 绘制FloatField
        /// </summary>
        /// <param name="v">要更改的值</param>
        private void DrawFloatField(ref float v, string label)
        {
            v = EditorGUILayout.FloatField(label, v, GUILayout.Height(MStyle.labelHeight));
            GUILayout.Space(MStyle.spacing);
        }

        /// <summary>
        /// 绘制SliderField
        /// </summary>
        /// <param name="v">要更改的值</param>
        private void DrawSliderField(ref float v, string label, float min, float max)
        {
            v = EditorGUILayout.Slider(label, v, min, max,GUILayout.Height(MStyle.labelHeight));
            GUILayout.Space(MStyle.spacing);
        }
        /// <summary>
        /// 绘制BoolField
        /// </summary>
        /// <param name="v">要更改的值</param>
        private void DrawBoolField(ref bool v, string label)
        {
            v = EditorGUILayout.Toggle(label, v, GUILayout.Height(MStyle.labelHeight));
            GUILayout.Space(MStyle.spacing);
        }
        /// <summary>
        /// 绘制ObjectField
        /// </summary>
        /// <param name="v">要更改的值</param>
        private void DrawObjectField<T>(ref T v, string label) where T: Object
        {
            v = (T)EditorGUILayout.ObjectField(label, v, typeof(T),false, GUILayout.Height(MStyle.labelHeight));
            GUILayout.Space(MStyle.spacing);
        }
        /// <summary>
        /// 绘制TextField
        /// </summary>
        /// <param name="text">要更改的Text</param>
        private void DrawTextField(ref string text, string label)
        {
            text = EditorGUILayout.TextField(label, text, GUILayout.Height(MStyle.labelHeight));
            GUILayout.Space(MStyle.spacing);
        }
        /// <summary>
        /// 绘制EnumField
        /// </summary>
        /// <param name="v">要更改的值</param>
        private void DrawEnumField<TEnum>(TEnum v,string label, Action<int> action) where TEnum : struct
        {
            EditorGUITool.ShowEnumSelectOption<TEnum>(label, v.ToString(), action ,true,GUILayout.Height(MStyle.labelHeight));
            GUILayout.Space(MStyle.spacing);
        }
        
        #endregion

        #region GUI绘制

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            
            //绘制主词条区域
            DrawMainEditGUI();
            
            //绘制分割线
            DrawDivideBar();
            
            //绘制副编辑区域
            DrawSideEditGUI();
            
            EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 绘制侧栏编辑GUI
        /// </summary>
        private void DrawSideEditGUI()
        {
            //绘制顶部的选项GUI
            DrawTopGUI();

            //绘制列表GUI
            DrawListGUI();
        }
        /// <summary>
        /// 绘制侧栏分割线
        /// </summary>
        private void DrawDivideBar()
        {
            //分割线
            Rect lineRect = EditorGUILayout.GetControlRect(GUILayout.Width(MStyle.lineWidth), GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(lineRect, Color.black);
        }
        /// <summary>
        /// 绘制拖拽放入的BOX
        /// </summary>
        private void DrawDragBox()
        {
            EditorGUILayout.BeginVertical();
	        
            GUILayout.Label("把Clip丢进这里添加:", EditorStyles.boldLabel);
                        
            //获取当前事件
            Event currentEvent = Event.current;
                        
            //画出一个拖拽区域
            Rect dropArea = GUILayoutUtility.GetRect(0f, 50f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "放入Clip区域");
                        
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
                        List<AudioClip> clips = new List<AudioClip>();
                        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                        {
                            //如果是AudioClip
                            if (draggedObject is AudioClip)
                            {
                                //那就获取添加到列表里面
                                AudioClip clip = (AudioClip)draggedObject;
                                clips.Add(clip);
                            }
                        }
                        

                        //如果只有一个那就直接添加
                        if (clips.Count == 1)
                        {
                            createGUIs.Add(new AudioCreateGUI(mainEditData, clips[0]));
                        }
                        //如果数量大于1个可以选择每个独立创建多个
                        //或者把所有clip放在1个里面创建1个
                        else if (clips.Count > 1)
                        {
                            EditorGUITool.DisplayDialog("提示", "一次拖入了多个Clip，你可以选择：" +
                                                              "\n1.每个Clip独立创建1个音效" +
                                                              "\n2.把所有Clip放在1个音效里面，只创建1个音效", "创建一个", "独立分开",
                                (x) =>
                                {
                                    switch (x)
                                    {
                                        case EditorSelectWindow.ConfirmType.None:
                                            break;
                                        //独立分开
                                        case EditorSelectWindow.ConfirmType.Cancel:
                                            for (int i = 0; i < clips.Count; i++)
                                            {
                                                createGUIs.Add(new AudioCreateGUI(mainEditData, clips[i]));
                                            }
                                            Repaint();
                                            break;
                                        //创建一个
                                        case EditorSelectWindow.ConfirmType.Confirm:
                                            createGUIs.Add(new AudioCreateGUI(mainEditData, clips));
                                            Repaint();
                                            break;
                                    }
                                });

                        }
                    }
                    Event.current.Use();
                    break;
            }
	        
            EditorGUILayout.EndVertical();
	        
        }
        
        /// <summary>
        /// 绘制模版相关选项
        /// </summary>
        private void DrawTemplateOption()
        {
             EditorGUILayout.BeginHorizontal();

            if (!saveTemplate)
            {
                            
                if (GUILayout.Button("加载模版", GUILayout.Height(MStyle.labelHeight)))
                {
                    //结束编辑输入栏
                    EditorGUITool.EndEditTextField();
                
                    //加载模版界面
                    AudioTemplateWindow.ShowWindow((data) =>
                    {
                        //加载数据
                        mainEditData.CopyData(data);
                        //同步更新其他数据
                        SyncUpdateDatas();
                    });
                

                }
                if (GUILayout.Button("保存为模版", GUILayout.Height(MStyle.labelHeight)))
                {
                    mainEditData.AudioName = "";
                    saveTemplate = true;
                }
            }
            else
            {
                EditorGUILayout.LabelField("模板名称：", GUILayout.Width(90f), GUILayout.Height(MStyle.labelHeight));
                mainEditData.AudioName = EditorGUILayout.TextField(mainEditData.AudioName, GUILayout.Height(MStyle.labelHeight));
                if (GUILayout.Button("保存", GUILayout.Height(MStyle.labelHeight)))
                {
                    if (string.IsNullOrEmpty(mainEditData.AudioName))
                    {
                        EditorUtility.DisplayDialog("错误", "模版名称不能为空！", "确认");
                    }
                    else
                    {
                        AudioEditorLibrary.Instance.SaveAudioTemplate(mainEditData, mainEditData.AudioName);
                        mainEditData.AudioName = "";
                        saveTemplate = false;
                    }

                }
                if (GUILayout.Button("取消", GUILayout.Height(MStyle.labelHeight)))
                {
                    mainEditData.AudioName = "";
                    saveTemplate = false;
                }
            }

            
            EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 绘制待创建的音效的公共主词条的编辑界面
        /// </summary>
        private void DrawMainEditGUI()
        {
            float mainWidth = Mathf.Min((position.width - MStyle.labelWidth) / 2f, MStyle.mainGUIMaxWidth);
            EditorGUILayout.BeginVertical(GUILayout.Width(mainWidth));

            EditorGUILayout.LabelField("公共主词条编辑：");
            
            mainEditScrollPos = EditorGUILayout.BeginScrollView(mainEditScrollPos);

            EditorGUI.BeginChangeCheck();
            
            #region 编辑参数

            DrawObjectField<AudioMixerGroup>(ref mainEditData.AudioMixerGroup, "音效分组:");
            DrawSliderField(ref mainEditData.Volume, "音量:", 0f, 1f);
            DrawSliderField(ref mainEditData.Pitch, "音高:", -3f, 3f);
            DrawSliderField(ref mainEditData.RandomMinPitch, "随机最低音高:", -3f, 3f);
            DrawSliderField(ref mainEditData.RandomMaxPitch, "随机最高音高:", -3f, 3f);
            DrawSliderField(ref mainEditData.MaxPitch, "最高音高:", -3f, 3f);
            DrawBoolField(ref mainEditData.LimitContinuousPlay, "限制连续播放:");
            if (mainEditData.LimitContinuousPlay)
            {
                DrawIntField(ref mainEditData.LimitPlayCount, "限制连续播放数量:");
            }
            DrawBoolField(ref mainEditData.Loop, "循环播放:");
            DrawBoolField(ref mainEditData.Is3D, "3D音效:");
            
            #endregion

            if (EditorGUI.EndChangeCheck())
            {
                //同步更新其他数据
                SyncUpdateDatas();
            }

            DrawEnumField<AudioesType>(mainEditData.AudioType, "音效类型:", (x) =>
            {
                mainEditData.AudioType = (AudioesType)x;
                //同步更新其他数据
                SyncUpdateDatas();
            });

            EditorGUILayout.EndScrollView();
            
            GUILayout.Space(5f);

            //绘制模版相关选项
            DrawTemplateOption();
            
            GUILayout.Space(10f);
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制顶部的选项GUI
        /// </summary>
        private void DrawTopGUI()
        {
            EditorGUILayout.BeginVertical();
            
            GUILayout.Space(10f);
            
            //确认并创建
            if (GUILayout.Button("确认并创建", GUILayout.Height(20f)))
            {
                CreateAudioStack();
            }
            
            GUILayout.Space(10f);
            
            //绘制拖拽放入区域
            DrawDragBox();
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制单个创建选项的GUI
        /// </summary>
        /// <param name="createGUI">待创建的音效的GUI</param>
        /// <param name="mainWidth">首列GUI宽度</param>
        /// <param name="sideWidth">侧栏GUI宽度</param>
        /// <param name="index">在列表中的下标</param>
        private void DrawAudioCreateGUI(AudioCreateGUI createGUI, float mainWidth, float sideWidth, int index)
        {
            //计算按钮宽度
            float btnWidth = sideWidth - 2 * MStyle.btnSpace;
            //计算左列和右列GUI的高度，然后选较大的
            float leftGUIHeight = MStyle.labelHeight * 2f + MStyle.spacing +
                                  (createGUI.ClipFoldout ? createGUI.AudioClips.Count * (MStyle.labelHeight + MStyle.spacing) : 0f);
            float rightGUIHeight = MStyle.labelHeight * 3f + MStyle.spacing * 2f;
            float guiHeight = Mathf.Max(leftGUIHeight, rightGUIHeight) + 2 * MStyle.spacing;
            //然后再算出左侧GUI和右侧GUI的上下空余空间
            float leftSpace = (guiHeight - leftGUIHeight) / 2f;
            float rightSpace = (guiHeight - rightGUIHeight) / 2f;
            
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();
            
            //首列显示选项参数
            EditorGUILayout.BeginVertical();

            GUILayout.Space(leftSpace);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("音效名称:", GUILayout.Width(MStyle.labelWidth), 
                GUILayout.Height(MStyle.labelHeight));
            GUILayout.Space(5f);
            createGUI.AudioName = EditorGUILayout.TextField(createGUI.AudioName,
            GUILayout.Width(mainWidth - MStyle.labelWidth - 5f),GUILayout.Height(MStyle.labelHeight));
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(MStyle.spacing);

            EditorGUITool.ShowAClearFoldOut(ref createGUI.ClipFoldout, "Clips");
            if (createGUI.ClipFoldout)
            {
                for (int i = 0; i < createGUI.AudioClips.Count; i++)
                {
                    EditorGUILayout.LabelField(createGUI.AudioClips[i].name, 
                        GUILayout.Width(mainWidth),GUILayout.Height(MStyle.labelHeight));
                }
            }
            
            GUILayout.Space(leftSpace);
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(5f);
            GUILayout.Space(MStyle.btnSpace);
            
            //侧列，显示按钮选项
            EditorGUILayout.BeginVertical();
            
            GUILayout.Space(rightSpace);

            if (GUILayout.Button("上移", GUILayout.Width(btnWidth), GUILayout.Height(MStyle.labelHeight)))
            {
                if (index != 0)
                {
                    createGUIs.RemoveAt(index);
                    createGUIs.Insert(index - 1, createGUI);
                    Repaint();
                }
            }
            GUILayout.Space(MStyle.spacing);
            if (GUILayout.Button("删除", GUILayout.Width(btnWidth), GUILayout.Height(MStyle.labelHeight)))
            {
                createGUIs.RemoveAt(index);
                Repaint();
            }
            GUILayout.Space(MStyle.spacing);
            if (GUILayout.Button("下移", GUILayout.Width(btnWidth), GUILayout.Height(MStyle.labelHeight)))
            {
                if (index != createGUIs.Count - 1)
                {
                    createGUIs.RemoveAt(index);
                    createGUIs.Insert(index + 1, createGUI);
                    Repaint();
                }
            }
            
            GUILayout.Space(rightSpace);
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(MStyle.btnSpace);
            
            EditorGUILayout.EndHorizontal();
            
            //分割线
            Rect lineRect = EditorGUILayout.GetControlRect(GUILayout.Height(MStyle.lineWidth), GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(lineRect, Color.black);
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制列表GUI
        /// </summary>
        private void DrawListGUI()
        {
            float sideWidth = Mathf.Min(position.width / 3f, MStyle.btnWidth + 2 * MStyle.btnSpace);
            float mainWidth = position.width - sideWidth - 5f;
            
            EditorGUILayout.BeginVertical();

            listScrollPosition = EditorGUILayout.BeginScrollView(listScrollPosition);
            
            //遍历绘制每个GUI
            for (int i = 0; i < createGUIs.Count; i++)
            {
                DrawAudioCreateGUI(createGUIs[i], mainWidth, sideWidth, i);
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }
        
        #endregion

        #region 保存与编辑

        /// <summary>
        /// 同步更新列表数据
        /// </summary>
        private void SyncUpdateDatas()
        {
            //遍历每一个编辑数据，进行同步更新
            foreach (AudioCreateGUI createGUI in createGUIs)
            {
                createGUI.SyncUpdateData(mainEditData);
            }
        }
        /// <summary>
        /// 检测待创建的音效
        /// </summary>
        /// <param name="createGUI">待创建的音效</param>
        /// <param name="errorText">输出的错误文本</param>
        /// <returns>如果没问题返回false，有问题返回true</returns>
        private bool CheckAudio(AudioCreateGUI createGUI, out string errorText)
        {
            errorText = "";

            if (AudioEditor.Library.CheckAudioNameValid(createGUI.AudioName) == false)
            {
                errorText = "音效名称有问题：";
                if (string.IsNullOrEmpty(createGUI.AudioName))
                {
                    errorText += "名称为空";
                }
                else
                {
                    errorText += "名称重复";
                }
                return true;
            }

            if (createGUI.AudioClips.Count == 0)
            {
                errorText = "Clip数量为0";
                return true;
            }
            
            return false;
        }
        /// <summary>
        /// 把当前列表里的音效创建添加到库中
        /// </summary>
        private void CreateAudioStack()
        {
            //结束编辑输入栏
            EditorGUITool.EndEditTextField();
            
            //如果没有待创建的项的话就返回
            if(createGUIs.Count == 0) return;
            
            //当前列表自检
            HashSet<string> creatingAudioNames = new HashSet<string>();
            foreach (AudioCreateGUI createGUI in createGUIs)
            {
                if (!creatingAudioNames.Add(createGUI.AudioName))
                {
                    EditorUtility.DisplayDialog("错误", $"创建 {createGUI.AudioName} 时发生错误：\n"+"已经包含重复名称的待创建音效。", "确认");
                    return;
                }
            }
            
            //添加到库中检测
            foreach (AudioCreateGUI createGUI in createGUIs)
            {
                if (CheckAudio(createGUI, out string errorText))
                {
                    EditorUtility.DisplayDialog("错误", $"创建 {createGUI.AudioName} 时发生错误：\n"+errorText, "确认");
                    return;
                }
            }
            
            //开始逐个创建
            foreach (AudioCreateGUI createGUI in createGUIs)
            {
                //创建并添加到库中
                AudioLibrary.AddAudioStack(CreateAudioStack(createGUI.EditData));
            }
            
            //创建完了清空列表
            createGUIs.Clear();
            
            //保存
            AudioLibrary.Instance.SaveAsset();
        }
        
        /// <summary>
        /// 把编辑器数据创建为AudioStack
        /// </summary>
        /// <param name="editData"></param>
        /// <returns></returns>
        public AudioStack CreateAudioStack(AudioEditData editData)
        {
            //创建一个Stack
            AudioStack stack = new AudioStack();
            
            //更新EditData数据
            //遍历更新Clipid列表
            editData.ClipIndexes.Clear();
            foreach (AudioClip clip in editData.Clips)
            {
                //如果能找到那就直接添加
                int id = AudioDic.GetAudioClipIndex(clip);
                if (id == -1)
                {
                    //找不到的话那就是新的音效，那就先加入库中然后添加
                    id = AudioLibrary.Instance.AudioClips.Count;
                    AudioLibrary.Instance.AudioClips.Add(clip);
                    AudioDic.SetAudioClipIndex(clip, id);
                }
                
                editData.ClipIndexes.Add(id);

            }
            
            //更新GroupIndex
            editData.AudioGroupIndex = AudioDic.GetAudioGroupIndex(editData.AudioMixerGroup);
            
            //复制数据
            editData.PasteData(stack);
            //设置id
            stack.AudioIndex = AudioLibrary.MaxAudioIndex + 1;
            AudioLibrary.MaxAudioIndex++;
            
            //返回Stack
            return stack;
        }

        #endregion
        
    }
}

#endif