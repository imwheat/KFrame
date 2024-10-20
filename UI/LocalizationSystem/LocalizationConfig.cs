//****************** 代码文件申明 ************************
//* 文件：LocalizationConfig                      
//* 作者：wheat
//* 创建时间：2024/09/19 星期四 16:08:22
//* 功能：本地化配置的一些数据
//*****************************************************


using System.Collections.Generic;
using KFrame.Attributes;
using KFrame.Utilities;

namespace KFrame.UI
{
    [KGlobalConfigPath(GlobalPathType.Assets, typeof(LocalizationConfig), true)]
    public class LocalizationConfig : GlobalConfigBase<LocalizationConfig>
    {
        #region 储存数据

#if UNITY_EDITOR

        /// <summary>
        /// 编辑器默认语言id
        /// </summary>
        public const int EditorDefaultLanguageId = 0;
        
#endif
        
        /// <summary>
        /// 本地化的文本数据
        /// </summary>
        public List<LocalizationStringData> TextDatas;
        /// <summary>
        /// 本地化的图片数据
        /// </summary>
        public List<LocalizationImageData> ImageDatas;
        /// <summary>
        /// 语言包
        /// </summary>
        public List<LanguagePackageReference> packageReferences;

        #endregion

    }
}