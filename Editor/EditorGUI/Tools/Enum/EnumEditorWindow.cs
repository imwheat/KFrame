//****************** 代码文件申明 ************************
//* 文件：EnumEditorWindow                                       
//* 作者：wheat
//* 创建时间：2024/01/25 14:55:43 星期四
//* 描述：一个可以显示枚举类型的编辑器窗口
//*****************************************************
#if UNITY_EDITOR

using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;

namespace KFrame.Editor
{
    public class EnumEditorWindow : EditorWindow
    {

        private bool searchFoldout;
        private string searchText;
        private Vector2 scrollPosition;
        private bool addnewEnumFoldout;
        private string addnewText;
        private Action<int> clickAction;
        private int[] optionValues;
        private string[] selectOptions;
        private GUIContent[] selectUIs;

        /// <summary>
        /// 打开面板
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        public static EnumEditorWindow ShowEnumSelectOption<TEnum>(Action<int> action) where TEnum : struct
        {
            //获取面板
            EnumEditorWindow window = EditorWindow.GetWindow<EnumEditorWindow>();
            //初始化
            window.Initailize<TEnum>(action);

            return window;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Initailize<TEnum>(Action<int> action) where TEnum : struct
        {
            //先获取所有的选项
            TEnum[] enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));

            //生成文本选项
            selectOptions = new string[enumValues.Length];
            selectUIs = new GUIContent[enumValues.Length];
            optionValues = new int[enumValues.Length];
            for (int i = 0; i < enumValues.Length; i++)
            {
                selectOptions[i] = enumValues[i].ToString();
                selectUIs[i] = new GUIContent(EditorGUITool.TryGetEnumLabel<TEnum>(selectOptions[i]));
                optionValues[i] = Convert.ToInt32(enumValues[i]);
            }
            //默认打开搜索栏
            searchFoldout = true;

            //修改显示文本
            this.titleContent = new GUIContent("点击选择一个" + typeof(TEnum).Name);
            //将位置设置到鼠标旁边
            Vector2 mousePosition = Event.current.mousePosition;
            Vector2 screenMousePosition = GUIUtility.GUIToScreenPoint(mousePosition);

            this.position = new Rect(screenMousePosition.x-125f, screenMousePosition.y, 250f, Mathf.Min(600f,100f + selectUIs.Length * 30f));
            clickAction = action;
        }
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                //搜索栏
                searchFoldout = EditorGUILayout.Foldout(searchFoldout, "搜索栏");
                if (searchFoldout)
                {
                    searchText = EditorGUILayout.TextField(searchText, GUILayout.Width(position.width));
                }
                

                //下拉选项框
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                {

                    //遍历每个按钮
                    for (int i = 0; i < selectUIs.Length; i++)
                    {

                        //如果搜索栏不是空的
                        if(string.IsNullOrEmpty(searchText) == false)
                        {
                            //如果搜索内容没有这个那就跳过
                            if (selectOptions[i].Contains(searchText) == false)
                            {
                                continue;
                            }
                        }
                        //显示按钮
                        if (GUILayout.Button(selectUIs[i]))
                        {
                            clickAction?.Invoke(optionValues[i]);
                            Close();
                        }

                    }

                }
                EditorGUILayout.EndScrollView();

                //添加枚举
                addnewEnumFoldout = EditorGUILayout.Foldout(addnewEnumFoldout, "添加新的枚举选项");
                if (addnewEnumFoldout)
                {
                    addnewText = EditorGUILayout.TextField(addnewText, GUILayout.Width(position.width));
                    if (GUILayout.Button("添加", GUILayout.Height(30), GUILayout.Width(position.width)))
                    {
                        Debug.Log("这个功能还没做呢！");
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        
    }
}

#endif