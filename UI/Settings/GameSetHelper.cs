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
        /// 只更新UI的语言，游戏语言需要另外更新
        /// 自行调用<see cref="KFrame.UI.LocalizationSystem.UpdateGameLanguage"/>>
        /// 重启游戏会自动调用，一般在开始游戏的时候使用更新
        /// </summary>
        /// <param name="languageType">语言类型</param>
        public static void SetLanguage(int languageType)
        {
            CurSettingsData.Language = languageType;
            LocalizationSystem.UILanguageType = languageType;
        }
        /// <summary>
        /// 加载游戏设置
        /// </summary>
        public static void LoadGameSettings()
        {
            LocalizationSystem.UpdateGameLanguage(CurSettingsData.Language);
        }
    }
}

