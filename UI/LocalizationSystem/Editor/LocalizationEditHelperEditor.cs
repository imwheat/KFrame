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

        public override void OnInspectorGUI()
        {
            if (localEditor == null)
            {
                base.OnInspectorGUI();
            }
            else
            {
                serializedObject.Update();
                
                EditorGUILayout.BeginVertical();
                
                EditorGUITool.ShowEnumSelectOption<LanguageType>("当前的语言类型:", localEditor.CurLanguage.ToString(), (x) =>
                {
                    localEditor.CurLanguage = (LanguageType)x;
                    serializedObject.ApplyModifiedProperties();
                });
                
                EditorGUI.BeginChangeCheck();;

                localEditor.Key = EditorGUILayout.TextField(KGUIHelper.TempContent("本地化Key:"), localEditor.Key);
                localEditor.Target = (Graphic)EditorGUILayout.ObjectField(KGUIHelper.TempContent("本地化对象:"), localEditor.Target,
                    typeof(Graphic), true);
                
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }

                if (localEditor.DrawImgData)
                {
                    
                }
                else
                {
                    
                }
                
                
                EditorGUILayout.EndVertical();
            }
        }
    }
}

