//****************** 代码文件申明 ***********************
//* 文件：AudioSettingsPanel
//* 作者：wheat
//* 创建时间：2024/10/02 18:18:13 星期三
//* 描述：音量的设置面板
//*******************************************************

using KFrame.Systems;
using TMPro;
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
        /// 主音量百分比文本
        /// </summary>
        [SerializeField]
        private TMP_Text masterVolumePercentText;
        /// <summary>
        /// 音效音量调节滚轮
        /// </summary>
        [SerializeField] 
        private KSlider sfxVolumeSlider;
        /// <summary>
        /// 音效音量百分比文本
        /// </summary>
        [SerializeField]
        private TMP_Text sfxVolumePercentText;
        /// <summary>
        /// BGM音量调节滚轮
        /// </summary>
        [SerializeField] 
        private KSlider bgmVolumeSlider;
        /// <summary>
        /// BGM音量百分比文本
        /// </summary>
        [SerializeField]
        private TMP_Text bgmVolumePercentText;
        /// <summary>
        /// UI音量调节滚轮
        /// </summary>
        [SerializeField] 
        private KSlider uiVolumeSlider;
        /// <summary>
        /// UI音量百分比文本
        /// </summary>
        [SerializeField]
        private TMP_Text uiVolumePercentText;
        
        #endregion

        #region 生命周期

        protected override void Awake()
        {
            base.Awake();
            
            //按键事件注册
            masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolume);
            sfxVolumeSlider.onValueChanged.AddListener(UpdateSFXVolume);
            bgmVolumeSlider.onValueChanged.AddListener(UpdateBGMVolume);
            uiVolumeSlider.onValueChanged.AddListener(UpdateUIVolume);
            returnBtn.OnClick.AddListener(OnPressESC);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            UpdateUI();
        }

        #endregion
    
        /// <summary>
        /// 更新UI
        /// </summary>
        private void UpdateUI()
        {
            masterVolumeSlider.value = AudioSystem.MasterVolume;
            sfxVolumeSlider.value = AudioSystem.SFXVolume;
            bgmVolumeSlider.value = AudioSystem.BGMVolume;
            uiVolumeSlider.value = AudioSystem.UIVolume;
        }

        #region UI事件

        /// <summary>
        /// 更新主音量
        /// </summary>
        /// <param name="v">音量</param>
        private void UpdateMasterVolume(float v)
        {
            AudioSetHelper.SetMasterVolume(v);
            masterVolumePercentText.text = (v * 100).ToString("F0") + "%";
        }
        /// <summary>
        /// 更新音效音量
        /// </summary>
        /// <param name="v">音量</param>
        private void UpdateSFXVolume(float v)
        {
            AudioSetHelper.SetSFXVolume(v);
            sfxVolumePercentText.text = (v * 100).ToString("F0") + "%";
        }
        /// <summary>
        /// 更新BGM音量
        /// </summary>
        /// <param name="v">音量</param>
        private void UpdateBGMVolume(float v)
        {
            AudioSetHelper.SetBGMVolume(v);
            bgmVolumePercentText.text = (v * 100).ToString("F0") + "%";
        }
        /// <summary>
        /// 更新UI音量
        /// </summary>
        /// <param name="v">音量</param>
        private void UpdateUIVolume(float v)
        {
            AudioSetHelper.SetUIVolume(v);
            uiVolumePercentText.text = (v * 100).ToString("F0") + "%";
        }

        #endregion


    }
}

