using KFrame.Attributes;

namespace KFrame.UI
{
    /// <summary>
    /// 语言类型
    /// </summary>
    public enum LanguageType
    {
        /// <summary>
        /// 简体中文
        /// </summary>
        [KLabelText("简体中文")]SimplifiedChinese = 0,
        /// <summary>
        /// 繁体中文
        /// </summary>
        [KLabelText("繁體中文")]TraditionalChinese = 1,
        /// <summary>
        /// 英语
        /// </summary>
        [KLabelText("English")]English = 2, 
        /// <summary>
        /// 日语
        /// </summary>
        [KLabelText("日本語")]Japanese = 3,
    }
}