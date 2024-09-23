//****************** 代码文件申明 ***********************
//* 文件：KEditorGUI
//* 作者：wheat
//* 创建时间：2024/04/28 07:39:47 星期日
//* 描述：用来绘制一些编辑器GUI
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame;
using KFrame.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace KFrame.Editor
{
    public static class KEditorGUI
    {
        #region 字段

        /// <summary>
        /// 所有的flag
        /// </summary>
        public const BindingFlags AllBinding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.IgnoreCase;

        #endregion

        private static bool ignoreNextLetterInSearchField = false;


        #region 绘制字段

        /// <summary>
        /// 绘制字段GUI
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="field">要被绘制的字段</param>
        /// <param name="drawingAction">绘制方法</param>
        internal static void DrawField(System.Object instance, FieldInfo field, Action<System.Object, FieldInfo> drawingAction)
        {
            //如果没有被忽略，那就绘制出来
            if (!IsFieldIgnored(field))
            {
                drawingAction(instance, field);
            }
        }
        /// <summary>
        /// 绘制属性以Unity原本的方式
        /// </summary>
        internal static void DrawNativeFiled(System.Object instance, FieldInfo property)
        {
            DrawNativeFiled(instance, property, new GUIContent(property.Name));
        }
        /// <summary>
        /// 绘制字段以Unity原本的方式
        /// </summary>
        internal static void DrawNativeFiled(System.Object instance, FieldInfo property, GUIContent label)
        {
            //根据类型进行绘制
            System.Object value = property.GetValue(instance);
            switch (Type.GetTypeCode(property.FieldType))
            {
                case TypeCode.Int32:
                    int intValue = EditorGUILayout.IntField(label, (int)value);
                    property.SetValue(instance, intValue);
                    break;
                case TypeCode.String:
                    string stringValue = EditorGUILayout.TextField(label, (string)value);
                    property.SetValue(instance, stringValue);
                    break;
                case TypeCode.Boolean:
                    bool boolValue = EditorGUILayout.Toggle(label, (bool)value);
                    property.SetValue(instance, boolValue);
                    break;
                case TypeCode.Byte:
                    byte byteValue = (byte)EditorGUILayout.IntField(label, (byte)value);
                    property.SetValue(instance, byteValue);
                    break;
                case TypeCode.Char:
                    char charValue = EditorGUILayout.TextField(label, ((char)value).ToString())[0];
                    property.SetValue(instance, charValue);
                    break;
                case TypeCode.Decimal:
                    decimal decimalValue = (decimal)EditorGUILayout.DoubleField(label, (double)(decimal)value);
                    property.SetValue(instance, decimalValue);
                    break;
                case TypeCode.Double:
                    double doubleValue = EditorGUILayout.DoubleField(label, (double)value);
                    property.SetValue(instance, doubleValue);
                    break;
                case TypeCode.Int16:
                    short shortValue = (short)EditorGUILayout.IntField(label, (short)value);
                    property.SetValue(instance, shortValue);
                    break;
                case TypeCode.Int64:
                    long longValue = EditorGUILayout.LongField(label, (long)value);
                    property.SetValue(instance, longValue);
                    break;
                case TypeCode.SByte:
                    sbyte sbyteValue = (sbyte)EditorGUILayout.IntField(label, (sbyte)value);
                    property.SetValue(instance, sbyteValue);
                    break;
                case TypeCode.Single:
                    float floatValue = EditorGUILayout.FloatField(label, (float)value);
                    property.SetValue(instance, floatValue);
                    break;
                case TypeCode.UInt16:
                    ushort ushortValue = (ushort)EditorGUILayout.IntField(label, (ushort)value);
                    property.SetValue(instance, ushortValue);
                    break;
                case TypeCode.UInt32:
                    uint uintValue = (uint)EditorGUILayout.LongField(label, (uint)value);
                    property.SetValue(instance, uintValue);
                    break;
                case TypeCode.Object:
                    // 处理自定义对象类型，你可以根据需要展开
                    EditorGUILayout.LabelField("自定义的Object字段: " + property.FieldType.Name);
                    break;
                default:
                    //不支持剩下的类型不进行绘制
                    break;
            }

        }

        #endregion

        #region 绘制属性

        /// <summary>
        /// 绘制属性GUI
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="property">要被绘制的属性</param>
        /// <param name="drawingAction">绘制方法</param>
        internal static void DrawProperty(System.Object instance, PropertyInfo property, Action<System.Object, PropertyInfo> drawingAction)
        {
            //如果没有被忽略，那就绘制出来
            if (!IsPropertyIgnored(property))
            {
                drawingAction(instance, property);
            }
        }
        /// <summary>
        /// 绘制属性以Unity原本的方式
        /// </summary>
        internal static void DrawNativeProperty(System.Object instance, PropertyInfo property)
        {
            DrawNativeProperty(instance, property, new GUIContent(property.Name));
        }
        /// <summary>
        /// 绘制属性以Unity原本的方式
        /// </summary>
        internal static void DrawNativeProperty(System.Object instance, PropertyInfo property, GUIContent label)
        {
            //根据类型进行绘制
            System.Object value = property.GetValue(instance);
            switch (Type.GetTypeCode(property.PropertyType))
            {
                case TypeCode.Int32:
                    int intValue = EditorGUILayout.IntField(label, (int)value);
                    property.SetValue(instance, intValue);
                    break;
                case TypeCode.String:
                    string stringValue = EditorGUILayout.TextField(label, (string)value);
                    property.SetValue(instance, stringValue);
                    break;
                case TypeCode.Boolean:
                    bool boolValue = EditorGUILayout.Toggle(label, (bool)value);
                    property.SetValue(instance, boolValue);
                    break;
                case TypeCode.Byte:
                    byte byteValue = (byte)EditorGUILayout.IntField(label, (byte)value);
                    property.SetValue(instance, byteValue);
                    break;
                case TypeCode.Char:
                    char charValue = EditorGUILayout.TextField(label, ((char)value).ToString())[0];
                    property.SetValue(instance, charValue);
                    break;
                case TypeCode.Decimal:
                    decimal decimalValue = (decimal)EditorGUILayout.DoubleField(label, (double)(decimal)value);
                    property.SetValue(instance, decimalValue);
                    break;
                case TypeCode.Double:
                    double doubleValue = EditorGUILayout.DoubleField(label, (double)value);
                    property.SetValue(instance, doubleValue);
                    break;
                case TypeCode.Int16:
                    short shortValue = (short)EditorGUILayout.IntField(label, (short)value);
                    property.SetValue(instance, shortValue);
                    break;
                case TypeCode.Int64:
                    long longValue = EditorGUILayout.LongField(label, (long)value);
                    property.SetValue(instance, longValue);
                    break;
                case TypeCode.SByte:
                    sbyte sbyteValue = (sbyte)EditorGUILayout.IntField(label, (sbyte)value);
                    property.SetValue(instance, sbyteValue);
                    break;
                case TypeCode.Single:
                    float floatValue = EditorGUILayout.FloatField(label, (float)value);
                    property.SetValue(instance, floatValue);
                    break;
                case TypeCode.UInt16:
                    ushort ushortValue = (ushort)EditorGUILayout.IntField(label, (ushort)value);
                    property.SetValue(instance, ushortValue);
                    break;
                case TypeCode.UInt32:
                    uint uintValue = (uint)EditorGUILayout.LongField(label, (uint)value);
                    property.SetValue(instance, uintValue);
                    break;
                case TypeCode.Object:
                    // 处理自定义对象类型，你可以根据需要展开
                    EditorGUILayout.LabelField("自定义的Object属性: " + property.PropertyType.Name);
                    break;
                default:
                    //不支持剩下的类型不进行绘制
                    break;
            }

        }

        #endregion

        #region 绘制SO

        /// <summary>
        /// 绘制属性GUI
        /// </summary>
        /// <param name="property">要被绘制的属性</param>
        /// <param name="drawingAction">绘制方法</param>
        internal static void DrawSerializedProperty(SerializedProperty property, Action<SerializedProperty> drawingAction)
        {
            //如果没有被忽略，那就绘制出来
            if (!IsSerializedPropertyIgnored(property))
            {
                drawingAction(property);
            }
        }
        /// <summary>
        /// 绘制属性以Unity原本的方式
        /// </summary>
        internal static void DrawNativeProperty(SerializedProperty property)
        {
            DrawSerializedProperty(property, new GUIContent(property.displayName));
        }
        /// <summary>
        /// 绘制属性以Unity原本的方式
        /// </summary>
        internal static void DrawSerializedProperty(SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.PropertyField(property, label);
        }

        #endregion

        #region 绘制SerializedProperty
        
        /// <summary>
        /// 绘制SerializedProperty
        /// </summary>
        /// <param name="property">要绘制的Property</param>
        /// <param name="label">标签</param>
        public static void PropertyField(SerializedProperty property, string label)
        {
            EditorGUILayout.PropertyField(property, KGUIHelper.TempContent(label));
        }

        #endregion

        #region 绘制GUI工具
        /// <summary>
        /// 这个字段有没有被忽略
        /// </summary>
        /// <returns>如果被忽略了就返回true</returns>
        internal static bool IsFieldIgnored(FieldInfo field)
        {
            //如果有HideInInspector的Attribute那就说明被隐藏了
            if (field.GetCustomAttribute<HideInInspector>() != null) return true;

            return false;
        }
        /// <summary>
        /// 这个字段有没有被忽略
        /// </summary>
        /// <returns>如果被忽略了就返回true</returns>
        internal static bool IsPropertyIgnored(PropertyInfo property)
        {
            //如果有HideInInspector的Attribute那就说明被隐藏了
            if (property.GetCustomAttribute<HideInInspector>() != null) return true;

            return false;
        }
        /// <summary>
        /// 这个字段有没有被忽略
        /// </summary>
        /// <returns>如果被忽略了就返回true</returns>
        internal static bool IsSerializedPropertyIgnored(SerializedProperty property)
        {
            if (property.name == "m_Script") return true;
            
            return false;
        }
        /// <summary>
        /// 从SO里面获取指定的serializedProperty
        /// </summary>
        /// <returns></returns>
        public static SerializedProperty GetSerializedProperty(SerializedObject serializedObject, string propertyPath)
        {
            //获取Property的迭代器
            SerializedProperty tmp = serializedObject.GetIterator();
            SerializedProperty result = null;
            //遍历获取目标的子Property
            while (tmp.NextVisible(true))
            {
                if (tmp.name == propertyPath || (tmp.name[0]=='<'&& tmp.name.Contains(propertyPath) && tmp.name.Contains("k__BackingField")))
                {
                    result = tmp;
                    break;
                }
            }

            //返回结果
            return result;
        }
        /// <summary>
        /// 从SO里面获取指定的serializedProperty包括children
        /// </summary>
        /// <param name="propertyPath">名称</param>
        /// <returns></returns>
        public static List<SerializedProperty> GetSerializedPropertyIncludeChildren(SerializedObject serializedObject, string propertyPath)
        {
            //获取Property的迭代器
            SerializedProperty tmp = serializedObject.GetIterator();
            List<SerializedProperty> result = new List<SerializedProperty>();
            bool find = false; // 是否找到目标了
            int depth = 0; // 目标的深度
            //遍历获取目标的子Property
            while (tmp.NextVisible(true))
            {
                //如果找到了
                if (find)
                {
                    //如果是子Property的Property就跳过
                    if (tmp.depth > depth + 1) continue;
                    //如果当前这个Property的depth和目标一样了
                    //说明到tA已经遍历完了，结束遍历
                    if (tmp.depth == depth) break;

                    //添加到列表
                    result.Add(tmp.Copy());
                }
                //如果找到tA了
                else if (tmp.name == propertyPath)
                {
                    //标记已经找了，并记录目标的深度
                    find = true;
                    depth = tmp.depth;
                }
            }

            //返回结果
            return result;
        }
        /// <summary>
        /// 从SO里面获取指定的SP包,包括children
        /// </summary>
        /// <param name="propertyPath">名称</param>
        /// <returns></returns>
        public static SerializedPropertyPack GetSPPackIncludeChildren(SerializedObject serializedObject, string propertyPath)
        {
            //获取Property的迭代器
            SerializedProperty tmp = serializedObject.GetIterator();
            SerializedPropertyPack result = null;
            Dictionary<int, SerializedPropertyPack> parentDic = new Dictionary<int, SerializedPropertyPack>();
            bool find = string.IsNullOrEmpty(propertyPath); // 是否找到目标了
            int targetDepth = 0; // 目标的深度
            if (find)
            {
                targetDepth = tmp.depth;
                result = new SerializedPropertyPack(tmp.Copy());
            }
            //遍历获取目标的子Property
            while (tmp.NextVisible(true))
            {
                //如果找到了
                if (find)
                {
                    //复制一份当前的SP
                    SerializedPropertyPack copy = new SerializedPropertyPack(tmp.Copy());
                    //如果它有子集
                    if (copy.Property.hasChildren)
                    {
                        //更新当前层级的父级
                        parentDic[copy.Property.depth + 1] = copy;
                    }
                    //如果是子Property的Property就设置父级然后跳过
                    if (tmp.depth > targetDepth + 1)
                    {
                        //更新子父集
                        SerializedPropertyPack parent = parentDic[tmp.depth];
                        copy.Parent = parent;
                        copy.Depth = copy.Parent.Depth + 1;
                        parent.Children.Add(copy);

                        continue;
                    }
                    //如果深度之比目标深度大一
                    else if (tmp.depth == targetDepth +1)
                    {
                        //那么父级就是目标
                        copy.Parent = result;
                        copy.Depth = copy.Parent.Depth + 1;
                    }
                    else if (tmp.depth == targetDepth)
                    {

                        //如果当前这个Property的depth和目标一样了
                        //说明到tA已经遍历完了，结束遍历
                        break;
                    }

                    //添加到列表
                    result.Children.Add(copy);
                }
                //如果找到tA了
                else if (tmp.name == propertyPath)
                {
                    //标记已经找了，并记录目标的深度
                    find = true;
                    targetDepth = tmp.depth;
                    result = new SerializedPropertyPack(tmp.Copy());
                }
            }
            
            //返回结果
            return result;
        }
        
        /// <summary>
        /// 开始Editor垂直分布附带空格
        /// </summary>
        /// <param name="leftSpace">左边空格</param>
        public static void BeginVerticleWithSpace(float leftSpace)
        {
            EditorGUILayout.BeginHorizontal();

            if (leftSpace > 0f)
            {
                GUILayout.Space(leftSpace);
            }

            EditorGUILayout.BeginVertical();
        }
        /// <summary>
        /// 开始Editor垂直分布附带空格
        /// </summary>
        /// <param name="rightSpace">右边空格</param>
        public static void EndVerticleWithSpace(float rightSpace)
        {
            EditorGUILayout.EndVertical();
            
            if (rightSpace > 0f)
            {
                GUILayout.Space(rightSpace);
            }
            
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region 绘制GUI

        /// <summary>
        /// 搜索栏TextField
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="searchText"></param>
        /// <param name="forceFocus"></param>
        /// <param name="controlName"></param>
        /// <returns></returns>
        public static string SearchTextField(Rect rect, string searchText, bool forceFocus = false, string controlName = "KSearchField")
        {
            //调整Rect
            rect = rect.AlignTop(16f);
            rect.width -= 16f;
            searchText = searchText ?? "";

            //判断当前focus的是不是当前搜索栏，判断要不要忽略下一个输入
            bool flag = GUI.GetNameOfFocusedControl() == controlName;
            bool flag2 = flag && Event.current.type == EventType.Used;
            if (flag2)
            {
                ignoreNextLetterInSearchField = true;
            }
            else if (ignoreNextLetterInSearchField && flag)
            {
                Event.current.character = '\0';
                ignoreNextLetterInSearchField = false;
            }

            //更新GUI现在的Control
            GUI.SetNextControlName(controlName);

            //显示搜索栏
            string text = EditorGUI.TextField(rect, searchText, KGUIStyles.ToolbarSearchTextField);
            if (!flag2)
            {
                searchText = text;
            }

            //如果搜索栏为空，那就显示搜索栏标签
            if (Event.current.type == EventType.Repaint && string.IsNullOrEmpty(searchText))
            {
                float y = 2f;
                if (UnityVersion.IsVersionOrGreater(2019, 3))
                {
                    y = 0f;
                }

                GUI.Label(rect.AddXMin(14f).SubY(y), KGUIHelper.TempContent("搜索栏"), KGUIStyles.LeftAlignedGreyMiniLabel);
            }

            //如果强制focus，那就focus
            if (forceFocus)
            {
                EditorGUI.FocusTextInControl(controlName);
            }

            //显示清空搜索栏按钮
            rect.x += rect.width;
            rect.width = 16f;
            if (GUI.Button(rect, GUIContent.none, KGUIStyles.ToolbarSearchCancelButton))
            {
                searchText = "";
                KGUIHelper.RemoveFocusControl();
                GUI.changed = true;
            }

            return searchText;
        }
        /// <summary>
        /// 绘制竖的滚轮
        /// </summary>
        /// <param name="areaRect">绘制Scroll的区域</param>
        /// <param name="scrollPosition">当前滚轮位置</param>
        /// <param name="height">总高度</param>
        public static float DrawScrollVertical(ref Rect areaRect, float scrollPosition, float height)
        {
            //如果目前可以显示的区域大于等于请求的那就直接返回原数据
            if (areaRect.height >= height) return scrollPosition;

            //获取scrollbar的高度
            float maxHeight = height - areaRect.height;

            //控制滚动
            if (Event.current.type == EventType.ScrollWheel && areaRect.Contains(Event.current.mousePosition))
            {
                scrollPosition += Event.current.delta.y * (maxHeight/10f);
                Event.current.Use();
            }

            //绘制滚动条
            scrollPosition = GUI.VerticalScrollbar(areaRect.AlignRight(16f), scrollPosition, 1f, 0f, maxHeight);
            areaRect.xMax -= 16f;

            return scrollPosition;
        }

        #endregion

    }
}

