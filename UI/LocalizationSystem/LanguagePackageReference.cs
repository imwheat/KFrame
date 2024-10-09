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
        /// 语言id
        /// </summary>
        public int languageId;
        /// <summary>
        /// 语言名称
        /// </summary>
        public string languageName;
        /// <summary>
        /// 语言包路径
        /// </summary>
        public string packagePath;
    }
}

