//****************** 代码文件申明 ************************
//* 文件：UISettingsSave                                       
//* 作者：wheat
//* 创建时间：2024/03/08 10:11:12 星期五
//* 描述：用来保存UI的设置参数
//*****************************************************
using UnityEngine;

using System;
using System.Collections.Generic;
using KFrame;
using KFrame.Systems;
using Sirenix.OdinInspector;

namespace KFrame.UI
{
    [System.Serializable]
    public class UISettingsSave
    {
        /// <summary>
        /// 游戏设置
        /// </summary>
        public GameSettingsData GameData;
        
        /// <summary>
        /// 图像设置
        /// </summary>
        public GraphicSettingsData GraphicData;

        /// <summary>
        /// 音量设置
        /// </summary>
        public AudioSettingsData AudioData;

        #region 按键设置

        public InputSetSaveData InputData;

        #endregion
        public UISettingsSave()
        {
            GameData = new GameSettingsData();
            GraphicData = new GraphicSettingsData();
            AudioData = new AudioSettingsData();

            InputData = new InputSetSaveData();
        }
        /// <summary>
        /// 复制一份
        /// </summary>
        public UISettingsSave(UISettingsSave other)
        {
            GameData = other.GameData;
            GraphicData = other.GraphicData;
            AudioData = other.AudioData;
            InputData = other.InputData;
        }


        /// <summary>
        /// 获取玩家按键配置data
        /// </summary>
        /// <param name="playerIndex">玩家id</param>
        /// <param name="key">按键保存Key</param>
        /// <returns>如果找到的话那就返回对应按键的保存data，找不到的话那就返回空</returns>
        public string GetInputSet(int playerIndex, string key)
        {
            return InputData.GetPlayerKeySet(playerIndex, key);
        }
        /// <summary>
        /// 设置玩家按键配置data
        /// </summary>
        /// <param name="playerIndex">玩家id</param>
        /// <param name="key">按键保存Key</param>
        public void SetInputSet(int playerIndex, string key, string value)
        {
            InputData.SetPlayerKeySet(playerIndex, key, value);
        }
    }

}

