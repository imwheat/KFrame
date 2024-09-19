//****************** 代码文件申明 ***********************
//* 文件：GraphicSettingsData
//* 作者：wheat
//* 创建时间：2024/09/19 08:20:53 星期四
//* 描述：画面设置的存档数据
//*******************************************************

using UnityEngine;

namespace KFrame.UI
{
    public class GraphicSettingsData
    {
        /// <summary>
        /// 全屏模式
        /// </summary>
        public FullScreenMode FullScreenMode;
        /// <summary>
        /// 分辨率
        /// </summary>
        public Vector2Int Resolution;
        /// <summary>
        /// 最大帧率
        /// </summary>
        public int FrameRate;
        /// <summary>
        /// 垂直同步
        /// </summary>
        public bool VSync;
        /// <summary>
        /// 选择的显示屏
        /// </summary>
        public int SelectedScreen;

        public GraphicSettingsData()
        {
            FullScreenMode = 0;
            Resolution = new Vector2Int(Screen.width, Screen.height);
            FrameRate = 60;
            VSync = true;
            SelectedScreen = 0;
        }

        /// <summary>
        /// 新建一个数据一样的
        /// </summary>
        /// <param name="other">要拷贝的数据</param>
        public GraphicSettingsData(GraphicSettingsData other)
        {
            CopyData(other);
        }
        
        /// <summary>
        /// 从另一份数据那边复制一份
        /// </summary>
        /// <param name="other">要拷贝的数据</param>
        public void CopyData(GraphicSettingsData other)
        {
            FullScreenMode = other.FullScreenMode;
            Resolution = other.Resolution;
            FrameRate = other.FrameRate;
            VSync = other.VSync;
            SelectedScreen = other.SelectedScreen;
        }
        /// <summary>
        /// 恢复默认设置
        /// </summary>
        public void ResetData()
        {
            GraphicSettingsData newData = new GraphicSettingsData();
            CopyData(newData);
        }
    }
}

