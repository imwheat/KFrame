//****************** 代码文件申明 ************************
//* 文件：LocalizationConfig                      
//* 作者：wheat
//* 创建时间：2024/09/19 星期四 16:08:22
//* 功能：本地化配置的一些数据
//*****************************************************


using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;
using KFrame.Attributes;
using KFrame.Tools;
using KFrame.Utilities;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KFrame.UI
{
    [CreateAssetMenu(menuName = "KFrame/LocalizationConfig", fileName = "LocalizationConfig")]
    public class LocalizationConfig : GlobalConfigBase<LocalizationConfig>
    {
        public Serialized_Dic<string, Serialized_Dic<LanguageType, LocalizationDataBase>> config =
            new Serialized_Dic<string, Serialized_Dic<LanguageType, LocalizationDataBase>>();
        public Serialized_Dic<LanguageType, int> LanguageFontSize
            = new Serialized_Dic<LanguageType, int>();
        
        /// <summary>
        /// 获取本地化配置
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="languageType">语言类型</param>
        /// <typeparam name="T">数据类型</typeparam>
        public T GetContent<T>(string key, LanguageType languageType) where T : LocalizationDataBase
        {
            LocalizationDataBase content = null;
            if (config.Dictionary.TryGetValue(key, out Serialized_Dic<LanguageType, LocalizationDataBase> dic))
            {
                dic.Dictionary.TryGetValue(languageType, out content);
            }

            return (T)content;
        }
        /// <summary>
        /// 尝试获取本地化配置
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="languageType">语言类型</param>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns>找不到就返回false</returns>
        public bool TryGetContent<T>(string key, LanguageType languageType, out T content) where T : LocalizationDataBase
        {
            content = null;
            if (config.Dictionary.TryGetValue(key, out Serialized_Dic<LanguageType, LocalizationDataBase> dic))
            {
                if (dic.Dictionary.TryGetValue(languageType, out LocalizationDataBase _content))
                {
                    content = _content as T;
                    return true;
                }
            }

            return false;
        }
    }
}