//****************** 代码文件申明 ***********************
//* 文件：StringExtensions
//* 作者：wheat
//* 创建时间：2024/06/05 09:13:35 星期三
//* 描述：字符串的拓展
//*******************************************************

using System;
using System.Globalization;
using System.Text;

namespace KFrame.Utilities
{
    public static class StringExtensions
    {
        /// <summary>
        /// 把全大写加_的转为驼峰命名
        /// </summary>
        /// <returns>比如MY_INT_VALUE => MyIntValue</returns>
        public static string ToTitleCase(this string input)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '_' && i + 1 < input.Length)
                {
                    char c2 = input[i + 1];
                    if (char.IsLower(c2))
                    {
                        c2 = char.ToUpper(c2, CultureInfo.InvariantCulture);
                    }

                    stringBuilder.Append(c2);
                    i++;
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 指定一种比较方法，用来检测字符串里面是否包含
        /// </summary>
        /// <returns></returns>
        public static bool Contains(this string source, string toCheck, StringComparison comparisonType)
        {
            return source.IndexOf(toCheck, comparisonType) >= 0;
        }

        /// <summary>
        /// 把驼峰命名拆为单词
        /// </summary>
        /// <returns>"thisIsCamelCase" -> "This Is Camel Case"</returns>
        public static string SplitPascalCase(this string input)
        {
            if (input == null || input.Length == 0)
            {
                return input;
            }

            StringBuilder stringBuilder = new StringBuilder(input.Length);
            if (char.IsLetter(input[0]))
            {
                stringBuilder.Append(char.ToUpper(input[0]));
            }
            else
            {
                stringBuilder.Append(input[0]);
            }

            for (int i = 1; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsUpper(c) && !char.IsUpper(input[i - 1]))
                {
                    stringBuilder.Append(' ');
                }

                stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 判断是否为空或者只包含空格
        /// </summary>
        /// <returns>如果为空或者只有空格那就返回true</returns>
        public static bool IsNullOrWhitespace(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (!char.IsWhiteSpace(str[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 计算俩个字符串的Levenshtein 距离
        ///  O(n*m)
        /// </summary>
        /// <returns></returns>
        public static int CalculateLevenshteinDistance(string source1, string source2)
        {
            int length = source1.Length;
            int length2 = source2.Length;
            int[,] array = new int[length + 1, length2 + 1];
            if (length == 0)
            {
                return length2;
            }

            if (length2 == 0)
            {
                return length;
            }

            int num = 0;
            while (num <= length)
            {
                array[num, 0] = num++;
            }

            int num2 = 0;
            while (num2 <= length2)
            {
                array[0, num2] = num2++;
            }

            for (int i = 1; i <= length; i++)
            {
                for (int j = 1; j <= length2; j++)
                {
                    int num3 = ((source2[j - 1] != source1[i - 1]) ? 1 : 0);
                    array[i, j] = Math.Min(Math.Min(array[i - 1, j] + 1, array[i, j - 1] + 1), array[i - 1, j - 1] + num3);
                }
            }

            return array[length, length2];
        }
    }
}

