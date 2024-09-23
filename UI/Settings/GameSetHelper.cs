//****************** 代码文件申明 ***********************
//* 文件：GameSetHelper
//* 作者：wheat
//* 创建时间：2024/09/19 08:57:13 星期四
//* 描述：帮忙处理游戏设置方面的东西
//*******************************************************

using UnityEngine;
using KFrame;
using KFrame.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using KFrame.Systems;

namespace KFrame.UI
{
    public static class GameSetHelper
    {

        /// <summary>
        /// 当前的设置数据
        /// </summary>
        private static GameSettingsData curSettingsData;
        /// <summary>
        /// 当前的设置数据
        /// </summary>
        public static GameSettingsData CurSettingsData
        {
            get
            {
                if (curSettingsData == null)
                {
                    curSettingsData = UISystem.Settings.GameData;
                }

                return curSettingsData;
            }
        }
        /// <summary>
        /// 设置语言
        /// </summary>
        /// <param name="languageType">语言类型</param>
        public static void SetLanguage(LanguageType languageType)
        {
            CurSettingsData.Language = languageType;
            LocalizationSystem.LanguageType = languageType;
        }
    }
}

