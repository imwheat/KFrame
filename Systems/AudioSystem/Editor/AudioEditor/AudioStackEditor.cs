//****************** 代码文件申明 ************************
//* 文件：AudioStackEditor                                       
//* 作者：wheat
//* 创建时间：2024/09/02 10:01:49 星期一
//* 描述：单个音效的编辑器
//*****************************************************
using UnityEngine;
using KFrame.Systems;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using KFrame;
using KFrame.Editor;
using UnityEditor;
using UnityEngine.Audio;

namespace KFrame.Systems
{
    public class AudioStackEditor : EditorWindow
    {
        #region 参数引用
        
        /// <summary>
        /// 音效库
        /// </summary>
        private static AudioLibrary library => AudioEditor.Library;

        #endregion
        
        #region GUI相关

        /// <summary>
        /// 编辑模式
        /// </summary>
        private enum EditMode
        {
            /// <summary>
            /// 编辑音效Stack
            /// </summary>
            Edit = 0,
            /// <summary>
            /// 新建音效Stack
            /// </summary>
            Create = 1,
        }
        /// <summary>
        /// ScrollView的位置
        /// </summary>
        private Vector2 editorScrollPosition;
        /// <summary>
        /// 保存模版
        /// </summary>
        private bool saveTemplate;
        /// <summary>
        /// 模版名称
        /// </summary>
        private string templateName;
        /// <summary>
        /// 显示AudioClips
        /// </summary>
        private bool showClips;
        
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
            internal static readonly float btnWidth = 40f;
            internal static readonly float labelHeight = 15f;
            
        }

        #endregion

        #region 编辑相关

        /// <summary>
        /// 编辑模式
        /// </summary>
        private EditMode editMode;
        /// <summary>
        /// 是否编辑了数据
        /// </summary>
        private bool editted;
        /// <summary>
        /// 当前编辑的音效
        /// </summary>
        private AudioStack curEditStack;
        /// <summary>
        /// 当前正在编辑的新建音效
        /// </summary>
        private AudioStackCreateEditor.AudioCreateGUI curEditCreateGUI;
        /// <summary>
        /// 正在编辑的音效数据
        /// </summary>
        private AudioEditData curEditData;
        /// <summary>
        /// 旧的音效数据
        /// </summary>
        private AudioEditData oldEditData;

        #endregion

        #region 初始化
        
        /// <summary>
        /// 打开面板
        /// </summary>
        /// <param name="audioStack">要编辑的音效</param>
        /// <returns></returns>
        public static AudioStackEditor ShowWindow(AudioStack audioStack)
        {
            AudioStackEditor window = EditorWindow.CreateWindow<AudioStackEditor>();
            window.titleContent = new GUIContent("正在编辑:"+audioStack.AudioName);
            window.Init(audioStack);

            return window;
        }
        /// <summary>
        /// 打开面板
        /// </summary>
        /// <param name="audioStack">要编辑的音效</param>
        /// <returns></returns>
        public static AudioStackEditor ShowWindow(AudioStackCreateEditor.AudioCreateGUI editCreateGUI)
        {
            AudioStackEditor window = EditorWindow.CreateWindow<AudioStackEditor>();
            window.titleContent = new GUIContent("正在编辑:"+editCreateGUI.AudioName);
            window.Init(editCreateGUI);

            return window;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="audioStack"></param>
        private void Init(AudioStackCreateEditor.AudioCreateGUI editCreateGUI)
        {
            editMode = EditMode.Create;
            curEditStack = null;
            curEditCreateGUI = editCreateGUI;
            curEditData = new AudioEditData(editCreateGUI.EditData);
            oldEditData = new AudioEditData(editCreateGUI.EditData);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="audioStack"></param>
        private void Init(AudioStack audioStack)
        {
            editMode = EditMode.Edit;
            curEditStack = audioStack;
            curEditCreateGUI = null;
            curEditData = new AudioEditData(curEditStack);
            oldEditData = new AudioEditData(curEditStack);
        }

        #endregion

        #region 生命周期

        private void OnDisable()
        {
            //关闭的时候检测要不要保存本次更改
            AskForSave();
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
            v = (T)EditorGUILayout.ObjectField(label, v, typeof(T), false,GUILayout.Height(MStyle.labelHeight));
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

        #region GUI绘制相关

        private void OnGUI()
        {
            //绘制模版相关选项
            DrawTemplateOption();
            
            //绘制编辑主界面
            DrawMainEditorGUI();
        }
        /// <summary>
        /// 绘制模版相关选项
        /// </summary>
        private void DrawTemplateOption()
        {
            EditorGUILayout.BeginVertical();
            
            GUILayout.Space(10f);

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
                        curEditData.CopyData(data);
                    });
                

                }
                if (GUILayout.Button("保存为模版", GUILayout.Height(MStyle.labelHeight)))
                {
                    templateName = "";
                    saveTemplate = true;
                }
            }
            else
            {
                EditorGUILayout.LabelField("模板名称：", GUILayout.Width(90f), GUILayout.Height(MStyle.labelHeight));
                templateName = EditorGUILayout.TextField(templateName, GUILayout.Height(MStyle.labelHeight));
                if (GUILayout.Button("保存", GUILayout.Height(MStyle.labelHeight)))
                {
                    if (string.IsNullOrEmpty(templateName))
                    {
                        EditorUtility.DisplayDialog("错误", "模版名称不能为空！", "确认");
                    }
                    else
                    {
                        AudioEditorLibrary.Instance.SaveAudioTemplate(curEditData, templateName);
                        saveTemplate = false;
                    }

                }
                if (GUILayout.Button("取消", GUILayout.Height(MStyle.labelHeight)))
                {
                    saveTemplate = false;
                }
            }

            
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10f);
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制编辑主界面
        /// </summary>
        private void DrawMainEditorGUI()
        {
            EditorGUILayout.BeginVertical();
            
            editorScrollPosition = EditorGUILayout.BeginScrollView(editorScrollPosition);

            EditorGUI.BeginChangeCheck();
            
            #region 编辑参数

            if (editMode == EditMode.Edit)
            {
                DrawIntField(ref curEditData.AudioIndex, "音效 id:");
            }
            
            DrawTextField(ref curEditData.AudioName, "音效名称:");
            DrawObjectField<AudioMixerGroup>(ref curEditData.AudioMixerGroup, "音效分组:");
            DrawSliderField(ref curEditData.Volume, "音量:", 0f, 1f);
            DrawSliderField(ref curEditData.Pitch, "音高:", -3f, 3f);
            DrawSliderField(ref curEditData.RandomMinPitch, "随机最低音高:", -3f, 3f);
            DrawSliderField(ref curEditData.RandomMaxPitch, "随机最高音高:", -3f, 3f);
            DrawSliderField(ref curEditData.MaxPitch, "最高音高:", -3f, 3f);
            DrawBoolField(ref curEditData.LimitContinuousPlay, "限制连续播放:");
            if (curEditData.LimitContinuousPlay)
            {
                DrawIntField(ref curEditData.LimitPlayCount, "限制连续播放数量:");
            }
            DrawBoolField(ref curEditData.Loop, "循环播放:");
            DrawBoolField(ref curEditData.Is3D, "3D音效:");
            
            #endregion

            //标志更改
            if (EditorGUI.EndChangeCheck())
            {
                SetEditted();
            }
            
            DrawEnumField<AudioesType>(curEditData.AudioType, "音效类型:", (x) =>
            {
                curEditData.AudioType = (AudioesType)x;
                SetEditted();
            });

            
            EditorGUITool.ShowAClearFoldOut(ref showClips, "Clip列表");
            if (showClips)
            {
                GUILayout.Space(5f);
                if (GUILayout.Button("编辑Clip列表", GUILayout.Height(MStyle.labelHeight)))
                {
                    AudioClipsListWindow.ShowWindow(curEditData.Clips);
                    
                    SetEditted();
                }
                
                GUILayout.Space(5f);
                
                for (int i = 0; i < curEditData.Clips.Count; i++)
                {
                    EditorGUILayout.LabelField(curEditData.Clips[i].name,
                        GUILayout.Height(MStyle.labelHeight));
                    
                    GUILayout.Space(MStyle.spacing);
                }
            }
            
            EditorGUILayout.EndScrollView();
            
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("保存更改", GUILayout.Height(MStyle.labelHeight)))
            {
                SaveEdit();
            }
            
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region 编辑操作相关
        /// <summary>
        /// 询问是否应用本次更改
        /// </summary>
        private void AskForSave()
        {
            //如果没有更改，那就不需要询问
            if(!editted) return;

            if (EditorUtility.DisplayDialog("提示", "是否要应用本次更改", "确认", "取消"))
            {
                SaveEdit();
            }
        }
        /// <summary>
        /// 标志被编辑了
        /// </summary>
        private void SetEditted()
        {
            //如果已经标记过了就返回
            if(editted) return;

            editted = true;
            titleContent.text += "*";
        }
        /// <summary>
        /// 清除编辑标记
        /// </summary>
        private void ClearEditted()
        {
            //如果没有标记，那就返回
            if(!editted) return;

            editted = false;
            titleContent.text = titleContent.text.Substring(0, titleContent.text.Length - 1);
        }
        /// <summary>
        /// 保存前的检测
        /// </summary>
        /// <returns>如果没有问题那就返回false</returns>
        private bool SaveCheck()
        {
            string errorTitle = "错误:保存失败";
            
            //如果在编辑模式，再检测一下id
            if (editMode == EditMode.Edit &&
                !AudioLibrary.Instance.CheckAudioIndexValid(curEditStack, curEditData.AudioIndex))
            {
                curEditData.AudioIndex = oldEditData.AudioIndex;
                EditorUtility.DisplayDialog(errorTitle, "音效id不合理\n自动更正为原先id", "确认");
                
                return true;
            }
            
            //更新分组id
            curEditData.AudioGroupIndex = AudioDic.GetAudioGroupIndex(curEditData.AudioMixerGroup);
            //如果音效名称为空也不合理
            if (string.IsNullOrEmpty(curEditData.AudioName))
            {
                curEditData.AudioName = oldEditData.AudioName;
                EditorUtility.DisplayDialog(errorTitle, "音效名称不能为空\n自动更正为原先名称", "确认");
                
                return true;
            }
            //如果随机音高最小值大于最大值就不合理
            if (curEditData.RandomMinPitch > curEditData.RandomMaxPitch)
            {
                EditorUtility.DisplayDialog(errorTitle, "随机音高不合理\n最小值不能大于最大值", "确认");
                
                return true;
            }
            //遍历检测clip
            foreach (AudioClip clip in curEditData.Clips)
            {
                library.EditorCheckAudioClip(clip);
            }
            
            //没有什么问题就返回false
            return false;
        }
        /// <summary>
        /// 保存编辑
        /// </summary>
        private void SaveEdit()
        {
            //点击保存的时候要结束编辑TextField
            EditorGUITool.EndEditTextField();
            
            //如果编辑了，那就赋值
            if (editted)
            {
                //进行保存前的检测，如果有问题，那就返回
                if (SaveCheck())
                {
                    return;
                }
                
                //更新下标列表
                curEditData.ClipIndexes = new List<int>();
                AudioDic.FindIndexesByAudioClips(curEditData.ClipIndexes, curEditData.Clips);
                
                //保存数据
                curEditData.PasteData(curEditStack);
                if (curEditCreateGUI != null)
                {
                    curEditCreateGUI.SyncUpdate = false;
                    curEditCreateGUI.EditData.CopyData(curEditData);
                }
            }
            
            //清除编辑标记
            ClearEditted();
            
            //保存
            EditorUtility.SetDirty(library);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        #endregion
    }
}

#endif