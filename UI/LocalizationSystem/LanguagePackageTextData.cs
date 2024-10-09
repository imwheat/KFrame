//****************** 代码文件申明 ***********************
//* 文件：LanguagePackageTextData
//* 作者：wheat
//* 创建时间：2024/10/09 13:49:59 星期三
//* 描述：语言包文本数据
//*******************************************************

namespace KFrame.UI
{
    /// <summary>
    /// 语言包文本数据
    /// </summary>
    [System.Serializable]
    public class LanguagePackageTextData
    {
        public string key;
        public string text;

        public LanguagePackageTextData(string key)
        {
            this.key = key;
        }

        public LanguagePackageTextData(string key, string text) : this(key)
        {
            this.text = text;
        }
    }
}

