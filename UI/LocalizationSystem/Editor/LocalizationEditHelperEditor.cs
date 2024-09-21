//****************** 代码文件申明 ***********************
//* 文件：LocalizationEditHelperEditor
//* 作者：wheat
//* 创建时间：2024/09/20 19:22:07 星期五
//* 描述：
//*******************************************************

using UnityEngine;
using KFrame;
using KFrame.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using KFrame.Editor;
using UnityEditor;
using UnityEngine.UI;

namespace KFrame.UI
{
    [CustomEditor(typeof(LocalizationEditHelper))]
    public class LocalizationEditHelperEditor : UnityEditor.Editor
    {
        private LocalizationEditHelper localEditor;

        private void OnEnable()
        {
            if (localEditor == null)
            {
                localEditor = target as LocalizationEditHelper;
            }
        }
        /// <summary>
        /// 获取语言类型的label文本
        /// </summary>
        private static string TryGetLanguageLabel(LanguageType language)
        {
            return EditorGUITool.TryGetEnumLabel<LanguageType>(language);
        }
        /// <summary>
        /// 绘制文本的GUI
        /// </summary>
        private void DrawTextGUI(LocalizationStringDataBase stringDataBase)
        {
            stringDataBase.Text = EditorGUILayout.TextField(
                KGUIHelper.TempContent(TryGetLanguageLabel(stringDataBase.Language)),
                stringDataBase.Text);
        }
        /// <summary>
        /// 绘制文本的GUI
        /// </summary>
        private void DrawImageGUI(LocalizationImageDataBase imageDataBase)
        {
            imageDataBase.Sprite = (Sprite)EditorGUILayout.ObjectField(
                KGUIHelper.TempContent(TryGetLanguageLabel(imageDataBase.Language)),
                imageDataBase.Sprite, typeof(Sprite), false);
        }
        public override void OnInspectorGUI()
        {
            if (localEditor == null)
            {
                base.OnInspectorGUI();
            }
            else
            {
                
                EditorGUILayout.BeginVertical();
                
                EditorGUITool.ShowEnumSelectOption<LanguageType>("当前的语言类型:", localEditor.CurLanguage.ToString(), (x) =>
                {
                    localEditor.CurLanguage = (LanguageType)x;
                    serializedObject.ApplyModifiedProperties();
                });
                
                EditorGUI.BeginChangeCheck();;

                localEditor.Target = (Graphic)EditorGUILayout.ObjectField(KGUIHelper.TempContent("本地化对象:"), localEditor.Target,
                    typeof(Graphic), true);
                
                localEditor.Key = EditorGUILayout.TextField(KGUIHelper.TempContent("本地化Key:"), localEditor.Key);


                if (localEditor.Target != null)
                {
                    GUILayout.Space(10f);

                    EditorGUILayout.BeginHorizontal();                    
                    
                    EditorGUITool.BoldLabelField("本地化配置");
                    
                    GUILayout.Space(25f);
                    
                    if (GUILayout.Button("保存"))
                    {
                        localEditor.SaveData();
                    }

                    if (GUILayout.Button("加载"))
                    {
                        localEditor.LoadData();
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    
                    GUILayout.Space(10f);
                    
                    if (localEditor.DrawImgData)
                    {
                        for (int i = 0; i < localEditor.ImageData.Datas.Count; i++)
                        {
                            DrawImageGUI(localEditor.ImageData.Datas[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < localEditor.StringData.Datas.Count; i++)
                        {
                            DrawTextGUI(localEditor.StringData.Datas[i]);
                        }
                    }
                }

                
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(target);
                }
                
                
                EditorGUILayout.EndVertical();
            }
        }
    }
}

