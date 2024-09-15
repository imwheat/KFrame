//****************** 代码文件声明 ***********************
//* 文件：KSelectable
//* 作者：wheat
//* 创建时间：2024/09/15 12:42:18 星期日
//* 描述：继承自Unity的Selectable的修改版
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

    public class KSelectable : Selectable
    {
        /// <summary>
        /// 更新UI的状态
        /// </summary>
        /// <param name="state">要切换的UI状态</param>
        public virtual void UpdateUIState(SelectState state)
        {
            switch (state)
            {
                case SelectState.Normal:
                    UpdateNormalUI();
                    break;
                case SelectState.Pressed:
                    UpdatePressUI();
                    break;
                case SelectState.Selected:
                    UpdateSelectedUI();
                    break;
                case SelectState.Disabled:
                    UpdateDisableUI();
                    break;
            }
        }
        /// <summary>
        /// 更新UI到普通状态
        /// </summary>
        public virtual void UpdateNormalUI()
        {
            
        }
        /// <summary>
        /// 更新UI到被按下状态
        /// </summary>
        public virtual void UpdatePressUI()
        {
            
        }
        /// <summary>
        /// 更新UI到被选择状态
        /// </summary>
        public virtual void UpdateSelectedUI()
        {
            
        }
        /// <summary>
        /// 更新UI到被禁用状态
        /// </summary>
        public virtual void UpdateDisableUI()
        {
            
        }
    }
}
