using System.Text.RegularExpressions;

namespace KFrame.Utilities
{
    public static class NetExtensions
    {
        #region 计算机网络相关

        /// <summary>
        /// 是否是电子邮箱
        /// </summary>
        /// <param name="inputStr">例如10086@qq.com</param>
        /// <returns></returns>
        public static bool IsEmail(string inputStr)
        {
            //这里是我们使用的匹配规则
            Regex reg = new Regex(@"^[A-Za-z0-9\u4e00-\u9fa5]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否是计算机域名（非网址, 不包含协议）
        /// </summary>
        /// <param name="inputStr">例如www.baidu.com</param>
        /// <returns></returns>
        public static bool IsInternetDomainName(string inputStr)
        {
            //这里是我们使用的匹配规则
            Regex reg = new Regex(@"^([0-9a-zA-Z-]{1,}\.)+([a-zA-Z]{2,})$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 必须是带端口号的网址(或ip)
        /// </summary>
        /// <param name="inputStr">例如https://www.qq.com:8080</param>
        /// <returns></returns>
        public static bool IsIpAddressHavePort(string inputStr)
        {
            //这里是我们使用的匹配规则
            Regex reg = new Regex(@"^((ht|f)tps?:\/\/)?[\w-]+(\.[\w-]+)+:\d{1,5}\/?$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否是统一资源标识符
        /// </summary>
        /// <param name="inputStr">例如www.qq.com</param>
        /// <returns></returns>
        public static bool IsURL(string inputStr)
        {
            //这里是我们使用的匹配规则
            Regex reg = new Regex(
                @"^(((ht|f)tps?):\/\/)?([^!@#$%^&*?.\s-]([^!@#$%^&*?.\s]{0,63}[^!@#$%^&*?.\s])?\.)+[a-z]{2,6}\/?");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否是子网掩码（不包含0.0.0.0）
        /// </summary>
        /// <param name="inputStr">例如255.255.255.0</param>
        /// <returns></returns>
        public static bool IsSubNetMask(string inputStr)
        {
            //这里是我们使用的匹配规则
            Regex reg = new Regex(
                @"^(254|252|248|240|224|192|128)\.0\.0\.0|255\.(254|252|248|240|224|192|128|0)\.0\.0|255\.255\.(254|252|248|240|224|192|128|0)\.0|255\.255\.255\.(255|254|252|248|240|224|192|128|0)$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否是迅雷链接
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool IsThunderURL(string inputStr)
        {
            //这里是我们使用的匹配规则
            Regex reg = new Regex(@"^thunderx?:\/\/[a-zA-Z\d]+=$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否是磁力链接
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool IsMagneticLinkURL(string inputStr)
        {
            //这里是我们使用的匹配规则
            Regex reg = new Regex(@"^magnet:\?xt=urn:btih:[0-9a-fA-F]{40,}.*$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否是IPV4端口
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static bool IsIPV4Port(string inputStr)
        {
            Regex reg = new Regex(
                @"^((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.){3}(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])(?::(?:[0-9]|[1-9][0-9]{1,3}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5]))?$");
            return reg.IsMatch(inputStr);
        }

        /// <summary>
        /// 是否是Mac地址
        /// </summary>
        /// <param name="inputStr">例如：38:f9:d3:4b:f5:51或00-0C-29-CA-E4-66</param>
        /// <returns></returns>
        public static bool IsMacAddress(string inputStr)
        {
            Regex reg = new Regex(@"^((([a-f0-9]{2}:){5})|(([a-f0-9]{2}-){5}))[a-f0-9]{2}$");
            return reg.IsMatch(inputStr);
        }

        #endregion
    }
}