using System.Text.RegularExpressions;

namespace KFrame.Utilities
{
    public class IDCardTool
    {
        #region 身份证检测、国家及地区检测

        /// <summary>
        /// 是否是中国的省份
        /// </summary>
        /// <param name="inputStr">例如浙江、台湾</param>
        /// <returns></returns>
        public static bool IsChineseProvinces(string inputStr)
        {
            //这里是我们使用的匹配规则
            Regex reg = new Regex(
                "^浙江|上海|北京|天津|重庆|黑龙江|吉林|辽宁|内蒙古|河北|新疆|甘肃|青海|陕西|宁夏|河南|山东|山西|安徽|湖北|湖南|江苏|四川|贵州|云南|广西|西藏|江西|广东|福建|台湾|海南|香港|澳门$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否是身份证号（1代，15位数字）
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool IsIdentityCard_1(string inputStr)
        {
            //这里是我们使用的匹配规则
            Regex reg = new Regex(@"^[1-9]\d{7}(?:0\d|10|11|12)(?:0[1-9]|[1-2][\d]|30|31)\d{3}$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否是身份证号（2代，18位数字，最后一位是校验位,可能为数字或字符X）
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool IsIdentityCard_2(string inputStr)
        {
            Regex reg = new Regex(
                @"^[1-9]\d{5}(?:18|19|20)\d{2}(?:0[1-9]|10|11|12)(?:0[1-9]|[1-2]\d|30|31)\d{3}[\dXx]$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否是香港身份证
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool IsHongkongIdentityCard(string inputStr)
        {
            Regex reg = new Regex(@"^[a-zA-Z]\d{6}\([\dA]\)$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否是澳门身份证
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool IsMacauIdentityCard(string inputStr)
        {
            Regex reg = new Regex(@"^[1|5|7]\d{6}\(\d\)$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否是台湾身份证
        /// </summary>
        /// <param name="inputStr">例如：U193683453</param>
        /// <returns></returns>
        public static bool IsTaiwanIdentityCard(string inputStr)
        {
            Regex reg = new Regex(@"^[a-zA-Z][0-9]{9}$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否是中国的邮政编码
        /// </summary>
        /// <param name="inputStr">例如100101</param>
        /// <returns></returns>
        public static bool IsChinesePostalCode(string inputStr)
        {
            Regex reg = new Regex(@"^(0[1-7]|1[0-356]|2[0-7]|3[0-6]|4[0-7]|5[1-7]|6[1-7]|7[0-5]|8[013-6])\d{4}$");
            return reg.IsMatch(inputStr);
        }

        #endregion
        
    }
}