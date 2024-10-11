//****************** 代码文件申明 ***********************
//* 文件：GameSettingsPanel
//* 作者：wheat
//* 创建时间：2024/10/02 08:52:18 星期三
//* 描述：游戏的设置面板
//*******************************************************

using UnityEngine;
using UnityEngine.Serialization;

namespace KFrame.UI
{
    [UIData(typeof(GameSettingsPanel), "GameSettingsPanel", true, 3)]
    public class GameSettingsPanel : UIPanelBase
    {
        #region UI配置

        /// <summary>
        /// 语言设置按钮
        /// </summary>
        [SerializeField] 
        private KSwitchButton languageButton;
        /// <summary>
        /// 返回按钮
        /// </summary>
        [SerializeField] 
        private KButton returnBtn;

        #endregion

        protected override void Awake()
        {
            base.Awake();
            
            //更新值
            languageButton.SetValue((int)LocalizationSystem.UILanguageType, false);
            
            //按键事件注册
            returnBtn.OnClick.AddListener(OnPressESC);
            languageButton.OnClick.AddListener(SwitchLanguage);
        }
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="v">语言的值</param>
        private void SwitchLanguage(int v)
        {
            GameSetHelper.SetLanguage(v);
        }
    }
}

