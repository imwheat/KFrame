//****************** 代码文件申明 ***********************
//* 文件：SettingsPanel
//* 作者：wheat
//* 创建时间：2024/10/01 10:16:18 星期二
//* 描述：游戏的设置面板
//*******************************************************

using UnityEngine;

namespace KFrame.UI
{
    [UIData(typeof(SettingsPanel), "SettingsPanel", true, 3)]
    public class SettingsPanel : UIPanelBase
    {
        #region UI配置

        /// <summary>
        /// 游戏设置按钮
        /// </summary>
        [SerializeField] 
        private KButton gameSettingsBtn;
        /// <summary>
        /// 音效设置按钮
        /// </summary>
        [SerializeField] 
        private KButton audioSettingsBtn;
        /// <summary>
        /// 画面设置按钮
        /// </summary>
        [SerializeField] 
        private KButton graphicSettingsBtn;
        /// <summary>
        /// 按键设置按钮
        /// </summary>
        [SerializeField] 
        private KButton inputSettingsBtn;
        /// <summary>
        /// 返回按钮
        /// </summary>
        [SerializeField] 
        private KButton returnBtn;

        #endregion

        protected override void Awake()
        {
            base.Awake();
            
            //按键事件注册
            returnBtn.OnClick.AddListener(OnPressESC);
        }
    }
}

