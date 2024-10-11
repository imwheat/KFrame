//****************** 代码文件申明 ***********************
//* 文件：GameSettingsData
//* 作者：wheat
//* 创建时间：2024/09/19 08:58:25 星期四
//* 描述：游戏设置方面的数据
//*******************************************************

using KFrame.Systems;

namespace KFrame.UI
{
    [System.Serializable]
    public class GameSettingsData
    {
        /// <summary>
        /// 游戏语言
        /// </summary>
        public int Language;

        public GameSettingsData()
        {
            Language = 0;
        }

        /// <summary>
        /// 新建一个数据一样的
        /// </summary>
        /// <param name="other">要拷贝的数据</param>
        public GameSettingsData(GameSettingsData other)
        {
            CopyData(other);
        }
        /// <summary>
        /// 从另一份数据那边复制一份
        /// </summary>
        /// <param name="other">要拷贝的数据</param>
        public void CopyData(GameSettingsData other)
        {
            Language = other.Language;
        }
        /// <summary>
        /// 恢复默认设置
        /// </summary>
        public void ResetData()
        {
            GameSettingsData newData = new GameSettingsData();
            CopyData(newData);
        }
    }
}

