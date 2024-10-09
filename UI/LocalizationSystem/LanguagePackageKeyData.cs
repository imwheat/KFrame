#if UNITY_EDITOR

//****************** 代码文件申明 ***********************
//* 文件：LanguagePackageKeyData
//* 作者：wheat
//* 创建时间：2024/10/09 18:40:42 星期三
//* 描述：存储记录了语言包的一项翻译对象的key数据
//*******************************************************

namespace KFrame.UI
{
    [System.Serializable]
    public class LanguagePackageKeyData
    {
        /// <summary>
        /// 该数据的key
        /// </summary>
        public string key;
        /// <summary>
        /// 备注
        /// </summary>
        public string notes;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">该数据的key</param>
        public LanguagePackageKeyData(string key)
        {
            this.key = key;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">该数据的key</param>
        /// <param name="notes">备注</param>
        public LanguagePackageKeyData(string key, string notes) : this(key)
        {
            this.notes = notes;
        }
    }
}

#endif