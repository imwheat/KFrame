//****************** 代码文件申明 ************************
//* 文件：LocalizationSystem                      
//* 作者：wheat
//* 创建时间：2024年09月14日 星期六 10:34
//* 功能：管理游戏本地化
//*****************************************************


using System;
using KFrame.Systems;
using UnityEngine;
using UnityEngine.Serialization;

namespace KFrame.UI
{
    public static class LocalizationSystem
    {
        private const string OnUpdateLanguage = "OnUpdateLanguage";
        
        /// <summary>
        /// 语言类型
        /// </summary>
        private static LanguageType languageType;
        /// <summary>
        /// 语言类型，设置时会自动分发语言修改事件
        /// </summary>
        public static LanguageType LanguageType
        {
            get
            {
                return languageType;
            }
            set
            {
                if (UISetPropertyUtility.SetStruct<LanguageType>(ref languageType, value))
                {
                    OnLanguageValueChanged();
                }
            }
        }

        public static void Init()
        {
        }

        public static LocalizationConfig Config => LocalizationConfig.Instance;

        public static void OnLanguageValueChanged()
        {
            EventBroadCastSystem.EventTrigger(OnUpdateLanguage, languageType);
        }
        /// <summary>
        /// 获取本地化配置数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <typeparam name="T">需要的数据类型</typeparam>
        /// <returns>没有的话返回null</returns>
        public static T GetLocalizationData<T>(string key, LanguageType language) where T : LocalizationDataBase
        {
            if (Config == null)
            {
                Debug.LogWarning("缺少本地化Config");
                return null;
            }

            return Config.GetContent<T>(key, language);
        }
        /// <summary>
        /// 尝试获取本地化配置数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <typeparam name="T">需要的数据类型</typeparam>
        /// <returns>没有的话返回false</returns>
        public static bool TryGetLocalizationData<T>(string key, LanguageType language, out T data) where T : LocalizationDataBase
        {
            data = null;
            
            if (Config == null)
            {
                Debug.LogWarning("缺少本地化Config");
                return false;
            }

            return Config.TryGetContent<T>(key, language, out data);
        }
        /// <summary>
        /// 根据key获取当前语言的文本内容
        /// </summary>
        public static string GetGlobalStringData(string key)
        {
            //如果不存在key直接返回
            if (!Config.config.ContainsKey(key)) return key;
            //如果不是文本data直接返回
            if (!(Config.config[key][LanguageType.SimplifiedChinese] is LocalizationStringData))
                return key;

            //如果条件都符合就返回当前语言的文本内容
            return ((LocalizationStringData)Config.config[key][languageType]).content;
        }

        public static void RegisterLanguageEvent(Action<LanguageType> action)
        {
            EventBroadCastSystem.AddEventListener(OnUpdateLanguage, action);
        }

        public static void UnregisterLanguageEvent(Action<LanguageType> action)
        {
            EventBroadCastSystem.RemoveEventListener(OnUpdateLanguage, action);
        }
    }
}