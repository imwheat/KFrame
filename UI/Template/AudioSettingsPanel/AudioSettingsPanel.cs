//****************** 代码文件申明 ***********************
//* 文件：AudioSettingsPanel
//* 作者：wheat
//* 创建时间：2024/10/02 18:18:13 星期三
//* 描述：音量的设置面板
//*******************************************************

using UnityEngine;

namespace KFrame.UI
{
    [UIData(typeof(AudioSettingsPanel), "AudioSettingsPanel", true, 3)]
    public class AudioSettingsPanel : UIPanelBase
    {
        #region UI配置

        /// <summary>
        /// 返回按钮
        /// </summary>
        [SerializeField] 
        private KButton returnBtn;
        /// <summary>
        /// 主音量调节滚轮
        /// </summary>
        [SerializeField] 
        private KSlider masterVolumeSlider;
        /// <summary>
        /// 音效音量调节滚轮
        /// </summary>
        [SerializeField] 
        private KSlider sfxVolumeSlider;
        /// <summary>
        /// BGM音量调节滚轮
        /// </summary>
        [SerializeField] 
        private KSlider bgmVolumeSlider;
        /// <summary>
        /// UI音量调节滚轮
        /// </summary>
        [SerializeField] 
        private KSlider uiVolumeSlider;
        
        #endregion

        protected override void Awake()
        {
            base.Awake();
            
            //按键事件注册
            returnBtn.OnClick.AddListener(OnPressESC);
        }

    }
}

