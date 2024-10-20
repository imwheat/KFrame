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

        #region 属性参数

        private const string OnUpdateLanguage = "OnUpdateLanguage";
        private const string OnUIUpdateLanguage = "OnUIUpdateLanguage";
        
        /// <summary>
        /// UI语言类型
        /// </summary>
        private static int uiLanguageType = -1;
        /// <summary>
        /// UI语言类型，设置时会自动分发语言修改事件
        /// </summary>
        public static int UILanguageType
        {
            get
            {
                return uiLanguageType;
            }
            set
            {
                if (UISetPropertyUtility.SetStruct(ref uiLanguageType, value))
                {
                    OnUILanguageValueChanged();
                }
            }
        }
        /// <summary>
        /// 语言类型
        /// </summary>
        private static int languageType = -1;
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
        public static LocalizationConfig Config => LocalizationConfig.Instance;

        #endregion

        #region 语言配置

        public static void Init()
        {
            LocalizationDic.Init();
        }
        /// <summary>
        /// 更新游戏语言类型
        /// </summary>
        /// <param name="setType">设置语言类型</param>
        public static void UpdateGameLanguage(int setType)
        {
            LanguageType = setType;
            UILanguageType = setType;
        }

        #endregion

        #region UI数据获取

        /// <summary>
        /// 获取当前语言类型的本地化文本数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>没有的话返回""</returns>
        public static string GetLocalizedText(string key)
        {
            return LocalizationDic.GetLocalizedText(key);
        }

        /// <summary>
        /// 尝试获取当前语言类型的本地化文本数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="text">输出文本</param>
        /// <returns>没有的话返回false</returns>
        public static bool TryGetLocalizedText(string key, out string text)
        {
            return LocalizationDic.TryGetLocalizedText(key,out text);
        }
        /// <summary>
        /// 获取本地化的UI文本数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <returns>没有的话返回""</returns>
        public static string GetUIText(string key, int language)
        {
            if (Config == null)
            {
                Debug.LogWarning("缺少本地化Config");
                return null;
            }

            return LocalizationDic.GetUIText(key, language);
        }
        /// <summary>
        /// 获取当前语言类型的本地化UI文本数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>没有的话返回""</returns>
        public static string GetUITextInCurLanguage(string key)
        {
            return GetUIText(key, UILanguageType);
        }

        /// <summary>
        /// 尝试获取本地化UI文本数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <param name="text">输出文本</param>
        /// <returns>没有的话返回false</returns>
        public static bool TryGetUIText(string key, int language, out string text)
        {
            return LocalizationDic.TryGetUIText(key, language, out text);
        }
        /// <summary>
        /// 获取本地化UI图片数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <returns>没有的话返回null</returns>
        public static Sprite GetUIImage(string key, int language)
        {
            if (Config == null)
            {
                Debug.LogWarning("缺少本地化Config");
                return null;
            }

            return LocalizationDic.GetUIImage(key, language);
        }
        /// <summary>
        /// 获取当前语言类型的本地化UI图片数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>没有的话返回null</returns>
        public static Sprite GetUIImageInCurLanguage(string key)
        {
            return GetUIImage(key, UILanguageType);
        }

        /// <summary>
        /// 尝试获取本地化文本数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">语言类型</param>
        /// <param name="sprite">输出图片</param>
        /// <returns>没有的话返回false</returns>
        public static bool TryGetUIImage(string key, int language, out Sprite sprite)
        {
            return LocalizationDic.TryGetUIImage(key, language, out sprite);
        }

        #endregion

        #region 事件
        
        /// <summary>
        /// UI语言更新了
        /// </summary>
        private static void OnUILanguageValueChanged()
        {
            EventBroadCastSystem.EventTrigger(OnUIUpdateLanguage, uiLanguageType);
        }
        /// <summary>
        /// 游戏语言更新了
        /// </summary>
        private static void OnLanguageValueChanged()
        {
            //加载语言包
            LocalizationDic.LoadLanguagePackage(languageType);
            EventBroadCastSystem.EventTrigger(OnUpdateLanguage, LanguageType);
        }
        
        /// <summary>
        /// 注册语言更新事件
        /// </summary>
        /// <param name="action">事件</param>
        public static void RegisterLanguageEvent(Action<int> action)
        {
            EventBroadCastSystem.AddEventListener(OnUpdateLanguage, action);
        }
        /// <summary>
        /// 注册语言更新事件
        /// </summary>
        /// <param name="action">事件</param>
        public static void UnregisterLanguageEvent(Action<int> action)
        {
            EventBroadCastSystem.RemoveEventListener(OnUpdateLanguage, action);
        }
        /// <summary>
        /// 注册UI语言更新事件
        /// </summary>
        /// <param name="action">事件</param>
        public static void RegisterUILanguageEvent(Action<int> action)
        {
            EventBroadCastSystem.AddEventListener(OnUIUpdateLanguage, action);
        }
        /// <summary>
        /// 注销UI语言更新事件
        /// </summary>
        /// <param name="action">事件</param>
        public static void UnregisterUILanguageEvent(Action<int> action)
        {
            EventBroadCastSystem.RemoveEventListener(OnUIUpdateLanguage, action);
        }

        #endregion

    }
}