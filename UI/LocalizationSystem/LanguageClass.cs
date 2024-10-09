//****************** 代码文件申明 ***********************
//* 文件：LanguageClass
//* 作者：wheat
//* 创建时间：2024/10/09 13:15:10 星期三
//* 描述：语言类，提供储存每种语言的基础
//*******************************************************

namespace KFrame.UI
{
    [System.Serializable]
    public class LanguageClass
    {
        /// <summary>
        /// 语言id
        /// </summary>
        public int languageId;
        /// <summary>
        /// 语言名称
        /// </summary>
        public string languageName;

        public LanguageClass(int id, string name)
        {
            languageId = id;
            languageName = name;
        }
    }
}

