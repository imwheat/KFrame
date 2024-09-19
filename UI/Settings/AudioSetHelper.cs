//****************** 代码文件申明 ***********************
//* 文件：AudioSetHelper
//* 作者：wheat
//* 创建时间：2024/09/19 09:12:13 星期四
//* 描述：帮忙处理音效方面变化的
//*******************************************************


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
    }
}

