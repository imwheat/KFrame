//****************** 代码文件申明 ***********************
//* 文件：ReflectionUtility
//* 作者：wheat
//* 创建时间：2024/04/30 11:16:53 星期二
//* 描述：专门用来反射获取UnityEditor的一些方法
//*******************************************************

using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using Object = UnityEngine.Object;

namespace KFrame.Editor
{
    public static class ReflectionUtility
    {
        private static readonly Assembly editorAssembly = typeof(UnityEditor.Editor).Assembly;

        public const BindingFlags allBindings = BindingFlags.Instance |
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        /// <summary>
        /// 获取编辑器的方法
        /// </summary>
        /// <param name="classType">类的名称</param>
        /// <param name="methodName">方法名</param>
        internal static MethodInfo GetEditorMethod(string classType, string methodName, BindingFlags falgs)
        {
            return editorAssembly.GetType(classType).GetMethod(methodName, falgs);
        }
        /// <summary>
        /// 获取SO里面的方法
        /// </summary>
        /// <param name="methodName">方法名称</param>
        internal static MethodInfo GetObjectMethod(string methodName, SerializedObject serializedObject)
        {
            return GetObjectMethod(methodName, serializedObject.targetObjects);
        }

        internal static MethodInfo GetObjectMethod(string methodName, params Object[] targetObjects)
        {
            return GetObjectMethod(methodName, allBindings, targetObjects);
        }

        internal static MethodInfo GetObjectMethod(string methodName, BindingFlags bindingFlags, params Object[] targetObjects)
        {
            if (targetObjects == null || targetObjects.Length == 0)
            {
                return null;
            }

            var targetType = targetObjects[0].GetType();
            var methodInfo = GetObjectMethod(targetType, methodName, bindingFlags);
            if (methodInfo == null && bindingFlags.HasFlag(BindingFlags.NonPublic))
            {
                //如果这个private的方法找不到的话，再从它的基类里面找
                var baseType = targetType.BaseType;
                while (baseType != null)
                {
                    methodInfo = GetObjectMethod(baseType, methodName, bindingFlags);
                    if (methodInfo != null)
                    {
                        break;
                    }

                    baseType = baseType.BaseType;
                }
            }

            return methodInfo;
        }

        internal static MethodInfo GetObjectMethod(Type targetType, string methodName, BindingFlags bindingFlags)
        {
            return targetType.GetMethod(methodName, bindingFlags, null, CallingConventions.Any, new Type[0], null);
        }

        /// <summary>
        /// 尝试反射调用方法
        /// </summary>
        /// <returns>调用成功返回true</returns>
        internal static bool TryInvokeMethod(string methodName, SerializedObject serializedObject)
        {
            var targetObjects = serializedObject.targetObjects;
            var method = GetObjectMethod(methodName, targetObjects);
            if (method == null)
            {
                return false;
            }

            var parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                return false;
            }

            for (var i = 0; i < targetObjects.Length; i++)
            {
                var target = targetObjects[i];
                if (target == null)
                {
                    continue;
                }

                method.Invoke(target, null);
            }

            return true;
        }

    }
}

