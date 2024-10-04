//****************** 代码文件声明 ***********************
//* 文件：UISelectSystem
//* 作者：wheat
//* 创建时间：2024/09/15 08:16:29 星期日
//* 描述：管理控制UI的选择、导航的系统
//*******************************************************
using System;
using System.Collections.Generic;
using KFrame;
using KFrame.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KFrame.UI
{

    public static class UISelectSystem
    {
        /// <summary>
        /// 可见UI面板数量
        /// </summary>
        private static int visibleUIPanelCount;
        /// <summary>
        /// 可见UI面板数量
        /// </summary>
        public static int VisibleUIPanelCount
        {
            get
            {
                return visibleUIPanelCount;
            }
            set
            {
                visibleUIPanelCount = value;
                if (value == 0)
                {
                    OnExitUIPanelState();
                }
                else if (value == 1)
                {
                    OnEnterUIPanelState();
                }
            }

        }
        #pragma warning disable
        /// <summary>
        /// 是否在UI面板
        /// </summary>
        private static bool inTheUIPanel;

        /// <summary>
        /// 是否在UI面板
        /// </summary>
        public static bool InTheUIPanel => inTheUIPanel;
        #pragma warning restore
        /// <summary>
        /// 当前选择的UI
        /// </summary>
        private static Selectable curSelectUI;
        /// <summary>
        /// 当前选择的UI
        /// </summary>
        public static Selectable CurSelectUI => curSelectUI;
        /// <summary>
        /// 当前选择的UI面板
        /// </summary>
        private static UIPanelBase curUIPanel;

        /// <summary>
        /// 当前选择的UI面板
        /// </summary>
        public static UIPanelBase CurUIPanel => curUIPanel;
        
        /// <summary>
        /// 在进入UI界面的时候调用
        /// </summary>
        public static void OnEnterUIPanelState()
        {
            inTheUIPanel = true;
        }
        /// <summary>
        /// 在关闭所有UI界面的时候调用
        /// </summary>
        public static void OnExitUIPanelState()
        {
            inTheUIPanel = false;
        }

        #region 选择
        
        /// <summary>
        /// 选择当前面板的默认选项
        /// </summary>
        public static void SelectDefaultUI()
        {
            if(curUIPanel == null) return;
            
            curUIPanel.SelectDefaultUI();
        }
        /// <summary>
        /// 选择UI面板
        /// </summary>
        /// <param name="panel">UI面板</param>
        public static void SelectPanel(UIPanelBase panel)
        {
            //如果已经选择这个面板了就返回
            if (curUIPanel == panel) return;

            curUIPanel = panel;

        }
        /// <summary>
        /// 取消UI面板
        /// </summary>
        /// <param name="panel">UI面板</param>
        public static void DeselectPanel(UIPanelBase panel)
        {
            //如果选择的不是这个UI面板了就返回
            if (curUIPanel != panel) return;

            curUIPanel = null;
        }
        /// <summary>
        /// 选择UI
        /// </summary>
        /// <param name="selectable">可选项</param>
        public static void SelectUI(Selectable selectable)
        {
            //如果已经选择了这个UI那就返回
            if(curSelectUI == selectable) return;

            curSelectUI = selectable;
        }
        /// <summary>
        /// 选择UI
        /// </summary>
        /// <param name="selectable">可选项</param>
        public static void DeselectUI(Selectable selectable)
        {
            //如果选择的不是这个UI那就返回
            if(curSelectUI != selectable) return;

            curSelectUI = null;
        }

        #endregion

        #region 操作处理
        
        /// <summary>
        /// 按Esc
        /// </summary>
        public static void PressEsc()
        {
            //如果没有菜单那就打开暂停界面
            if (curUIPanel == null)
            {
                UISystem.Show<PausePanel>();
            }
            else
            {
                curUIPanel.OnPressESC();
            }
        }

        #endregion
        
    }
}
