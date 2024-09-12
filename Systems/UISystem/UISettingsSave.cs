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
using Sirenix.OdinInspector;

namespace KFrame.Systems
{
    [System.Serializable]
    public class UISettingsSave
    {
        #region 游戏设置

        /// <summary>
        /// 游戏语言
        /// </summary>
        [LabelText("游戏语言"), TabGroup("游戏设置")]
        public LanguageType Language;

        #endregion

        #region 图像设置

        /// <summary>
        /// 全屏模式
        /// </summary>
        [LabelText("全屏模式"), TabGroup("图像设置")]
        public int FullScreenMode;
        /// <summary>
        /// 分辨率
        /// </summary>
        [LabelText("分辨率"), TabGroup("图像设置")]
        public Vector2Int Resolution;
        /// <summary>
        /// 最大帧率
        /// </summary>
        [LabelText("最大帧率"), TabGroup("图像设置")]
        public int FrameLimit;
        /// <summary>
        /// 垂直同步
        /// </summary>
        [LabelText("垂直同步"), TabGroup("图像设置")]
        public bool VSync = true;
        /// <summary>
        /// 选择的显示屏
        /// </summary>
        [LabelText("选择的显示屏"), TabGroup("图像设置")]
        public int SelectedScreen;
        /// <summary>
        /// 碎渣上限
        /// </summary>
        [LabelText("碎渣上限"), TabGroup("图像设置")]
        public int DustLimit;

        #endregion

        #region 音频设置

        /// <summary>
        /// 主音量
        /// </summary>
        [LabelText("主音量"), TabGroup("音频设置")]
        public float MainVolume;
        /// <summary>
        /// 音效音量
        /// </summary>
        [LabelText("音效音量"), TabGroup("音频设置")]
        public float SFXVolume;
        /// <summary>
        /// 音乐音量
        /// </summary>
        [LabelText("音乐音量"), TabGroup("音频设置")]
        public float BGMVolume;
        /// <summary>
        /// UI音量
        /// </summary>
        [LabelText("UI音量"), TabGroup("音频设置")]
        public float UIVolume;

        #endregion

        #region 按键设置

        public InputSetSaveData InputData;

        #endregion
        public UISettingsSave()
        {
            Language = LanguageType.SimplifiedChinese;

            FullScreenMode = 0;
            Resolution = new Vector2Int(Screen.width, Screen.height);
            FrameLimit = 60;
            VSync = true;
            SelectedScreen = 0;

            MainVolume = 1f;
            SFXVolume = 1f;
            BGMVolume = 1f;
            UIVolume = 1f;

            InputData = new InputSetSaveData();
        }
        /// <summary>
        /// 复制一份
        /// </summary>
        public UISettingsSave(UISettingsSave other)
        {
            this.Language = other.Language;

            this.FullScreenMode = other.FullScreenMode;
            this.Resolution = other.Resolution;
            this.FrameLimit = other.FrameLimit;
            this.VSync = other.VSync;
            this.SelectedScreen = other.SelectedScreen;
            this.DustLimit = other.DustLimit;

            this.MainVolume = other.MainVolume;
            this.SFXVolume = other.SFXVolume;
            this.BGMVolume = other.BGMVolume;
            this.UIVolume = other.UIVolume;

            InputData = other.InputData;
        }
        /// <summary>
        /// 恢复画面默认设置
        /// </summary>
        public void ResetGraphic()
        {
            FullScreenMode = 0;
            Resolution = new Vector2Int(Screen.width, Screen.height);
            FrameLimit = 60;
            VSync = true;
            SelectedScreen = 0;
            DustLimit = 500;
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

