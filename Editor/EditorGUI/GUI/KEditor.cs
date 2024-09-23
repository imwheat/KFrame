//****************** 代码文件申明 ***********************
//* 文件：KEditor
//* 作者：wheat
//* 创建时间：2024/05/01 13:07:20 星期三
//* 描述：重写Unity的一些编辑器绘制
//*******************************************************

using UnityEngine;
using UnityEditor;
using KFrame;
using KFrame.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KFrame.Editor
{
    [InitializeOnLoad]
    public static class KEditor
    {
        private static Dictionary<SerializedObject, Dictionary<List<SerializedProperty>, KEditorDrawer>> m_DrawerDic;
        private static Dictionary<SerializedObject, Dictionary<SerializedPropertyPack, KEditorDrawer>> m_DrawerPackDic;
        static KEditor()
        {
            m_DrawerDic = new Dictionary<SerializedObject, Dictionary<List<SerializedProperty>, KEditorDrawer>>();
            m_DrawerPackDic = new Dictionary<SerializedObject, Dictionary<SerializedPropertyPack, KEditorDrawer>>();
        }
        /// <summary>
        /// 绘制指定的Properties
        /// </summary>
        public static void DrawProperties(SerializedObject serializedObject, List<SerializedProperty> serializedProperties)
        {
            //获取对应Drawer然后开始绘制
            GetDrawer(serializedObject, serializedProperties).DrawEditorGUI();
        }
        /// <summary>
        /// 绘制指定的Properties
        /// </summary>
        public static void DrawProperties(SerializedObject serializedObject, SerializedPropertyPack pack)
        {
            if (pack == null)
            {
                Debug.LogWarning("无法绘制pack为空");
                return;
            }
            //获取对应Drawer然后开始绘制
            GetDrawer(serializedObject, pack).DrawEditorGUI();
        }
        /// <summary>
        /// 绘制指定的Properties
        /// </summary>
        public static void DrawProperties(SerializedObject serializedObject, SerializedPropertyPack pack, Rect rect)
        {
            if(pack == null)
            {
                Debug.LogWarning("无法绘制pack为空");
                return;
            }
            //获取对应Drawer然后开始绘制
            GetDrawer(serializedObject, pack).DrawEditorGUI(rect);
        }
        /// <summary>
        /// 获取Drawer如果没有的话就创建
        /// </summary>
        internal static KEditorDrawer GetDrawer(SerializedObject serializedObject, List<SerializedProperty> serializedProperties)
        {
            if(m_DrawerDic.ContainsKey(serializedObject) == false)
            {
                m_DrawerDic[serializedObject] = new Dictionary<List<SerializedProperty>, KEditorDrawer>();
                m_DrawerDic[serializedObject][serializedProperties] = new KEditorDrawer(serializedObject, serializedProperties);
            }
            else if (m_DrawerDic[serializedObject].ContainsKey(serializedProperties) == false)
            {
                m_DrawerDic[serializedObject][serializedProperties] = new KEditorDrawer(serializedObject, serializedProperties);
            }

            return m_DrawerDic[serializedObject][serializedProperties];
        }
        /// <summary>
        /// 获取Drawer如果没有的话就创建
        /// </summary>
        internal static KEditorDrawer GetDrawer(SerializedObject serializedObject, SerializedPropertyPack pack)
        {
            if (m_DrawerPackDic.ContainsKey(serializedObject) == false)
            {
                m_DrawerPackDic[serializedObject] = new Dictionary<SerializedPropertyPack, KEditorDrawer>();
                m_DrawerPackDic[serializedObject][pack] = new KEditorDrawer(serializedObject, pack);
            }
            else if (m_DrawerPackDic[serializedObject].ContainsKey(pack) == false)
            {
                m_DrawerPackDic[serializedObject][pack] = new KEditorDrawer(serializedObject, pack);
            }

            return m_DrawerPackDic[serializedObject][pack];
        }
        /// <summary>
        /// 测试用，清理缓存
        /// </summary>
        private static void ClearCache()
        {
            foreach(var v in m_DrawerDic.Values)
            {
                v.Clear();
            }
            foreach (var v in m_DrawerPackDic.Values)
            {
                v.Clear();
            }
            m_DrawerDic.Clear();
            m_DrawerPackDic.Clear();
        }
    }
}

