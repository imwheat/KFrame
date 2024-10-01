//****************** 代码文件申明 ***********************
//* 文件：PausePanel
//* 作者：wheat
//* 创建时间：2024/10/01 10:16:18 星期二
//* 描述：游戏的暂停面板
//*******************************************************

#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;


namespace KFrame.UI
{
    [UIData(typeof(PausePanel), "PausePanel", true, 3)]
    public class PausePanel : UIPanelBase
    {
        #region UI配置

        /// <summary>
        /// 返回按钮
        /// </summary>
        [SerializeField] 
        private KButton returnBtn;
        /// <summary>
        /// 设置按钮
        /// </summary>
        [SerializeField] 
        private KButton settingsBtn;
        /// <summary>
        /// 返回菜单按钮
        /// </summary>
        [SerializeField] 
        private KButton menuBtn;
        /// <summary>
        /// 退出按钮
        /// </summary>
        [SerializeField] 
        private KButton exitBtn;

        #endregion

        protected override void Awake()
        {
            base.Awake();
            
            settingsBtn.OnClick.AddListener(OnClickSettingsBtn);
            exitBtn.OnClick.AddListener(OnClickExitBtn);
        }

        #region 按键事件
        
        /// <summary>
        /// 按下设置按钮
        /// </summary>
        private void OnClickSettingsBtn()
        {
            SwitchToThisPanel<SettingsPanel>(true);
        }
        /// <summary>
        /// 按下退出按钮
        /// </summary>
        private void OnClickExitBtn()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
                EditorApplication.ExitPlaymode();
            }
#else
            Application.Quit();
#endif

        }

        #endregion
    }
}

