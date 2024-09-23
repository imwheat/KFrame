//****************** 代码文件声明 ***********************
//* 文件：UIPanelBase
//* 作者：wheat
//* 创建时间：2024/09/15 08:20:32 星期日
//* 描述：面板类UI的基类、一般菜单类UI都是继承自这个
//*******************************************************
using System;
using System.Collections.Generic;
using KFrame;
using KFrame.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KFrame.UI
{

    public class UIPanelBase : UIBase
    {
        #region 参数
        
        /// <summary>
        /// 上一级面板
        /// </summary>
        protected UIPanelBase parent;
        /// <summary>
        /// 上一级面板
        /// </summary>
        public UIPanelBase Parent => parent;
        /// <summary>
        /// 关闭后切换的面板
        /// </summary>
        protected UIPanelBase switchPanel;
        /// <summary>
        /// 关闭后切换的面板
        /// </summary>
        public UIPanelBase SwitchPanel => switchPanel;
        /// <summary>
        /// UI面板的默认选项
        /// </summary>
        [SerializeField]
        protected Selectable defaultSelection;
        /// <summary>
        /// UI面板的默认选项
        /// </summary>
        public Selectable DefaultSelection => defaultSelection;
        /// <summary>
        /// 动画机
        /// </summary>
        [SerializeField]
        protected Animator anim;
        /// <summary>
        /// 正在播放动画
        /// </summary>
        protected bool isPlayingAnim;

        #endregion

        #region 生命周期

        protected virtual void Awake()
        {
            if (anim == null) anim = GetComponent<Animator>();
        }

        #endregion

        #region 方法重写
        
        /// <summary>
        /// 打开UI面板
        /// </summary>
        protected override void OnShow()
        {
            base.OnShow();
            //通知系统选择当前面板
            UISelectSystem.SelectPanel(this);
            //可见UI面板数量加一
            UISelectSystem.VisibleUIPanelCount++;
        }
        /// <summary>
        /// 关闭UI面板
        /// </summary>
        protected override void OnClose()
        {
            base.OnClose();
            
            //如果要切换的面板不为空，那就显示
            if (SwitchPanel != null)
            {
                SwitchPanel.Show();
            }
            //没有要切换的面板的话，那就取消选择面板
            else
            {
                UISelectSystem.DeselectPanel(this);
            }
            
            //可见UI面板数量减一
            UISelectSystem.VisibleUIPanelCount--;
        }

        #endregion

        #region UI面板操作
        
        /// <summary>
        /// 选择面板的UI默认选项
        /// </summary>
        public void SelectDefaultUI()
        {
            //如果默认选项不为空并且可以交互，那集选择
            if (defaultSelection != null && (!defaultSelection.IsActive() || !defaultSelection.interactable))
            {
                defaultSelection.Select();
            }
            else
            {
                //获取子集所有可以交互的选项
                Selectable[] selectables = transform.GetComponentsInChildren<Selectable>();
                
                //遍历每一个可选项
                foreach (Selectable selectable in selectables)
                {
                    //如果不能交互那就跳过
                    if (!defaultSelection.IsActive() || !defaultSelection.interactable) continue;
                    //找到可以选择的那就选择，然后停止遍历
                    selectable.Select();
                    break;
                }
            }
        }
        
        /// <summary>
        /// 关闭当前面板
        /// </summary>
        protected virtual void ClosePanel()
        {
            //如果没有动画机，那就直接关闭
            if (anim == null)
            {
                //关闭面板
                Close();
            }
            //如果有动画机，那就交给动画控制
            else
            {
                
            }

        }
        
        /// <summary>
        /// 绑定UI面板的上一级
        /// </summary>
        /// <param name="parent">上一级面板</param>
        public void SetParent(UIPanelBase parent)
        {
            this.parent = parent;
        }
        /// <summary>
        /// 关闭当前UI面板，切换到另一个UI面板
        /// </summary>
        /// <param name="switchPanel">要切换的UI面板</param>
        /// <param name="setParent">将这个面板设为另一个面板上一级</param>
        public void SwitchToThisPanel(UIPanelBase switchPanel, bool setParent)
        {
            //设置面板层级关系，然后关闭当前面板
            this.switchPanel = switchPanel;
            if (setParent)
            {
                switchPanel.SetParent(this);
            }
            ClosePanel();
        }
        /// <summary>
        /// 当点击了ESC
        /// </summary>
        public virtual void OnPressESC()
        {   
            //如果有上一级面板
            if (Parent != null)
            {
                //那就切换到上一级面板
                SwitchToThisPanel(Parent, false);
            }
            //如果没有上一级面板
            else
            {
                //那就关闭面板
                ClosePanel();
            }
        }

        #endregion
    }
}
