//****************** 代码文件申明 ***********************
//* 文件：PropertyUtility
//* 作者：wheat
//* 创建时间：2024/04/27 18:43:33 星期六
//* 描述：
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
    public static class PropertyUtility
    {
        /// <summary>
        /// 从SerializedProperty里获取FiledInfo的方法
        /// </summary>
        private static readonly MethodInfo getGetFieldInfoFromPropertyMethod =
            ReflectionUtility.GetEditorMethod("UnityEditor.ScriptAttributeUtility", "GetFieldInfoFromProperty",
                BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// 判断是不是默认的脚本属性
        /// </summary>
        public static bool IsDefaultScriptProperty(SerializedProperty property)
        {
            return IsDefaultScriptPropertyByPath(property.propertyPath);
        }
        /// <summary>
        /// 判断是不是默认的脚本属性
        /// </summary>
        public static bool IsDefaultScriptPropertyByPath(string propertyPath)
        {
            return propertyPath == "m_Script";
        }
        /// <summary>
        /// 反射获取SerializedProperty的Type和FiledInfo
        /// </summary>
        public static FieldInfo GetFieldInfo(this SerializedProperty property, out Type propertyType)
        {
            //如果为空直接返回空
            if(property == null)
            {
                propertyType = null;
                return null;
            }

            var parameters = new object[] { property, null };
            var result = getGetFieldInfoFromPropertyMethod.Invoke(null, parameters) as FieldInfo;
            propertyType = parameters[1] as Type;
            return result;
        }
    }
}

