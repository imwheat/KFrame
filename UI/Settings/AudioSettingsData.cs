//****************** 代码文件申明 ***********************
//* 文件：AudioSettingsData
//* 作者：wheat
//* 创建时间：2024/09/19 09:13:14 星期四
//* 描述：音量设置的数据
//*******************************************************

namespace KFrame.UI
{
    [System.Serializable]
    public class AudioSettingsData
    {
        
        /// <summary>
        /// 主音量
        /// </summary>
        public float MasterVolume;
        /// <summary>
        /// 音效音量
        /// </summary>
        public float SFXVolume;
        /// <summary>
        /// 音乐音量
        /// </summary>
        public float BGMVolume;
        /// <summary>
        /// UI音量
        /// </summary>
        public float UIVolume;

        public AudioSettingsData()
        {
            MasterVolume = 1f;
            SFXVolume = 1f;
            BGMVolume = 1f;
            UIVolume = 1f;
        }

        /// <summary>
        /// 新建一个数据一样的
        /// </summary>
        /// <param name="other">要拷贝的数据</param>
        public AudioSettingsData(AudioSettingsData other)
        {
            CopyData(other);
        }
        /// <summary>
        /// 从另一份数据那边复制一份
        /// </summary>
        /// <param name="other">要拷贝的数据</param>
        public void CopyData(AudioSettingsData other)
        {
            MasterVolume = other.MasterVolume;
            SFXVolume = other.SFXVolume;
            BGMVolume = other.BGMVolume;
            UIVolume = other.UIVolume;
        }
        /// <summary>
        /// 恢复默认设置
        /// </summary>
        public void ResetData()
        {
            AudioSettingsData newData = new AudioSettingsData();
            CopyData(newData);
        }
    }
}

