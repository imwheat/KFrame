//****************** 代码文件申明 ***********************
//* 文件：LocalizationCreateEditorWindow
//* 作者：wheat
//* 创建时间：2024/09/22 09:06:45 星期日
//* 描述：创建添加本地化数据的编辑器
//*******************************************************

using UnityEngine;
using KFrame;
using KFrame.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using KFrame.Editor;
using UnityEditor;

namespace KFrame.UI.Editor
{
    public class LocalizationCreateEditorWindow : KEditorWindow
    {
        #region 参数引用

        /// <summary>
        /// 本地化配置的数据
        /// </summary>
        private LocalizationConfig config => LocalizationConfig.Instance;

        #endregion

        #region 编辑参数

        /// <summary>
        /// 保存的key
        /// </summary>
        /// <returns></returns>
        private string saveKey;
        /// <summary>
        /// 所有语言类型
        /// </summary>
        private int[] languages;
        /// <summary>
        /// 语言文本
        /// </summary>
        private Dictionary<int, string> languageTexts;
        /// <summary>
        /// 语言图片
        /// </summary>
        private Dictionary<int, Sprite> languageSprites;

        #endregion
        
        #region GUI显示

        private static class MStyle
        {
            /// <summary>
            /// 空格
            /// </summary>
            internal static readonly float spacing = 5f;
            /// <summary>
            /// 文本高度
            /// </summary>
            internal static readonly float labelHeight = 20f;
        }
        /// <summary>
        /// 选择绘制类型
        /// </summary>
        private enum SelectType
        {
            StringData = 0,
            ImageData = 1,
        }
        /// <summary>
        /// 当前选择的绘制类型
        /// </summary>
        private SelectType curSelectType;

        #endregion

        #region 生命周期

        private void OnEnable()
        {
            Init();
        }

        #endregion
        
        #region 初始化
        

        /// <summary>
        /// 打开窗口
        /// </summary>
        public static void ShowWindow()
        {
            LocalizationCreateEditorWindow window = EditorWindow.GetWindow<LocalizationCreateEditorWindow>();
            window.titleContent = new GUIContent("新建本地化数据");
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            //清空key
            saveKey = "";
            //获取所有语言类型，然后初始化每种语言数据
            languages = LocalizationDic.GetLanguageIdArray();
            languageTexts = new Dictionary<int, string>();
            languageSprites = new Dictionary<int, Sprite>();
            //遍历设置key清空参数
            foreach (var language in languages)
            {
                languageTexts[language] = "";
                languageSprites[language] = null;
            }
        }

        #endregion
        
        #region GUI绘制
        /// <summary>
        /// 切换当前绘制的类型
        /// </summary>
        private void SwitchDrawList(SelectType type)
        {
            curSelectType = type;
            Repaint();
        }
        /// <summary>
        /// 绘制底部GUI
        /// </summary>
        private void DrawBottomGUI()
        {
            EditorGUILayout.BeginVertical();
            
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("切换文本"))
            {
                SwitchDrawList(SelectType.StringData);
            }

            if (GUILayout.Button("切换图片"))
            {
                SwitchDrawList(SelectType.ImageData);
            }

            if (GUILayout.Button("保存"))
            {
                switch (curSelectType)
                {
                    case SelectType.StringData:
                        if (SaveStringData())
                        {
                            //保存后重新初始化GUI
                            Init();
                        }
                        break;
                    case SelectType.ImageData:
                        if (SaveImageData())
                        {
                            //保存后重新初始化GUI
                            Init();
                        }
                        break;
                }

            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制文本数据GUI
        /// </summary>
        private void DrawStringDataGUI()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(MStyle.spacing);
            saveKey = EditorGUILayout.TextField("保存key",saveKey, GUILayout.Height(MStyle.labelHeight));
            GUILayout.Space(MStyle.spacing);
            foreach (var languageId in languages)
            {
                languageTexts[languageId] = EditorGUILayout.TextField(LocalizationDic.GetLanguageName(languageId),languageTexts[languageId], GUILayout.Height(MStyle.labelHeight));
                GUILayout.Space(MStyle.spacing);
            }
            
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制图片数据GUI
        /// </summary>
        private void DrawImageDataGUI()
        {
            EditorGUILayout.BeginVertical();

            GUILayout.Space(MStyle.spacing);
            saveKey = EditorGUILayout.TextField("保存key",saveKey, GUILayout.Height(MStyle.labelHeight));
            GUILayout.Space(MStyle.spacing);
            foreach (var language in languages)
            {
                languageSprites[language] = (Sprite)EditorGUILayout.ObjectField(LocalizationDic.GetLanguageName(language),languageSprites[language], typeof(Sprite), false);
                GUILayout.Space(MStyle.spacing);
            }
            
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// 绘制主要GUI
        /// </summary>
        private void DrawMainGUI()
        {
            //根据类型绘制不同GUI
            switch (curSelectType)
            {
                case SelectType.StringData:
                    DrawStringDataGUI();
                    break;
                case SelectType.ImageData:
                    DrawImageDataGUI();
                    break;
            }
        }
        protected override void OnGUI()
        {
            //绘制主要GUI
            DrawMainGUI();
            
            //绘制底部GUI
            DrawBottomGUI();
            
            base.OnGUI();
        }

        #endregion

        #region 保存操作

        /// <summary>
        /// 检测能不能保存
        /// </summary>
        /// <returns>没问题返回true</returns>
        private bool SaveCheck()
        {
            //key为空不能保存
            if (string.IsNullOrEmpty(saveKey))
            {
                EditorUtility.DisplayDialog("错误", "保存的key不能为空", "确认");
                
                return false;
            }
            //key不能重复
            else if (curSelectType == SelectType.StringData && LocalizationDic.CheckUITextKey(saveKey))
            {
                EditorUtility.DisplayDialog("错误", $"已经有key为{saveKey}的数据存在了", "确认");
                
                return false;
            }
            else if (curSelectType == SelectType.ImageData && LocalizationDic.CheckUIImageKey(saveKey))
            {
                EditorUtility.DisplayDialog("错误", $"已经有key为{saveKey}的数据存在了", "确认");
                
                return false;
            }
            
            return true;
        }
        /// <summary>
        /// 保存文本数据
        /// </summary>
        /// <returns>保存成功返回true</returns>
        private bool SaveStringData()
        {
            //进行保存检测
            if (!SaveCheck())
            {
                return false;
            }
            
            //停止文本编辑
            EditorGUITool.EndEditTextField();

            //新建数据
            LocalizationStringData data = new LocalizationStringData(saveKey);
            foreach (var language in languages)
            {
                data.Datas.Add(new LocalizationStringDataBase(language, languageTexts[language]));
            }
            //然后保存添加
            LocalizationDic.SaveUITextData(data);
            
            return true;
        }
        /// <summary>
        /// 保存图片数据
        /// </summary>
        /// <returns>保存成功返回true</returns>
        private bool SaveImageData()
        {
            //进行保存检测
            if (!SaveCheck())
            {
                return false;
            }

            //新建数据
            LocalizationImageData data = new LocalizationImageData(saveKey);
            foreach (var language in languages)
            {
                data.Datas.Add(new LocalizationImageDataBase(language, languageSprites[language]));
            }
            //然后保存添加
            LocalizationDic.SaveImageData(data);
            
            return true;
        }
        #endregion
    }
}

