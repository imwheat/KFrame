//****************** 代码文件声明 ***********************
//* 文件：GraphicSetHelper
//* 作者：wheat
//* 创建时间：2024/09/19 08:12:24 星期四
//* 描述：帮忙处理画面方面的设置
//*******************************************************
using System;
using UnityEngine;

namespace KFrame.UI
{

    public static class GraphicSetHelper
    {
        /// <summary>
        /// 分辨率更新事件
        /// </summary>
        public static Action OnResolutionUpdated;
        /// <summary>
        /// 备份的设置数据
        /// 用于还原设置
        /// </summary>
        private static GraphicSettingsData backupSettingsData;
        /// <summary>
        /// 当前的设置数据
        /// </summary>
        private static GraphicSettingsData curSettingsData;
        /// <summary>
        /// 当前的设置数据
        /// </summary>
        public static GraphicSettingsData CurSettingsData
        {
            get
            {
                if (curSettingsData == null)
                {
                    curSettingsData = UISystem.Settings.GraphicData;
                }

                return curSettingsData;
            }
        }
        /// <summary>
        /// 设置垂直同步
        /// </summary>
        /// <param name="vsync">是否启用</param>
        public static void SetVsync(bool vsync)
        {
            QualitySettings.vSyncCount = vsync ? 1 : 0;
            CurSettingsData.vSync = vsync;
        }
        /// <summary>
        /// 设置帧数上限
        /// </summary>
        /// <param name="frameRate">帧数上限</param>
        public static void SetFrameRate(int frameRate)
        {
            Application.targetFrameRate = frameRate;
            CurSettingsData.frameRate = frameRate;
        }
        /// <summary>
        /// 设置显示相关，分辨率、全屏
        /// </summary>
        /// <param name="resolution">分辨率</param>
        /// <param name="fullScreenMode">全屏模式</param>
        public static void SetDisplay(Vector2Int resolution, FullScreenMode fullScreenMode)
        {

            //只有当前屏幕显示情况和设置不一样的时候才设置
            if (Screen.width!= resolution.x || Screen.height != resolution.y || Screen.fullScreenMode != fullScreenMode)
            {
                //设置显示模式
                Screen.SetResolution(resolution.x, resolution.y, fullScreenMode);
                
                //更新保存数据
                CurSettingsData.resolution = resolution;
                CurSettingsData.fullScreenMode = fullScreenMode;
                
                //调用分辨率更新事件
                if(Screen.width != resolution.x || Screen.height != resolution.y)
                {
                    OnResolutionUpdated?.Invoke();
                }
            }
        }
        /// <summary>
        /// 加载画面设置
        /// </summary>
        public static void LoadGraphicSettings()
        {
            SetVsync(CurSettingsData.vSync);
            SetFrameRate(CurSettingsData.frameRate);
            SetDisplay(CurSettingsData.resolution, CurSettingsData.fullScreenMode);
        }
        /// <summary>
        /// 恢复备份设置
        /// </summary>
        public static void RestoreBackup()
        {
            //如果没有备份数据那就返回
            if (backupSettingsData == null)
            {
                //备份一下现在的数据然后返回
                BackupSettings();
                return;
            }
            //复制备份的数据然后重新加载设置
            CurSettingsData.CopyData(backupSettingsData);
            LoadGraphicSettings();
        }
        /// <summary>
        /// 备份设置
        /// </summary>
        public static void BackupSettings()
        {
            backupSettingsData = new GraphicSettingsData(CurSettingsData);
        }
        /// <summary>
        /// 恢复默认设置
        /// </summary>
        public static void RestoreDefaultSettings()
        {
            //恢复默认数据然后加载设置
            CurSettingsData.ResetData();
            BackupSettings();
            LoadGraphicSettings();
        }
    }
}
