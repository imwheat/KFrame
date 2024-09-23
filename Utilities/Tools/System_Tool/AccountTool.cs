using System.Text.RegularExpressions;
using UnityEngine;

namespace KFrame.Utilities
{
    /// <summary>
    /// 创建账号使用
    /// </summary>
    public static class AccountTool
    {
        /// <summary>
        /// 将指定的文本复制到系统剪贴板。
        /// </summary>
        /// <param name="copyText">要复制的文本。</param>
        public static void CopyText(string copyText)
        {
            GUIUtility.systemCopyBuffer = copyText;
        }

        /// <summary>
        /// 得到当前系统剪贴板的内容
        /// </summary>
        /// <returns></returns>
        public static string GetCopyText()
        {
            return GUIUtility.systemCopyBuffer;
        }

        #region 输入检测、密码检测相关

        /// <summary>
        /// 是否是md5格式（32位）
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool CheckMD5Format(string inputStr)
        {
            Regex reg = new Regex(@"^[a-fA-F0-9]{32}$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// QQ号格式是否正确
        /// </summary>
        /// <param name="inputStr">例如1210420078</param>
        /// <returns></returns>
        public static bool CheckQQFormat(string inputStr)
        {
            Regex reg = new Regex(@"^[1-9][0-9]{4,10}$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 账号是否合法（字母开头，允许5-16字节，允许字母数字下划线组合）
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool CheckAccountIsLegitimate(string inputStr)
        {
            Regex reg = new Regex(@"^[a-zA-Z]\w{4,15}$");
            return reg.IsMatch(inputStr);
        }


        /// <summary>
        /// 密码强度校验，最少6位，包括至少1个大写字母，1个小写字母，1个数字，1个特殊字符
        /// </summary>
        /// <param name="inputStr">例如Zc@bilibili66666</param>
        /// <returns></returns>
        public static bool CheckPasswordStrength(string inputStr)
        {
            Regex reg = new Regex(@"^\S*(?=\S{6,})(?=\S*\d)(?=\S*[A-Z])(?=\S*[a-z])(?=\S*[!@#$%^&*? ])\S*$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否由中文和数字组成
        /// </summary>
        /// <param name="inputStr">例如：你好6啊</param>
        /// <returns></returns>
        public static bool IsChineseAndNumberStruct(string inputStr)
        {
            Regex reg = new Regex(
                @"^((?:[\u3400-\u4DB5\u4E00-\u9FEA\uFA0E\uFA0F\uFA11\uFA13\uFA14\uFA1F\uFA21\uFA23\uFA24\uFA27-\uFA29]|[\uD840-\uD868\uD86A-\uD86C\uD86F-\uD872\uD874-\uD879][\uDC00-\uDFFF]|\uD869[\uDC00-\uDED6\uDF00-\uDFFF]|\uD86D[\uDC00-\uDF34\uDF40-\uDFFF]|\uD86E[\uDC00-\uDC1D\uDC20-\uDFFF]|\uD873[\uDC00-\uDEA1\uDEB0-\uDFFF]|\uD87A[\uDC00-\uDFE0])|(\d))+$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否不包含字母
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool IsWithoutAlphabet(string inputStr)
        {
            Regex reg = new Regex("^[^A-Za-z]*$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 大写字母，小写字母，数字，特殊符号 `@#$%^&*`~()-+=` 中任意3项密码
        /// </summary>
        /// <param name="inputStr">例如：a1@,A1@</param>
        /// <returns></returns>
        public static bool CheckPasswordHaveHowManyType(string inputStr)
        {
            Regex reg = new Regex(
                @"^(?![a-zA-Z]+$)(?![A-Z0-9]+$)(?![A-Z\W_!@#$%^&*`~()-+=]+$)(?![a-z0-9]+$)(?![a-z\W_!@#$%^&*`~()-+=]+$)(?![0-9\W_!@#$%^&*`~()-+=]+$)[a-zA-Z0-9\W_!@#$%^&*`~()-+=]");
            return reg.IsMatch(inputStr);
        }

        #endregion

        /// <summary>
        /// 版本号(version)格式必须为X.Y.Z
        /// </summary>
        /// <param name="inputStr">例如16.3.10</param>
        /// <returns></returns>
        public static bool CheckVersionForamt(string inputStr)
        {
            //这里是我们使用的匹配规则
            Regex reg = new Regex(@"^\d+(?:\.\d+){2}$");
            return reg.IsMatch(inputStr);
        }
    }
}