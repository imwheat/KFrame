//****************** 代码文件申明 ***********************
//* 文件：SerializedPropertyPack
//* 作者：wheat
//* 创建时间：2024/05/02 20:05:05 星期四
//* 描述：一个包含深度和子关系的SerializedProperty包
//*******************************************************

using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using KFrame.Attributes;

namespace KFrame.Editor
{
    public class SerializedPropertyPack : IDisposable
    {

        /// <summary>
        /// 深度
        /// </summary>
        public int Depth;
        /// <summary>
        /// 当前这个SerializedProperty
        /// </summary>
        public SerializedProperty Property;
        /// <summary>
        /// 它的父级
        /// </summary>
        public SerializedPropertyPack Parent;
        /// <summary>
        /// 它的子集
        /// </summary>
        public List<SerializedPropertyPack> Children;
        /// <summary>
        /// 它的方法
        /// </summary>
        public List<MethodInfo> Methods;
        public SerializedPropertyPack(SerializedProperty property)
        {
            Property = property;
            Children = new List<SerializedPropertyPack>();
            Methods = new List<MethodInfo>();

            //获取方法
            PropertyUtility.GetFieldInfo(property, out Type type);
            if(type != null)
            {
                //获取有按钮特性的方法
                var tmpMethods = type.GetMethods(KEditorGUI.AllBinding);
                foreach (var method in tmpMethods)
                {
                    if(method.GetCustomAttribute<KButtonAttribute>()  != null)
                    {
                        Methods.Add(method);
                    }
                }
            }
        }

        public void Dispose()
        {
            Children.Clear();
            Children = null;
            Parent = null;
            Property = null;
        }
    }
}

