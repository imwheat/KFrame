//****************** 代码文件申明 ***********************
//* 文件：GraphicSettingsData
//* 作者：wheat
//* 创建时间：2024/09/19 08:20:53 星期四
//* 描述：画面设置的存档数据
//*******************************************************

using UnityEngine;

namespace KFrame.UI
{
    [System.Serializable]
    public class GraphicSettingsData
    {
        /// <summary>
        /// 全屏模式
        /// </summary>
        public FullScreenMode fullScreenMode;
        /// <summary>
        /// 分辨率
        /// </summary>
        public Vector2Int resolution;
        /// <summary>
        /// 最大帧率
        /// </summary>
        public int frameRate;
        /// <summary>
        /// 垂直同步
        /// </summary>
        public bool vSync;


        public GraphicSettingsData()
        {
            fullScreenMode = FullScreenMode.FullScreenWindow;
            resolution = new Vector2Int(Screen.width, Screen.height);
            frameRate = 60;
            vSync = true;
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
            fullScreenMode = other.fullScreenMode;
            resolution = other.resolution;
            frameRate = other.frameRate;
            vSync = other.vSync;
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

