//****************** 代码文件申明 ***********************
//* 文件：UILocalizationHelper
//* 作者：wheat
//* 创建时间：2024/09/20 08:30:04 星期五
//* 描述：辅助进行UI本地化
//*******************************************************

using UnityEngine;
using KFrame;
using KFrame.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace KFrame.UI
{
    public static class UILocalizationHelper
    {
        /// <summary>
        /// 本地化配置数据
        /// </summary>
        public static LocalizationConfig Config => LocalizationConfig.Instance;
        
        /// <summary>
        /// 更新UI语言
        /// </summary>
        public static void UpdateUILanguage(Graphic component, string key, LanguageType languageType)
        {
            //如果传参有为空的就返回
            if (component == null || string.IsNullOrEmpty(key)) return;

            //先判断组件类型，然后根据对应类型获取数据进行本地化更新
            switch (component)
            {   
                case Text text:
                    if (LocalizationSystem.TryGetLocalizationData(key, languageType, out LocalizationStringData data))
                    {
                        text.text = data.content;
                    }
                    break;
                case TMP_Text tmpText:
                    if (LocalizationSystem.TryGetLocalizationData(key, languageType, out data))
                    {
                        tmpText.text = data.content;
                    }
                    break;
                case Image image:
                    if (LocalizationSystem.TryGetLocalizationData(key, languageType, out LocalizationImageData imgData))
                    {
                        image.sprite = imgData.content;
                    }
                    break;
            }
            
        }
        /// <summary>
        /// 更新UI语言
        /// </summary>
        public static void UpdateUILanguage(this UILocalizationData data, LanguageType languageType)
        {
            //如果为空就返回
            if(data == null) return;
            UpdateUILanguage(data.Component, data.Key, languageType);
        }
    }
}

