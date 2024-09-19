using System.Collections.Generic;
using System;
using KFrame;
using KFrame.Tools;
using KFrame.Utilities;
using UnityEngine;

namespace KFrame.UI
{
    public class LocalizationConfigBase : GlobalConfigBase<LocalizationConfigBase>
    {
        
    }

    public class LocalizationConfigBase<LanguageType> : LocalizationConfigBase where LanguageType : Enum
    {
        public Serialized_Dic<string, Serialized_Dic<LanguageType, LocalizationDataBase>> config =
            new Serialized_Dic<string, Serialized_Dic<LanguageType, LocalizationDataBase>>();
        public Serialized_Dic<LanguageType, int> LanguageFontSize
            = new Serialized_Dic<LanguageType, int>();
        

        public T GetContent<T>(string key, LanguageType languageType) where T : LocalizationDataBase
        {
            LocalizationDataBase content = null;
            if (config.Dictionary.TryGetValue(key, out Serialized_Dic<LanguageType, LocalizationDataBase> dic))
            {
                dic.Dictionary.TryGetValue(languageType, out content);
            }

            return (T)content;
        }
    }
}