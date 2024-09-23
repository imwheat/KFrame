using System;
using System.Globalization;

namespace KFrame.Utilities
{
    public static class DataTimeExtensions
    {
        #region 时间工具

        /// <summary>
        /// 获取当前格式化的系统时间(!默认有一个参数，默认为false)
        /// </summary>
        /// <returns>返回24小时制的当前时间</returns>
        public static string GetFormatNowTime(bool is12Time = false)
        {
            return DateTime.Now.ToString(is12Time ? "yyyyMMdd_hh" : "yyyyMMdd_HH");
        }

        /// <summary>
        /// 获取当前格式化的系统时间(!默认有一个参数，默认为false)
        /// </summary>
        /// <returns>返回24小时制的当前时间</returns>
        public static string GetFormatNowTimeToSecond(bool is12Time = false)
        {
            return DateTime.Now.ToString(is12Time ? "yyyyMMdd_hh_mm_ss" : "yyyyMMdd_HH_mm_ss");
        }


        /// <summary>
        /// 简化获取时间的方式
        /// </summary>
        /// <returns></returns>
        public static string GetNowTime()
        {
            //CultureInfo.CurrentCulture表示当前系统上下文中使用的文化设置。例如，它可以用于格式化和解析日期、时间、货币、数字等本地化数据。
            return DateTime.Now.ToString(CultureInfo.CurrentCulture);
        }

        #endregion
    }
}