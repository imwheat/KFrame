//****************** 代码文件申明 ************************
//* 文件：ValueParse                                       
//* 作者：Koo
//* 创建时间：2023/12/30 17:20:27 星期六
//* 功能：读表时候经常需要用到的自定义的数值解析
//*****************************************************

using System;
using System.Collections;
using System.Collections.Generic;

namespace KFrame.Utilities
{
    public static partial class UtilityTools
    {
        /// <summary>
        /// 检测此类型是否是一个多对象容器
        /// </summary>
        /// <param name="typeFullName">类型的完整名称</param>
        /// <returns></returns>
        public static bool IsMultiTypeContainer(string typeFullName)
        {
            Type type = Type.GetType(typeFullName);

            if (type != null)
            {
                // 检查是否是数组类型
                if (type.IsArray)
                {
                    return true;
                }

                // 检查是否是泛型集合类型
                if (type.IsGenericType)
                {
                    Type genericTypeDefinition = type.GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(List<>) ||
                        genericTypeDefinition == typeof(Dictionary<,>) ||
                        genericTypeDefinition == typeof(HashSet<>))
                    {
                        return true;
                    }
                }

                // 检查是否是非泛型集合类型
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检测此类型是否是一个多对象容器
        /// </summary>
        /// <param name="typeFullName">类型的完整名称</param>
        /// <returns></returns>
        public static bool IsMultiTypeContainer(Type type)
        {
            if (type != null)
            {
                // 检查是否是数组类型
                if (type.IsArray)
                {
                    return true;
                }

                // 检查是否是泛型集合类型
                if (type.IsGenericType)
                {
                    Type genericTypeDefinition = type.GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(List<>) ||
                        genericTypeDefinition == typeof(Dictionary<,>) ||
                        genericTypeDefinition == typeof(HashSet<>))
                    {
                        return true;
                    }
                }

                // 检查是否是非泛型集合类型
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 映射字典池
        /// </summary>
        private static Dictionary<string, string> typeMapping = new Dictionary<string, string>
        {
            {"System.Single", "float"},
            {"System.Single[]", "float[]"},
            {"System.Int32", "int"},
            {"System.Int32[]", "int[]"},
            {"System.Boolean", "bool"}
            // 添加其他映射
        };

        public static string SimplyTypeName(string fullName)
        {
            return typeMapping.TryGetValue(fullName, out var result) ? result : fullName;
        }

        public static string GetFullNameBySimple(string simplyName)
        {
            foreach (var kvp in typeMapping)
            {
                if (kvp.Value == simplyName)
                    return kvp.Key;
            }

            return simplyName;
        }
    }
}