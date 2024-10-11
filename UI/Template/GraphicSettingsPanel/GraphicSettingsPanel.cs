//****************** 代码文件申明 ***********************
//* 文件：GraphicSettingsPanel
//* 作者：wheat
//* 创建时间：2024/10/03 08:39:14 星期四
//* 描述：音量的设置面板
//*******************************************************

using System.Collections.Generic;
using KFrame.Utilities;
using TMPro;
using UnityEngine;

namespace KFrame.UI
{
    [UIData(typeof(GraphicSettingsPanel), "GraphicSettingsPanel", true, 3)]
    public class GraphicSettingsPanel : UIPanelBase
    {
        #region UI配置

        
        /// <summary>
        /// 全屏按钮
        /// </summary>
        [SerializeField] 
        private KSwitchButton fullScreenBtn;
        /// <summary>
        /// 全屏文本
        /// </summary>
        [SerializeField]
        private TMP_Text fullScreenText;
        /// <summary>
        /// 分辨率按钮
        /// </summary>
        [SerializeField] 
        private KSwitchButton resolutionBtn;
        /// <summary>
        /// 分辨率文本
        /// </summary>
        [SerializeField]
        private TMP_Text resolutionText;
        /// <summary>
        /// 垂直同步按钮
        /// </summary>
        [SerializeField] 
        private KSwitchButton vSyncBtn;
        /// <summary>
        /// 垂直同步文本
        /// </summary>
        [SerializeField]
        private TMP_Text vSyncText;
        /// <summary>
        /// 帧率限制按钮
        /// </summary>
        [SerializeField] 
        private KSwitchButton frameRateCapBtn;
        /// <summary>
        /// 帧率限制文本
        /// </summary>
        [SerializeField]
        private TMP_Text frameRateCapText;
        /// <summary>
        /// 应用设置按钮
        /// </summary>
        [SerializeField] 
        private KButton applyBtn;
        /// <summary>
        /// 返回按钮
        /// </summary>
        [SerializeField] 
        private KButton returnBtn;
        
        #endregion

        #region UI配置参数

        /// <summary>
        /// 全屏配置选项
        /// 窗口化全屏、独占全屏、窗口化
        /// </summary>
        private static readonly int[] FullScreenConfig = new[] { 0, 1, 3 };
        /// <summary>
        /// 全屏化配置UI本地化文本key
        /// </summary>
        private static readonly string[] FullScreenConfigUIKey =
            new[] { "ui_FullScreenConfig", "ui_FullScreenWindowConfig", "ui_WindowConfig" };
        /// <summary>
        /// 分辨率配置选项
        /// </summary>
        private List<Vector2Int> resolutionConfig;
        /// <summary>
        /// 常规的默认分辨率配置
        /// </summary>
        private static readonly Vector2Int[] DefaultResolutionConfigs = new[] { new Vector2Int(640, 480) , new Vector2Int(800, 600), new Vector2Int(1280, 720), new Vector2Int(1366, 768), new Vector2Int(1600, 900),new Vector2Int(1920,1080), new Vector2Int(2560, 1440), new Vector2Int(3840,2160)};
        /// <summary>
        /// 帧率限制选项
        /// </summary>
        private List<int> frameRateCapConfig;
        
        #endregion

        #region 生命周期

        protected override void Awake()
        {
            base.Awake();
            
            //按键事件注册
            returnBtn.OnClick.AddListener(OnPressESC);
            fullScreenBtn.OnClick.AddListener(OnSwitchFullScreen);
            resolutionBtn.OnClick.AddListener(OnSwitchResolution);
            vSyncBtn.OnClick.AddListener(OnSwitchVsync);
            frameRateCapBtn.OnClick.AddListener(OnSwitchFrameRateCap);
            applyBtn.OnClick.AddListener(OnApplyModify);
        }

        protected override void OnShow()
        {
            base.OnShow();
            
            InitUI();
        }

        #endregion


        #region 初始化
        
        /// <summary>
        /// 初始化UI选项
        /// </summary>
        private void InitUI()
        {
            //更新当前全屏状态
            var fullScreenMode = FullScreenConfig.FindElementIndex((int)Screen.fullScreenMode, 1);
            GraphicSetHelper.CurSettingsData.fullScreenMode = (FullScreenMode)fullScreenMode;
            //更新UI
            fullScreenBtn.Range = new Vector2Int(0, 3);
            fullScreenBtn.SetValue(fullScreenMode, true);

            //更新分辨率配置
            var maxWidth = Screen.currentResolution.width;
            var maxHeight = Screen.currentResolution.height;
            resolutionConfig = new List<Vector2Int>();
            //遍历每个常规选项
            foreach (var resolution in DefaultResolutionConfigs)
            {
                //如果超过了那就终止
                if(resolution.x > maxWidth || resolution.y > maxHeight) break;
                resolutionConfig.Add(resolution);
            }
            //如果分辨率选项里面没有这个选项再补充
            if (!resolutionConfig.Contains(new Vector2Int(maxWidth, maxHeight)))
            {
                resolutionConfig.Add(new Vector2Int(maxWidth, maxHeight));
            }
            GraphicSetHelper.CurSettingsData.resolution = new Vector2Int(Screen.width, Screen.height);
            //更新UI
            resolutionBtn.Range = new Vector2Int(0, resolutionConfig.Count);
            resolutionBtn.SetValue(resolutionConfig.FindElementIndex(GraphicSetHelper.CurSettingsData.resolution, resolutionConfig.Count -1), true);
            
            //更新垂直同步
            var vSync = QualitySettings.vSyncCount == 1;
            GraphicSetHelper.CurSettingsData.vSync = vSync;
            //更新UI
            vSyncBtn.Range = new Vector2Int(0, 2);
            vSyncBtn.SetValue(vSync? 1: 0, true);
            
            //帧率限制
            var maxFrameRate = Screen.currentResolution.refreshRate;
            frameRateCapConfig = new List<int>(){-1};
            for (var i = 30; i < maxFrameRate; i+= 30)
            {
                frameRateCapConfig.Add(i);
            }
            frameRateCapConfig.Add(maxFrameRate);
            GraphicSetHelper.CurSettingsData.frameRate = Application.targetFrameRate;
            //更新UI
            frameRateCapBtn.Range = new Vector2Int(0, frameRateCapConfig.Count);
            frameRateCapBtn.SetValue(frameRateCapConfig.FindElementIndex(Application.targetFrameRate), true);
            
        }

        #endregion
        
        #region UI事件

        /// <summary>
        /// 切换全屏
        /// </summary>
        /// <param name="v">值</param>
        private void OnSwitchFullScreen(int v)
        {
            fullScreenText.text = LocalizationSystem.GetUITextInCurLanguage(FullScreenConfigUIKey[v]);
        }
        /// <summary>
        /// 切换分辨率
        /// </summary>
        /// <param name="v">值</param>
        private void OnSwitchResolution(int v)
        {
            var resolution = resolutionConfig[v];
            resolutionText.text = resolution.x + "x" + resolution.y;
        }
        /// <summary>
        /// 开关垂直同步
        /// </summary>
        /// <param name="v">值</param>
        private void OnSwitchVsync(int v)
        {
            vSyncText.text = LocalizationSystem.GetUITextInCurLanguage(UISystem.GetOnOffUIKey(v == 1));
        }
        /// <summary>
        /// 切换帧率限制
        /// </summary>
        /// <param name="v">值</param>
        private void OnSwitchFrameRateCap(int v)
        {
            var rate = frameRateCapConfig[v];
            frameRateCapText.text = rate == -1 ? LocalizationSystem.GetUITextInCurLanguage(UISystem.GetOnOffUIKey(false)) : rate.ToString();
        }
        /// <summary>
        /// 按下应用设置按钮
        /// </summary>
        private void OnApplyModify()
        {
            //保存画面设置
            GraphicSettingsData data = GraphicSetHelper.CurSettingsData;
            data.fullScreenMode = (FullScreenMode)FullScreenConfig[fullScreenBtn.Value];
            data.resolution = resolutionConfig[resolutionBtn.Value];
            data.vSync = vSyncBtn.Value == 1;
            data.frameRate = frameRateCapConfig[frameRateCapBtn.Value];
            
            //保存UI设置
            UISystem.SaveUISettings();
            
            //应用设置
            GraphicSetHelper.LoadGraphicSettings();
        }

        #endregion
    }
}

