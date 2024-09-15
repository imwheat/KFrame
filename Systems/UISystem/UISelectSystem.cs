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

namespace KFrame.Systems
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
        /// <summary>
        /// 是否在UI面板
        /// </summary>
        private static bool inTheUIPanel;
        /// <summary>
        /// 是否在UI面板
        /// </summary>
        public static bool InTheUIPanel => InTheUIPanel;
        /// <summary>
        /// 当前选择的UI
        /// </summary>
        public static Selectable CurSelectUI;
        /// <summary>
        /// 当前选择的UI面板
        /// </summary>
        public static UIPanelBase CurUIPanel;
        
        /// <summary>
        /// 选择当前选项
        /// </summary>
        public static void SelectPanel(UIPanelBase panel)
        {
            //如果已经选择这个面板了就返回
            if (CurUIPanel == panel) return;

            CurUIPanel = panel;

        }
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
    }
}
