//****************** 代码文件申明 ***********************
//* 文件：GraphicSettingsPanel
//* 作者：wheat
//* 创建时间：2024/10/03 08:39:14 星期四
//* 描述：音量的设置面板
//*******************************************************

using UnityEngine;

namespace KFrame.UI
{
    [UIData(typeof(GraphicSettingsPanel), "GraphicSettingsPanel", true, 3)]
    public class GraphicSettingsPanel : UIPanelBase
    {
        #region UI配置

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

