//****************** 代码文件申明 ***********************
//* 文件：LanguagePackageReference
//* 作者：wheat
//* 创建时间：2024/10/09 13:38:06 星期三
//* 描述：存储记录了LanguagePackage的路径等基础信息
//*******************************************************

namespace KFrame.UI
{
    [System.Serializable]
    public class LanguagePackageReference
    {
        /// <summary>
        /// 语言数据类
        /// </summary>
        public LanguageClass language;
        /// <summary>
        /// 语言id
        /// </summary>
        public int LanguageId
        {
            get => language.languageId;
            set => language.languageId = value;
        }

        /// <summary>
        /// 语言key
        /// </summary>
        public string LanguageKey
        {
            get => language.languageKey;
            set => language.languageKey = value;
        }

        /// <summary>
        /// 语言名称
        /// </summary>
        public string LanguageName
        {
            get => language.languageName;
            set => language.languageName = value;
        }
        /// <summary>
        /// 语言包路径
        /// </summary>
        public string packagePath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="language">语言数据类</param>
        /// <param name="path">语言包路径</param>
        public LanguagePackageReference(LanguageClass language,string path)
        {
            this.language = language;
            packagePath = path;
        }
    }
}

