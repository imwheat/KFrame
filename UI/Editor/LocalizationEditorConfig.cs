//****************** 代码文件申明 ***********************
//* 文件：LocalizationEditorConfig
//* 作者：wheat
//* 创建时间：2024/10/09 18:48:30 星期三
//* 描述：本地化编辑器配置
//*******************************************************

using System.Collections.Generic;
using KFrame.Utilities;
using KFrame.Attributes;

namespace KFrame.UI.Editor
{
    [KGlobalConfigPath(GlobalPathType.Editor, typeof(LocalizationEditorConfig), true)]
    public class LocalizationEditorConfig : GlobalConfigBase<LocalizationEditorConfig>
    {
        #region 存储数据

        /// <summary>
        /// 本地化配置的key
        /// </summary>
        public List<LanguagePackageKeyData> localizationKeys;

        #endregion
    }
}

