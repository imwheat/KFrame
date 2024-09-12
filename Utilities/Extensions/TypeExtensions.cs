//****************** 代码文件申明 ***********************
//* 文件：TypeExtensions
//* 作者：wheat
//* 创建时间：2024/06/03 11:29:36 星期一
//* 描述：Type的一些拓展
//*******************************************************

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using System.Text;

namespace KFrame.Utilities
{
    /// <summary>
    /// Type的一些拓展
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 类型别名字典
        /// </summary>
        public static readonly Dictionary<Type, string> TypeNameAlternatives = new Dictionary<Type, string>
        {
            {
                typeof(float),
                "float"
            },
            {
                typeof(double),
                "double"
            },
            {
                typeof(sbyte),
                "sbyte"
            },
            {
                typeof(short),
                "short"
            },
            {
                typeof(int),
                "int"
            },
            {
                typeof(long),
                "long"
            },
            {
                typeof(byte),
                "byte"
            },
            {
                typeof(ushort),
                "ushort"
            },
            {
                typeof(uint),
                "uint"
            },
            {
                typeof(ulong),
                "ulong"
            },
            {
                typeof(decimal),
                "decimal"
            },
            {
                typeof(string),
                "string"
            },
            {
                typeof(char),
                "char"
            },
            {
                typeof(bool),
                "bool"
            },
            {
                typeof(float[]),
                "float[]"
            },
            {
                typeof(double[]),
                "double[]"
            },
            {
                typeof(sbyte[]),
                "sbyte[]"
            },
            {
                typeof(short[]),
                "short[]"
            },
            {
                typeof(int[]),
                "int[]"
            },
            {
                typeof(long[]),
                "long[]"
            },
            {
                typeof(byte[]),
                "byte[]"
            },
            {
                typeof(ushort[]),
                "ushort[]"
            },
            {
                typeof(uint[]),
                "uint[]"
            },
            {
                typeof(ulong[]),
                "ulong[]"
            },
            {
                typeof(decimal[]),
                "decimal[]"
            },
            {
                typeof(string[]),
                "string[]"
            },
            {
                typeof(char[]),
                "char[]"
            },
            {
                typeof(bool[]),
                "bool[]"
            }
        };
        private static readonly object CachedNiceNames_LOCK = new object();

        private static readonly Dictionary<Type, string> CachedNiceNames = new Dictionary<Type, string>();
        /// <summary>
        /// 从缓存里面获取NiceName
        /// </summary>
        private static string GetCachedNiceName(Type type)
        {
            lock (CachedNiceNames_LOCK)
            {
                //如果没有那就生成
                if (!CachedNiceNames.TryGetValue(type, out var value))
                {
                    value = CreateNiceName(type);
                    CachedNiceNames.Add(type, value);
                    return value;
                }

                return value;
            }
        }
        /// <summary>
        /// 创建一个NiceName
        /// </summary>
        private static string CreateNiceName(Type type)
        {
            if (type.IsArray)
            {
                int arrayRank = type.GetArrayRank();
                return type.GetElementType().GetNiceName() + ((arrayRank == 1) ? "[]" : "[,]");
            }

            if (type.InheritsFrom(typeof(Nullable<>)))
            {
                return type.GetGenericArguments()[0].GetNiceName() + "?";
            }

            if (type.IsByRef)
            {
                return "ref " + type.GetElementType().GetNiceName();
            }

            if (type.IsGenericParameter || !type.IsGenericType)
            {
                return type.GetMaybeSimplifiedTypeName();
            }

            StringBuilder stringBuilder = new StringBuilder();
            string name = type.Name;
            int num = name.IndexOf("`");
            if (num != -1)
            {
                stringBuilder.Append(name.Substring(0, num));
            }
            else
            {
                stringBuilder.Append(name);
            }

            stringBuilder.Append('<');
            Type[] genericArguments = type.GetGenericArguments();
            for (int i = 0; i < genericArguments.Length; i++)
            {
                Type type2 = genericArguments[i];
                if (i != 0)
                {
                    stringBuilder.Append(", ");
                }

                stringBuilder.Append(type2.GetNiceName());
            }

            stringBuilder.Append('>');
            return stringBuilder.ToString();
        }
        /// <summary>
        /// 获取简洁名称
        /// </summary>
        private static string GetMaybeSimplifiedTypeName(this Type type)
        {
            if (TypeNameAlternatives.TryGetValue(type, out var value))
            {
                return value;
            }

            return type.Name;
        }
        /// <summary>
        /// 返回一个规整不重复的Type的名称
        /// </summary>
        /// <returns>返回一个规整不重复的Type的名称</returns>
        public static string GetNiceName(this Type type)
        {
            if (type.IsNested && !type.IsGenericParameter)
            {
                return type.DeclaringType.GetNiceName() + "." + GetCachedNiceName(type);
            }

            return GetCachedNiceName(type);
        }
    }
}

