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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">语言id</param>
        /// <param name="name">语言名称</param>
        /// <param name="path">语言包路径</param>
        public LanguagePackageReference(int id, string name, string path)
        {
            languageId = id;
            languageName = name;
            packagePath = path;
        }
    }
}

