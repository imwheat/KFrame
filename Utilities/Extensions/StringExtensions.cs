//****************** 代码文件申明 ***********************
//* 文件：StringExtensions
//* 作者：wheat
//* 创建时间：2024/06/05 09:13:35 星期三
//* 描述：字符串的拓展
//*******************************************************


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KFrame.Utilities
{
    public static class StringExtensions
    {
        private static readonly string[] inputDelimiters = new string[] { "\"\"", "''", "{}", "()", "[]" ,"//","\\\\"};

        #region 字符串拆分与替换

        public static char[] Punctuations = {
            '。',
            '.',
            ';',
            '；',
            '?',
            '？',
            '!',
            '！',
            ',',
            '，'
        };

        /// <summary>
        /// 按照,#+|，将字符串进行拆分
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static string[] GetSpliteStringFormat(string inputStr)
        {
            Regex reg = new Regex("[,#+|]");
            return reg.Split(inputStr);
        }

        /// <summary>
        /// 将输入字符串中的空格、换行符、新行、tab字符替换为指定的字符串。
        /// </summary>
        /// <param name="inputStr">要替换的输入字符串。</param>
        /// <param name="replaceStr">要用于替换空格的字符串（默认为空字符串）。</param>
        /// <returns>替换后的字符串。</returns>
        public static string ReplaceSpaceByFormat(string inputStr, string replaceStr = "")
        {
            Regex reg = new Regex(@"\s");
            return reg.Replace(inputStr, replaceStr);
        }

        /// <summary>
        /// 仅以指定格式替换左右两端的空格，string里的 Trim也可删除左右两端空格
        /// </summary>
        /// <param name="inputStr"></param>
        /// <param name="replaceStr">要替换成的字符</param>
        /// <returns></returns>
        public static string ReplaceTrimSpaceByFormat(string inputStr, string replaceStr = "")
        {
            Regex reg = new Regex(@"(^\s*)|(\s*$)");
            return reg.Replace(inputStr, replaceStr);
        }

        /// <summary>
        /// 仅以指定格式替换左端的空格，string里的 TrimStart也可删除左端空格
        /// </summary>
        /// <param name="inputStr"></param>
        /// <param name="replaceStr">要替换成的字符</param>
        /// <returns></returns>
        public static string ReplaceTrimStartByFormat(string inputStr, string replaceStr = "")
        {
            Regex reg = new Regex(@"(^\s*)");
            return reg.Replace(inputStr, replaceStr);
        }

        /// <summary>
        /// 仅以指定格式替换右端的空格，string里的 TrimEnd也可删除右端空格
        /// </summary>
        /// <param name="inputStr"></param>
        /// <param name="replaceStr">要替换成的字符</param>
        /// <returns></returns>
        public static string ReplaceTrimEndByFormat(string inputStr, string replaceStr = "")
        {
            Regex reg = new Regex(@"(\s*$)");
            return reg.Replace(inputStr, replaceStr);
        }

        #endregion

        /// <summary>
        /// 把每个单词中间空格去除
        /// </summary>
        /// <returns>比如My Int Value => MyIntValue</returns>
        public static string ConnectWords(this string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var c in input)
            {
                //如果是空格就跳过
                if(c == ' ') continue;
                stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }
        /// <summary>
        /// 把一个单词调整好格式，去掉空格，首字母大写
        /// </summary>
        /// <param name="input">原文本</param>
        /// <returns>"  asd "=>"Asd"</returns>
        public static string GetNiceFormat(this string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            
            StringBuilder sb = new StringBuilder();
            bool firstCharacter = true;
            foreach (var c in input)
            {
                //如果是空格就跳过
                if(c == ' ') continue;
                if (firstCharacter && !Char.IsUpper(c))
                {
                    sb.Append(Char.ToUpper(c));
                }
                else
                {
                    sb.Append(c);
                }
                firstCharacter = false;
            }

            return sb.ToString();
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
            if (string.IsNullOrEmpty(input))
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
        
        /// <summary>
        /// 是否是数字和字母组成
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool IsNumberAndLetterStruct(string inputStr)
        {
            Regex reg = new Regex("^[A-Za-z0-9]+$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 判断字符是否由数字或者字母组成 调用的是char的系统api
        /// </summary>
        /// <param name="inputChar">输入的字符</param>
        /// <returns>是数字或者字母返回true 反之false</returns>
        public static bool IsNumberAndLetterStruct(char inputChar)
        {
            Regex reg = new Regex("^[A-Za-z0-9]+$");
            return reg.IsMatch(inputChar.ToString());
        }

        /// <summary>
        /// 判断一个字符是否是用来进行文本分割的标点符号 比如中英文中的逗号 句号 分号等
        /// </summary>
        /// <param name="inputChar"></param>
        /// <returns></returns>
        public static bool IsSegmentationPunctuations(char inputChar)
        {
            //return Punctuations.Contains(inputChar);
            return inputChar switch
            {
                '。' => true,
                '.' => true,
                ';' => true,
                '；' => true,
                '?' => true,
                '？' => true,
                '!' => true,
                '！' => true,
                ',' => true,
                '，' => true,
                '~' => true,
                _ => false
            };
        }

        /// <summary>
        /// 判断字符是否是中文汉字
        /// </summary>
        /// <param name="word">字符</param>
        /// <returns></returns>
        public static bool IsChineseChar(char word)
        {
            //在ASCII码中 中文汉字的范围以下区间
            return 0x4e00 <= word && word <= 0x9fbb;
        }

        /// <summary>
        /// 是否是小写英文字母组成
        /// </summary>
        /// <param name="inputStr">例如russel</param>
        /// <returns></returns>
        public static bool IsLowercaseLetterStruct(string inputStr)
        {
            Regex reg = new Regex("^[a-z]+$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否由大写英文字母组成
        /// </summary>
        /// <param name="inputStr">例如ABC</param>
        /// <returns></returns>
        public static bool IsUppercaseLetterStruct(string inputStr)
        {
            Regex reg = new Regex("^[A-Z]+$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 对字符串操作的拓展
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string content)
        {
            return string.IsNullOrEmpty(content);
        }
        /// <summary>
        /// 在字符串中查找分隔符组 'c' 所属的索引
        /// 例如"()", 输入'('，返回3
        /// </summary>
        /// <returns>如果不是分割符，那就返回-1</returns>
        private static int IndexOfDelimiterGroup(char c)
        {
            for (int i = 0; i < inputDelimiters.Length; i++)
            {
                if (c == inputDelimiters[i][0])
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// 找到某一节短字符串的末尾下标
        /// </summary>
        /// <param name="text">原string</param>
        /// <param name="delimiter">分隔符</param>
        /// <param name="startIndex">开始下标</param>
        /// <returns>找到分隔符的另一半的下标，比如'('要找到')'</returns>
        public static int IndexOfDelimiterGroupEnd(string text, char delimiter, int startIndex)
        {
            //获取分隔符的索引
            int delimiterIndex = IndexOfDelimiterGroup(delimiter);

            //不是分隔符直接返回command.Length
            if (delimiterIndex == -1)
            {
                return text.Length;
            }

            //获取分隔符的字符
            char startChar = inputDelimiters[delimiterIndex][0];
            char endChar = inputDelimiters[delimiterIndex][1];

            // 检查数组支持的分隔符深度（例如，Vector2数组的[[1 2] [3 4]]）
            int depth = 1;

            //开始遍历
            for (int i = startIndex; i < text.Length; i++)
            {
                //每找到一个“开分隔符”深度就+1，找到一个“末尾分隔符”深度就-1
                char c = text[i];
                //当深度为0时，结束返回结果。
                if (c == endChar && --depth <= 0)
                    return i;
                else if (c == startChar)
                    depth++;
            }

            //如果超出范围了，那就直接返回指令长度
            return text.Length;
        }
        /// <summary>
        /// 查找某一限度区域的字符串的下标
        /// </summary>
        /// <param name="text">原字符串</param>
        /// <param name="find">要找的字符串</param>
        /// <param name="startIndex">开始下标</param>
        /// <param name="endIndex">末尾下标</param>
        /// <returns>找不到就返回-1，找到了就返回对应下标</returns>
        public static int IndexOf(string text, string find,int startIndex, int endIndex = -1)
        {
            //如果不填写末尾下标，那就默认为最后一位
            if(endIndex == -1)
            {
                endIndex = text.Length -1;
            }
            //如果要找的为空，那就直接返回开始位置的索引
            if (string.IsNullOrEmpty(find)) return startIndex;
            //如果要找的比原先的text要长，那就直接返回-1
            if (find.Length > text.Length) return -1;
            //防止越界
            startIndex = Math.Clamp(startIndex, 0, text.Length -1);
            endIndex = Math.Clamp(endIndex, startIndex, Math.Min(text.Length - find.Length, text.Length -1));

            //开始遍历查找
            for (int i = startIndex; i <= endIndex; i++)
            {
                //如果这一段文本相等，那就返回i
                if(IsEqual(text, find, i))
                {
                    return i;
                }
            }

            //找不到返回-1
            return -1;
        }
        /// <summary>
        /// 检测text1的某一片段是否和text2相等
        /// </summary>
        /// <param name="text1">原文本</param>
        /// <param name="text2">检测文本</param>
        /// <param name="startIndex">起始位置</param>
        /// <returns>相等的话返回true</returns>
        public static bool IsEqual(string text1, string text2, int startIndex)
        {
            //如果为空直接返回true
            if (string.IsNullOrEmpty(text2)) return true;

            //如果原text为空直接返回false
            if(string.IsNullOrEmpty(text1)) return false;

            //防止越界
            startIndex = Math.Clamp(startIndex, 0, text1.Length -1);

            //如果超过了text1的长度直接返回false
            if (startIndex + text2.Length > text1.Length) return false;
            for (int i = 0; i < text2.Length; i++)
            {
                //如果有一个char不相等就返回false
                if (text1[i + startIndex] != text2[i]) return false;
            }

            return true;
        }
        
        /// <summary>
        /// 判断text是否包含数字里的任意一个元素
        /// </summary>
        /// <param name="text">原文本</param>
        /// <param name="array">检测数组</param>
        /// <param name="comparison">检测方式</param>
        /// <returns>如果符合就返回true</returns>
        public static bool ContainsAny(this string text, IList<string> array, StringComparison comparison = StringComparison.Ordinal)
        {
            //如果数组和文本都为空返回true
            if (array == null && string.IsNullOrEmpty(text)) return true;
            //有一个为空返回false
            else if (array == null || string.IsNullOrEmpty(text)) return false;

            //逐个遍历检测
            foreach (var str in array)
            {
                if (text.Contains(str, comparison))
                {
                    return true;
                }
            }
            
            //都不符合返回false
            return false;
        }
    }
}