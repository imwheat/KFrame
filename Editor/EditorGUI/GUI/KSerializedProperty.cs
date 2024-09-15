//****************** 代码文件申明 ***********************
//* 文件：KSerializedProperty
//* 作者：wheat
//* 创建时间：2024/05/01 19:37:42 星期三
//* 描述：用于KGUI处理的SerializedProperty
//*******************************************************

using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using KFrame.Attributes;
using UnityEngine.UIElements;
using Codice.Client.BaseCommands.BranchExplorer;

namespace KFrame.Editor
{
    public class KSerializedProperty
    {
        /// <summary>
        /// 绑定的Drawer
        /// </summary>
        private readonly KEditorDrawer drawer;
        /// <summary>
        /// 绑定的SerializedObject
        /// </summary>
        private SerializedObject serializedObject => SerializedProperty.serializedObject;
        /// <summary>
        /// SerializedProperty包含了所有需要的信息
        /// </summary>
        public readonly SerializedProperty SerializedProperty;
        /// <summary>
        /// SerializedProperty的字段信息 <see cref="SerializedProperty"/>.
        /// </summary>
        public readonly FieldInfo FieldInfo;
        private Action<Rect> drawPropertyAction;
        /// <summary>
        /// SerializedProperty的类型<see cref="SerializedProperty"/>.
        /// </summary>
        private readonly Type type;
        /// <summary>
        /// 判断是否为List/Array
        /// </summary>
        private readonly bool isArray;
        /// <summary>
        /// 判断这是否一个子Property
        /// </summary>
        private readonly bool isChild;
        /// <summary>
        /// 判断这是否一个子ArrayProperty
        /// </summary>
        private readonly bool isArrayProperty;
        /// <summary>
        /// 子集
        /// </summary>
        private readonly SerializedPropertyPack children;
        /// <summary>
        /// Property显示的Label
        /// </summary>
        private readonly GUIContent label;
        /// <summary>
        /// 用于KGUI处理的SerializedProperty
        /// </summary>
        /// <param name="serializedProperty">绑定的serializedProperty</param>
        /// <param name="drawer">绑定的Drawer</param>
        /// <param name="processBuildImmediate">是否立刻build处理信息</param>
        public KSerializedProperty(SerializedProperty serializedProperty, KEditorDrawer drawer, bool processBuildImmediate = true)
        {
            this.drawer = drawer;
            this.SerializedProperty = serializedProperty;

            //新建Label标签，默认显示displayName
            label = new GUIContent(serializedProperty.displayName);

            //获取字段信息和类型，如果无法获取那就返回
            if ((FieldInfo = serializedProperty.GetFieldInfo(out type)) == null)
            {
                return;
            }
            
            //子集为空
            children = null;
            //判断是否为数组
            isArray = serializedProperty.isArray && serializedProperty.propertyType == SerializedPropertyType.Generic;
            //判断是否为子Property，如果字段名称和Property名称对不上那就是子Property
            isChild = serializedProperty.name != FieldInfo.Name;


            //处理信息
            if(processBuildImmediate)
            {
                ProcessBuiltInData();
                ProcessKGUIData();
            }

        }
        /// <summary>
        /// 用于KGUI处理的SerializedProperty
        /// </summary>
        /// <param name="pack">绑定的serializedPropertyPack</param>
        /// <param name="drawer">绑定的Drawer</param>
        public KSerializedProperty(SerializedPropertyPack pack, KEditorDrawer drawer) : this(pack.Property, drawer, false)
        {
            //设置子集
            children = pack;
            //判断是不是Array的子Property
            isArrayProperty = children.Parent != null && children.Parent.Property.isArray;

            if (FieldInfo == null)
            {
                return;
            }
            
            //处理信息
            ProcessBuiltInData();
            ProcessKGUIData();
        }
        private void ProcessBuiltInData()
        {
            if (isArrayProperty) return;

            var attributes = FieldInfo.GetCustomAttributes<PropertyAttribute>();
            foreach (var attribute in attributes)
            {
                HandleNewAttribute(attribute);
            }

            //判断绘制方式
            //如果没有绑定的children包
            if (children == null || isArray)
            {
                //那就按unity自身绘制
                drawPropertyAction = DrawDefaultProperty;
            }
            //如果有的话
            else
            {
                drawPropertyAction = DrawDefaultWithChildren;
            }
        }

        /// <summary>
        /// 处理所有KGUI相关的特性
        /// </summary>
        private void ProcessKGUIData()
        {
            if (isArrayProperty) return;

            //获取所有特性，然后遍历处理
            var attributes = FieldInfo.GetCustomAttributes<KGUIAttribute>();
            foreach (var attribute in attributes)
            {
                HandleNewAttribute(attribute);
            }

        }
        /// <summary>
        /// 处理新的特性
        /// </summary>
        private void HandleNewAttribute(PropertyAttribute attribute)
        {
            switch (attribute)
            {
                case TooltipAttribute a:
                    label.tooltip = a.tooltip;
                    return;
            }
        }
        /// <summary>
        /// 处理新的特性
        /// </summary>
        private void HandleNewAttribute(KGUIAttribute attribute)
        {
            switch (attribute)
            {
                case KLabelTextAttribute label:
                    this.label.text = label.Text;
                    return;
                case KTabGroupAttribute tab:
                    TryAssignTabGroupAttribute(tab);
                    break;
                case KFoldoutGroupAttribute foldout:
                    TryAssignFoldoutGroupAttribute(foldout);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 创建绘制Property
        /// </summary>
        public void DoProperty()
        {
            Rect rect = GUILayoutUtility.GetRect(0, GetPropertyHeight(), GUILayout.ExpandWidth(true));
            DrawProperty(rect);
        }
        /// <summary>
        /// 绘制Property
        /// </summary>
        public void DrawProperty(Rect rect)
        {
            drawPropertyAction?.Invoke(rect);
        }
        /// <summary>
        /// 默认的绘制方法
        /// </summary>
        /// <param name="rect"></param>
        private void DrawDefaultProperty(Rect rect)
        {
            EditorGUI.PropertyField(rect, SerializedProperty, label, true);
        }
        /// <summary>
        /// 以默认方式绘制包括子集
        /// </summary>
        /// <param name="rect"></param>
        private void DrawDefaultWithChildren(Rect rect)
        {
            //那就进行KEditor绘制
            EditorGUI.PropertyField(rect, SerializedProperty, label, false);

            //获取包含和不包含改元素children的高度
            float elementHeight = EditorGUI.GetPropertyHeight(SerializedProperty, false);
            float childrenHeight = EditorGUI.GetPropertyHeight(SerializedProperty, true);
            //判断有没有折叠起来
            bool foldout = elementHeight == childrenHeight;
            //如果折叠起来了，那就不绘制children
            if (foldout == false)
            {
                //更新rect
                rect.y += elementHeight;
                rect.height = childrenHeight - elementHeight;
                rect.xMin += KGUIStyle.propertyChildPadding;
                rect.xMax -= KGUIStyle.propertyChildPadding;
                KEditor.DrawProperties(serializedObject, children, rect);
            }
        }
        /// <summary>
        /// 根据当前状态获取GUI的高度
        /// </summary>
        /// <returns></returns>
        public float GetPropertyHeight()
        {
            //如果有子集，并且是展开的
            if(SerializedProperty.hasChildren && SerializedProperty.isExpanded)
            {
                //那就获取包括自己的高度
                return GetPropertyHeight(true);
            }
            else
            {
                //否则就获取自身高度
                return GetPropertyHeight(false);
            }
        }
        /// <summary>
        /// 获取GUI的高度
        /// </summary>
        /// <param name="includeChildren">是否包括子集</param>
        public float GetPropertyHeight(bool includeChildren)
        {
            //如果没有绑定的children包
            if (children == null || isArray)
            {
                //直接获取自身Property高度
                return EditorGUI.GetPropertyHeight(SerializedProperty, includeChildren);
            }
            //如果有的话，且要获取包括子集的高度
            else if (includeChildren)
            {
                //获取包含和不包含改元素children的高度
                float elementHeight = EditorGUI.GetPropertyHeight(SerializedProperty, false);
                float childrenHeight = EditorGUI.GetPropertyHeight(SerializedProperty, true);
                //判断有没有折叠起来
                bool foldout = elementHeight == childrenHeight;
                //如果折叠起来了，那就不绘制children
                if (foldout == false)
                {
                    KEditorDrawer drawer = KEditor.GetDrawer(serializedObject, children);
                    float drawerHeight = childrenHeight - elementHeight;
                    if (drawer != null)
                    {
                        drawerHeight = drawer.GetDrawerHeight();
                    }

                    return elementHeight + drawerHeight;
                }

                return elementHeight;
            }
            //获取自身高度
            else
            {
                //直接返回自身Property高度
                return EditorGUI.GetPropertyHeight(SerializedProperty, false);
            }

        }

        #region 运算符

        /// <summary>
        /// 隐式获取SerializedProperty
        /// </summary>
        public static implicit operator SerializedProperty(KSerializedProperty property)
        {
            return property.SerializedProperty;
        }

        #endregion

        #region 配置特性

        /// <summary>
        /// 处理TabGroup的特性
        /// </summary>
        /// <returns>如果失败了返回false</returns>
        private bool TryAssignTabGroupAttribute(KTabGroupAttribute attribute)
        {
            //获取Group
            KTabGroup group = drawer.FindTabGroup(attribute.GroupName, attribute.Order);

            //把Property加入到这个TabGroup里面
            group.SerializedProperties.Add(this);

            return true;
        }
        /// <summary>
        /// 处理FoldoutGroup的特性
        /// </summary>
        /// <returns>如果失败了返回false</returns>
        private bool TryAssignFoldoutGroupAttribute(KFoldoutGroupAttribute attribute)
        {
            //获取Group
            KFoldoutGroup group = drawer.FindFoldoutGroup(attribute.GroupName, attribute.Order);

            //把Property加入到这个TabGroup里面
            group.SerializedProperties.Add(this);

            return true;
        }
        #endregion
    }
}

