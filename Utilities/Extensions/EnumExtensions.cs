//****************** 代码文件申明 ***********************
//* 文件：EnumExtensions
//* 作者：wheat
//* 创建时间：2024/09/21 16:36:03 星期六
//* 描述：拓展辅助枚举的一些操作
//*******************************************************


using System;

namespace KFrame.Utilities
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取某个枚举类型的所有值
        /// </summary>
        /// <typeparam name="TEnum">你要的枚举类型</typeparam>
        /// <returns>该枚举类型的所有值的数组</returns>
        public static TEnum[] GetValues<TEnum>() where TEnum : struct
        {
            return (TEnum[])Enum.GetValues(typeof(TEnum));
        }
        
    }
}

