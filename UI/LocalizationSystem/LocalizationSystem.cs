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
        private static int languageType;
        /// <summary>
        /// 语言类型，设置时会自动分发语言修改事件
        /// </summary>
        public static int LanguageType
        {
            get
            {
                return languageType;
            }
            set
            {
                if (UISetPropertyUtility.SetStruct(ref languageType, value))
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
        /// 获取本地化文本数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <returns>没有的话返回""</returns>
        public static string GetLocalizedText(string key, int language)
        {
            if (Config == null)
            {
                Debug.LogWarning("缺少本地化Config");
                return null;
            }

            return Config.GetLocalizedText(key, language);
        }
        /// <summary>
        /// 获取当前语言类型的本地化文本数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>没有的话返回""</returns>
        public static string GetLocalizedTextInCurLanguage(string key)
        {
            return GetLocalizedText(key, LanguageType);
        }
        /// <summary>
        /// 尝试获取本地化文本数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <returns>没有的话返回false</returns>
        public static bool TryGetLocalizedText(string key, LanguageType language, out string text)
        {
            return Config.TryGetLocalizedText(key, (int)language, out text);
        }
        /// <summary>
        /// 获取本地化图片数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <returns>没有的话返回null</returns>
        public static Sprite GetLocalizedImage(string key, LanguageType language)
        {
            if (Config == null)
            {
                Debug.LogWarning("缺少本地化Config");
                return null;
            }

            return Config.GetLocalizedImage(key, (int)language);
        }
        /// <summary>
        /// 获取当前语言类型的本地化图片数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>没有的话返回null</returns>
        public static Sprite GetLocalizedImageInCurLanguage(string key)
        {
            return GetLocalizedImage(key, (LanguageType)LanguageType);
        }
        /// <summary>
        /// 尝试获取本地化文本数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <returns>没有的话返回false</returns>
        public static bool TryGetLocalizedImage(string key, LanguageType language, out Sprite sprite)
        {
            return Config.TryGetLocalizedImage(key, (int)language, out sprite);
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