//****************** 代码文件申明 ************************
//* 文件：ShowValueAttributeDraw                                       
//* 作者：Koo
//* 创建时间：2024/03/01 15:28:34 星期五
//* 功能：绘制显示值
//*****************************************************

using System;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using KFrame;
using KFrame.Attributes;

namespace KFrame.AttributeDrawers
{
    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    public class ShowValueAttributeDrawer : OdinAttributeDrawer<ShowValueAttribute>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            // 开始水平布局
            EditorGUILayout.BeginHorizontal();

            // 绘制Label字段
            EditorGUI.LabelField(EditorGUILayout.GetControlRect(), label);

            var value = this.Property.ValueEntry.WeakSmartValue;
            // 判断字段的类型并绘制相应的GUI
            if (value != null)
            {
                Type valueType = value.GetType();

                if (valueType == typeof(int))
                {
                    EditorGUI.IntField(EditorGUILayout.GetControlRect(), label, (int)value);
                }
                else if (valueType == typeof(float))
                {
                    EditorGUI.FloatField(EditorGUILayout.GetControlRect(), label, (float)value);
                }
                else if (valueType == typeof(string))
                {
                    EditorGUI.TextField(EditorGUILayout.GetControlRect(), label, (string)value);
                }
            }


            // 结束水平布局
            EditorGUILayout.EndHorizontal();
        }
    }
}