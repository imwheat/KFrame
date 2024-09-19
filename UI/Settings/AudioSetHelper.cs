//****************** 代码文件申明 ***********************
//* 文件：AudioSetHelper
//* 作者：wheat
//* 创建时间：2024/09/19 09:12:13 星期四
//* 描述：帮忙处理音效方面变化的
//*******************************************************


using KFrame.Systems;

namespace KFrame.UI
{
    public class AudioSetHelper
    {
        
        /// <summary>
        /// 当前的设置数据
        /// </summary>
        private static AudioSettingsData curSettingsData;
        /// <summary>
        /// 当前的设置数据
        /// </summary>
        public static AudioSettingsData CurSettingsData
        {
            get
            {
                if (curSettingsData == null)
                {
                    curSettingsData = UISystem.Settings.AudioData;
                }

                return curSettingsData;
            }
        }
        /// <summary>
        /// 设置主音量
        /// </summary>
        public static void SetMasterVolume(float value)
        {
            AudioSystem.MasterVolume = value;
            CurSettingsData.MasterVolume = value;
        }
        /// <summary>
        /// 设置音乐音量
        /// </summary>
        public static void SetBGMVolume(float value)
        {
            AudioSystem.BGMVolume = value;
            CurSettingsData.BGMVolume = value;
        }
        /// <summary>
        /// 设置音效音量
        /// </summary>
        public static void SetSFXVolume(float value)
        {
            AudioSystem.SFXVolume = value;
            CurSettingsData.SFXVolume = value;
        }
    }
}

