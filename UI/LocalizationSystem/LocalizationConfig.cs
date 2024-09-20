//****************** 代码文件申明 ************************
//* 文件：LocalizationConfig                      
//* 作者：wheat
//* 创建时间：2024/09/19 星期四 16:08:22
//* 功能：本地化配置的一些数据
//*****************************************************


using System;
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
        #region 储存数据

        /// <summary>
        /// 本地化的文本数据
        /// </summary>
        public List<LocalizationStringData> TextDatas;
        /// <summary>
        /// 本地化的图片数据
        /// </summary>
        public List<LocalizationImageData> ImageDatas;
        

        #endregion

        #region 本地化搜索字典

        private Dictionary<LanguageType, Dictionary<string, string>> textDic;

        public Dictionary<LanguageType, Dictionary<string, string>> TextDic
        {
            get
            {
                if (textDic == null)
                {
                    Init();
                }

                return textDic;
            }
        }
        private Dictionary<LanguageType, Dictionary<string, Sprite>> imgDic;

        private Dictionary<LanguageType, Dictionary<string, Sprite>> ImgDic
        {
            get
            {
                if (imgDic == null)
                {
                    Init();
                }

                return imgDic;
            }
        }
        public Dictionary<LanguageType, int> LanguageFontSize
            = new Dictionary<LanguageType, int>();

        #endregion

        #region 初始化
        
        /// <summary>
        /// 初始化字典
        /// </summary>
        public void Init()
        {
            //注册语言类型的字典
            LanguageType[] languages = (LanguageType[])Enum.GetValues(typeof(LanguageType));
            textDic = new Dictionary<LanguageType, Dictionary<string, string>>();
            imgDic = new Dictionary<LanguageType, Dictionary<string, Sprite>>();
            foreach (LanguageType language in languages)
            {
                textDic[language] = new Dictionary<string, string>();
                imgDic[language] = new Dictionary<string, Sprite>();
            }
            //然后遍历每个数据然后添加入字典中
            foreach (LocalizationStringData stringData in TextDatas)
            {
                RegisterTextData(stringData);
            }
            foreach (LocalizationImageData imageData in ImageDatas)
            {
                RegisterImageData(imageData);
            }
        }
        /// <summary>
        /// 注册文本数据
        /// </summary>
        /// <param name="stringData">文本数据</param>
        private void RegisterTextData(LocalizationStringData stringData)
        {
            //遍历所有数据，然后注册进入字典
            foreach (LocalizationStringDataBase data in stringData.Datas)
            {
                textDic[data.Language][stringData.Key] = data.Text;
            }
        }
        /// <summary>
        /// 注册文本数据
        /// </summary>
        /// <param name="stringData">文本数据</param>
        private void RegisterImageData(LocalizationImageData imageData)
        {
            //遍历所有数据，然后注册进入字典
            foreach (LocalizationImageDataBase data in imageData.Datas)
            {
                imgDic[data.Language][imageData.Key] = data.Sprite;
            }
        }
        #endregion

        #region 获取本地化信息

        /// <summary>
        /// 获取本地化文本
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="languageType">语言类型</param>
        /// <returns>本地化后的文本，如果没有就返回""</returns>
        public string GetLocalizedText(string key, LanguageType languageType)
        {
            if (TextDic[languageType].TryGetValue(key, out string text))
            {
                return text;
            }
            
            return "";
        }
        /// <summary>
        /// 尝试获取本地化文本
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="languageType">语言类型</param>
        /// <returns>本如果没有就返回false</returns>
        public bool TryGetLocalizedText(string key, LanguageType languageType, out string text)
        {
            return TextDic[languageType].TryGetValue(key, out text);
        }
        /// <summary>
        /// 获取本地化文本
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="languageType">语言类型</param>
        /// <returns>本地化后的文本，如果没有就返回null</returns>
        public Sprite GetLocalizedImage(string key, LanguageType languageType)
        {
            if (ImgDic[languageType].TryGetValue(key, out Sprite sprite))
            {
                return sprite;
            }
            
            return null;
        }
        /// <summary>
        /// 尝试获取本地化文本
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="languageType">语言类型</param>
        /// <returns>本如果没有就返回false</returns>
        public bool TryGetLocalizedImage(string key, LanguageType languageType, out Sprite sprite)
        {
            return ImgDic[languageType].TryGetValue(key, out sprite);
        }

        #endregion
       

    }
}