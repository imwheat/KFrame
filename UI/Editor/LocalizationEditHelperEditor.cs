//****************** 代码文件申明 ***********************
//* 文件：LocalizationEditHelperEditor
//* 作者：wheat
//* 创建时间：2024/09/20 19:22:07 星期五
//* 描述：
//*******************************************************

using UnityEngine;
using KFrame;
using KFrame.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using KFrame.Editor;
using UnityEditor;
using UnityEngine.UI;

namespace KFrame.UI.Editor
{
    [CustomEditor(typeof(LocalizationEditHelper))]
    public class LocalizationEditHelperEditor : UnityEditor.Editor
    {
        private LocalizationEditHelper localEditor;
        private static int[] languageTypeValues;
        private static string[] languageDisplays;

        private void OnEnable()
        {
            if (localEditor == null)
            {
                localEditor = target as LocalizationEditHelper;
                localEditor?.RefreshLinkedData();
            }

            if (languageDisplays == null)
            {
                LocalizationConfig config = LocalizationConfig.Instance;
                languageDisplays = new string[config.packageReferences.Count];
                languageTypeValues = new int[config.packageReferences.Count];
                for (int i = 0; i < config.packageReferences.Count; i++)
                {
                    languageDisplays[i] = config.packageReferences[i].LanguageName;
                    languageTypeValues[i] = config.packageReferences[i].LanguageId;
                }
            }
        }
        /// <summary>
        /// 绘制文本的GUI
        /// </summary>
        private void DrawTextGUI(LocalizationStringDataBase stringDataBase)
        {
            stringDataBase.Text = EditorGUILayout.TextField(
                KGUIHelper.TempContent(LocalizationDic.GetLanguageName(stringDataBase.LanguageId)),
                stringDataBase.Text);
        }
        /// <summary>
        /// 绘制文本的GUI
        /// </summary>
        private void DrawImageGUI(LocalizationImageDataBase imageDataBase)
        {
            imageDataBase.Sprite = (Sprite)EditorGUILayout.ObjectField(
                KGUIHelper.TempContent(LocalizationDic.GetLanguageName(imageDataBase.LanguageId)),
                imageDataBase.Sprite, typeof(Sprite), false);
        }
        public override void OnInspectorGUI()
        {
            if (localEditor == null)
            {
                localEditor = target as LocalizationEditHelper;
            }
            
            if (localEditor == null)
            {
                base.OnInspectorGUI();
            }
            else
            {
                
                EditorGUILayout.BeginVertical();
                
                EditorGUI.BeginChangeCheck();;
                
                localEditor.CurLanguage = EditorGUILayout.IntPopup("当前语言类型:", localEditor.CurLanguage, languageDisplays,
                    languageTypeValues);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(target);
                    localEditor.UpdateLanguage(localEditor.CurLanguage);
                }
                
                EditorGUI.BeginChangeCheck();;

                localEditor.Target = (Graphic)EditorGUILayout.ObjectField(KGUIHelper.TempContent("本地化对象:"), localEditor.Target,
                    typeof(Graphic), true);

                EditorGUILayout.BeginHorizontal();
                
                localEditor.Key = EditorGUILayout.TextField(KGUIHelper.TempContent("本地化Key:"), localEditor.Key);

                //从库里选择一个数据然后更新UI
                if (GUILayout.Button("选择", GUILayout.Width(60f)))
                {
                    if (localEditor.DrawImgData)
                    {
                        LocalizationEditorWindow.ShowWindowAsImageSelector((data) =>
                        {
                            localEditor.Key = data.Key;
                            localEditor.ImageData.CopyData(data as LocalizationImageData);
                            localEditor.UpdateUI();
                            serializedObject.ApplyModifiedProperties();
                        });
                    }
                    else
                    {
                        LocalizationEditorWindow.ShowWindowAsStringSelector((data) =>
                        {
                            localEditor.Key = data.Key;
                            localEditor.StringData.CopyData(data as LocalizationStringData);
                            localEditor.UpdateUI();
                            serializedObject.ApplyModifiedProperties();
                        });
                    }
                }
                
                EditorGUILayout.EndHorizontal();
                

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

