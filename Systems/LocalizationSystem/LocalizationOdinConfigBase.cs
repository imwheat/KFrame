using System.Collections.Generic;
using System;
using KFrame;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KFrame.Systems
{
    public abstract class LocalizationOdinConfigSuperBase : ScriptableObject
    {
        
    }

    public abstract class LocalizationOdinConfigBase<LanguageType> : LocalizationOdinConfigSuperBase where LanguageType : Enum
    {
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.CollapsedFoldout),
            LabelText("本地化内容配置"), FoldoutGroup("信息查询")]
        public Dictionary<string, Dictionary<LanguageType, LocalizationDataBase>> config =
            new Dictionary<string, Dictionary<LanguageType, LocalizationDataBase>>();
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.CollapsedFoldout),
            LabelText("语言文本字体大小设置"), FoldoutGroup("信息查询")]
        public Dictionary<LanguageType, int> LanguageFontSize
            = new Dictionary<LanguageType, int>();
        

        public T GetContent<T>(string key, LanguageType languageType) where T : LocalizationDataBase
        {
            LocalizationDataBase content = null;
            if (config.TryGetValue(key, out Dictionary<LanguageType, LocalizationDataBase> dic))
            {
                dic.TryGetValue(languageType, out content);
            }

            return (T)content;
        }
    }
}