//****************** 代码文件申明 ***********************
//* 文件：KEditorDrawer
//* 作者：wheat
//* 创建时间：2024/04/30 18:54:08 星期二
//* 描述：绘制EditorGUI
//*******************************************************

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using KFrame.Attributes;
using System.Diagnostics;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace KFrame.Editor
{
    public class KEditorDrawer
    {
        #region 参数字段

        /// <summary>
        /// 目标的SerializedObject
        /// </summary>
        private SerializedObject serializedObject;
        /// <summary>
        /// 所有的SerializedProperty
        /// </summary>
        private List<KSerializedProperty> serializedProperties;
        /// <summary>
        /// TabGroup列表
        /// </summary>
        private List<KTabGroupDrawer> tabs;
        /// <summary>
        /// FoldoutGroup列表
        /// </summary>
        private List<KFoldoutGroupDrawer> foldouts;
        /// <summary>
        /// 剩余的SerializedProperty
        /// </summary>
        private List<KSerializedProperty> otherSerializedProperties;

        #endregion

        #region 缓存字段
        /// <summary>
        /// 暂时的Group的字典
        /// </summary>
        private Dictionary<string, KTabGroup> tmpTabGroupDic;
        /// <summary>
        /// 暂时的FoldoutGroup的字典
        /// </summary>
        private Dictionary<string, KFoldoutGroup> tmpFoldoutGroupDic;

        #endregion

        #region 构造
        public KEditorDrawer(SerializedObject serializedObject, List<SerializedProperty> serializedProperties, bool buildImmediate = true)
        {
            this.serializedObject = serializedObject;
            this.tabs = new List<KTabGroupDrawer>();
            this.foldouts = new List<KFoldoutGroupDrawer>();
            this.otherSerializedProperties = new List<KSerializedProperty>();
            this.tmpTabGroupDic = new Dictionary<string, KTabGroup>();
            this.tmpFoldoutGroupDic = new Dictionary<string, KFoldoutGroup>();
            this.serializedProperties = new List<KSerializedProperty>();
            if(serializedProperties != null)
            {
                foreach (var property in serializedProperties)
                {
                    this.serializedProperties.Add(new KSerializedProperty(property, this));
                }
            }


            //处理初始化数据
            if(buildImmediate)
            {
                ProcessBuiltInData();
            }
        }
        public KEditorDrawer(SerializedObject serializedObject, SerializedPropertyPack serializedPack) : this(serializedObject, null, false)
        {
            //初始化每个Property
            foreach (var pack in serializedPack.Children)
            {
                //如果不符合那就跳过
                if(KEditorGUI.IsSerializedPropertyIgnored(pack.Property)) continue;
                this.serializedProperties.Add(new KSerializedProperty(pack, this));
            }

            //处理初始化数据
            ProcessBuiltInData();
        }
        /// <summary>
        /// 处理Layout分类
        /// </summary>
        private void ProcessLayoutData()
        {
            //创建临时TabGroup列表
            //todo :这边之后会改成多个分组TabGroup之后再弄
            List<KTabGroup> tabGroups = new List<KTabGroup>();
            if(tmpTabGroupDic.Count>0)
            {
                tabGroups.AddRange(tmpTabGroupDic.Values);
                tabs.Add(new KTabGroupDrawer(tabGroups));
            }
            List<KFoldoutGroup> foldoutGroups = new List<KFoldoutGroup>();
            if (tmpFoldoutGroupDic.Count > 0)
            {
                foldoutGroups.AddRange(tmpFoldoutGroupDic.Values);
                foldouts.Add(new KFoldoutGroupDrawer(foldoutGroups));
            }

            //把在其他不在Group里面的SerializedProperty放在一个list里
            HashSet<SerializedProperty> inGroupSP = new HashSet<SerializedProperty>();
            foreach (var tab in tabGroups)
            {
                foreach (var sp in tab.SerializedProperties)
                {
                    inGroupSP.Add(sp);
                }
            }
            foreach (var foldout in foldoutGroups)
            {
                foreach (var sp in foldout.SerializedProperties)
                {
                    inGroupSP.Add(sp);
                }
            }
            foreach (var sp in serializedProperties)
            {
                if (inGroupSP.Contains(sp) == false)
                {
                    otherSerializedProperties.Add(sp);
                }
            }

            //清理缓存
            tmpFoldoutGroupDic.Clear();
            tmpTabGroupDic.Clear();
            tmpTabGroupDic = null;
            tmpFoldoutGroupDic = null;
        }
        /// <summary>
        /// 处理剩余的构造数据
        /// </summary>
        private void ProcessBuiltInData()
        {
            //处理分类信息
            ProcessLayoutData();
        }
        /// <summary>
        /// 在构造的时候使用，查找TabGroup
        /// </summary>
        /// <param name="groupName">分组名称</param>
        /// <param name="order">分组的优先度</param>
        internal KTabGroup FindTabGroup(string groupName, int order)
        {
            //如果已经构造完成，字典被清理了，就返回空
            if(tmpTabGroupDic == null)
            {
                return null;
            }

            if(tmpTabGroupDic.ContainsKey(groupName))
            {
                return tmpTabGroupDic[groupName];
            }
            else
            {
                return tmpTabGroupDic[groupName] = new KTabGroup(order, groupName); 
            }
        }
        /// <summary>
        /// 在构造的时候使用，查找FoldoutGroup
        /// </summary>
        /// <param name="groupName">分组名称</param>
        /// <param name="order">分组的优先度</param>
        internal KFoldoutGroup FindFoldoutGroup(string groupName, int order)
        {
            //如果已经构造完成，字典被清理了，就返回空
            if (tmpFoldoutGroupDic == null)
            {
                return null;
            }

            if (tmpFoldoutGroupDic.ContainsKey(groupName))
            {
                return tmpFoldoutGroupDic[groupName];
            }
            else
            {
                return tmpFoldoutGroupDic[groupName] = new KFoldoutGroup(order, groupName);
            }
        }

        #endregion

        #region 绘制

        /// <summary>
        /// 绘制Editor
        /// </summary>
        public void DrawEditor()
        {

        }
        /// <summary>
        /// 绘制EditorGUI
        /// </summary>
        public void DrawEditorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            //绘制TabGroup
            foreach (var tab in tabs)
            {
                tab.DoTabGroup();
            }

            //绘制FoldoutGroup
            foreach (var foldout in foldouts)
            {
                foldout.DoFoldoutGroup();
            }

            //绘制剩余Property
            foreach (var property in otherSerializedProperties)
            {
                property.DoProperty();
            }

            serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 绘制EditorGUI
        /// </summary>
        /// <param name="rect">指定区域</param>
        public void DrawEditorGUI(Rect rect)
        {
            serializedObject.UpdateIfRequiredOrScript();

            //绘制TabGroup
            foreach (var tab in tabs)
            {
                tab.DoTabGroup(rect);
                rect.y += tab.GetTabGroupHeight();
            }

            //绘制FoldoutGroup
            foreach (var foldout in foldouts)
            {
                foldout.DoFoldoutGroup(rect);
                rect.y += foldout.GetGroupHeight();
            }

            //绘制剩余Property
            foreach (var property in otherSerializedProperties)
            {
                rect.height = property.GetPropertyHeight();
                property.DrawProperty(rect);
                rect.y += rect.height;
                rect.y += KGUIStyle.padding;
            }

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 参数获取

        /// <summary>
        /// 获取Drawer的rect高度
        /// </summary>
        /// <returns></returns>
        internal float GetDrawerHeight()
        {
            float tabHeight = 0f;
            foreach (var tab in tabs)
            {
                tabHeight += tab.GetTabGroupHeight();
            }
            float foldoutHeight = 0f;

            float othersHeight = 0f;

            foreach (var sp in otherSerializedProperties)
            {
                othersHeight += sp.GetPropertyHeight(true);
            }

            return tabHeight + foldoutHeight + othersHeight;
        }

        #endregion

    }
}

