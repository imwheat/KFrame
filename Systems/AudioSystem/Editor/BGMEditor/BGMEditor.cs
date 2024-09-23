//****************** 代码文件申明 ***********************
//* 文件：BGMEditor
//* 作者：wheat
//* 创建时间：2024/09/13 11:19:48 星期五
//* 描述：
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame;
using KFrame.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using KFrame.Editor;
using Object = UnityEngine.Object;

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
        /// <summary>
        /// ScrollView的位置
        /// </summary>
        private Vector2 editorScrollPosition;

        #endregion
        
        #region 编辑相关
        
        private enum EditMode
        {
            /// <summary>
            /// 编辑模式
            /// </summary>
            Edit = 0,
            /// <summary>
            /// 创建模式
            /// </summary>
            Create = 1,
        }
        /// <summary>
        /// 编辑模式
        /// </summary>
        private EditMode editMode;
        /// <summary>
        /// 是否编辑了数据
        /// </summary>
        private bool editted;
        /// <summary>
        /// 当前编辑的BGM
        /// </summary>
        private BGMStack curEditStack;
        /// <summary>
        /// 正在编辑的BGM数据
        /// </summary>
        private BGMEditData curEditData;
        /// <summary>
        /// 旧的BGM数据
        /// </summary>
        private BGMEditData oldEditData;

        #endregion
        
        #region 初始化
        
        /// <summary>
        /// 打开面板并创建新的BGM
        /// </summary>
        public static BGMEditor ShowWindowCreateNewStack()
        {
            BGMEditor window = EditorWindow.CreateWindow<BGMEditor>();
            window.titleContent = new GUIContent("正在编辑创建新的BGM");
            window.Init(null);

            return window;
        }
        /// <summary>
        /// 打开面板
        /// </summary>
        /// <param name="bgmStack">要编辑的BGM</param>
        /// <returns></returns>
        public static BGMEditor ShowWindow(BGMStack bgmStack)
        {
            BGMEditor window = EditorWindow.CreateWindow<BGMEditor>();
            window.titleContent = new GUIContent("正在编辑:"+bgmStack.BGMName);
            window.Init(bgmStack);

            return window;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init(BGMStack bgmStack)
        {
            if (bgmStack == null)
            {
                editMode = EditMode.Create;
                curEditStack = null;
                curEditData = new BGMEditData();
                oldEditData = new BGMEditData();
            }
            else
            {
                editMode = EditMode.Edit;
                curEditStack = bgmStack;
                curEditData = new BGMEditData(curEditStack);
                oldEditData = new BGMEditData(curEditStack);
            }

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

        protected override void OnGUI()
        {
            //绘制编辑主界面
            DrawMainEditorGUI();
            
            base.OnGUI();
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
                DrawIntField(ref curEditData.BGMIndex, "BGM id:");
            }
            
            DrawTextField(ref curEditData.BGMName, "BGM名称:");
            DrawObjectField<AudioClip>(ref curEditData.Clip, "BGMClip:");
            DrawSliderField(ref curEditData.Volume, "音量:", 0f, 1f);
            DrawBoolField(ref curEditData.Loop, "循环播放:");
            
            #endregion

            //标志更改
            if (EditorGUI.EndChangeCheck())
            {
                SetEditted();
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
                !AudioLibrary.Instance.CheckBGMIndexValid(curEditStack, curEditData.BGMIndex))
            {
                curEditData.BGMIndex = oldEditData.BGMIndex;
                EditorUtility.DisplayDialog(errorTitle, "BGMid不合理\n自动更正为原先id", "确认");
                
                return true;
            }
            
            //如果音效名称为空也不合理
            if (string.IsNullOrEmpty(curEditData.BGMName))
            {
                curEditData.BGMName = oldEditData.BGMName;
                EditorUtility.DisplayDialog(errorTitle, "音效名称不能为空\n自动更正为原先名称", "确认");
                
                return true;
            }

            //Clip不能为空
            if (curEditData.Clip == null)
            {
                EditorUtility.DisplayDialog(errorTitle, "Clip不能为空", "确认");

                return true;
            }
            //检测clip
            AudioLibrary.Instance.EditorCheckBGMClip(curEditData.Clip);
            //更新Clip下标
            curEditData.ClipIndex = AudioDic.GetBGMClipIndex(curEditData.Clip);
            
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

                //如果是创建模式
                if (editMode == EditMode.Create)
                {
                    //那就新建一个Stack，然后更新id然后切换到编辑模式
                    curEditStack = new BGMStack();
                    curEditStack.BGMIndex = AudioLibrary.MaxBGMIndex + 1;
                    AudioLibrary.MaxBGMIndex++;
                    AudioLibrary.AddBGMStack(curEditStack);
                    editMode = EditMode.Edit;
                }
                
                //保存数据
                curEditData.PasteData(curEditStack);
            }
            
            //清除编辑标记
            ClearEditted();
            
            //保存
            AudioLibrary.Instance.SaveAsset();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        #endregion
        
    }
}

