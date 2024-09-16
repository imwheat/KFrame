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

namespace KFrame.UI
{
    [AddComponentMenu("KUI/Selectable", 35)]
    public class KSelectable : Selectable
    {
        /// <summary>
        /// 随状态切换变化的图像
        /// </summary>
        [SerializeField]
        private List<Graphic> m_TargetGraphics;
        /// <summary>
        /// 随状态切换变化的图像
        /// </summary>
        public List<Graphic> TargetGraphics
        {
            get => m_TargetGraphics;
            set
            {
                if (UISetPropertyUtility.SetClass<List<Graphic>>(ref m_TargetGraphics, value))
                {
                    OnSetProperty();
                }
            }
        }

        /// <summary>
        /// 当前的选择状态
        /// </summary>
        protected SelectState m_SelectState;
        /// <summary>
        /// 当前的选择状态
        /// </summary>
        public SelectState SelectState
        {
            get => m_SelectState;
            set
            {
                if (UISetPropertyUtility.SetStruct<SelectState>(ref m_SelectState, value))
                {
                    OnSetProperty();
                }
            }
        }

        #region 方法重写

        /// <summary>
        /// 当设置参数的时候调用
        /// </summary>
        protected virtual void OnSetProperty()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DoStateTransition(currentSelectionState, true);
            else
#endif
                DoStateTransition(currentSelectionState, false);
        }

        #endregion
        
        #region 状态切换

        /// <summary>
        /// 开始图像颜色变化
        /// </summary>
        /// <param name="targetColor">目标颜色</param>
        /// <param name="instant">立刻变化</param>
        protected void StartColorTween(Color targetColor, bool instant)
        {
            if (targetGraphic == null)
                return;

            float duration = instant ? 0f : colors.fadeDuration;
            targetGraphic.CrossFadeColor(targetColor, duration, true, true);
            for (int i = 0; i < TargetGraphics.Count; i++)
            {
                if (TargetGraphics[i] != null)
                {
                    TargetGraphics[i].CrossFadeColor(targetColor, duration, true, true);
                }
            }
        }
        /// <summary>
        /// 图片切换
        /// </summary>
        /// <param name="newSprite">要切换的图片</param>
        protected void DoSpriteSwap(Sprite newSprite)
        {
            if (image == null)
                return;

            image.overrideSprite = newSprite;
        }
        /// <summary>
        /// 触发动画
        /// </summary>
        /// <param name="triggername"></param>
        protected void TriggerAnimation(string triggername)
        {
#if PACKAGE_ANIMATION
            if (transition != Transition.Animation || animator == null || !animator.isActiveAndEnabled || !animator.hasBoundPlayables || string.IsNullOrEmpty(triggername))
                return;

            animator.ResetTrigger(m_AnimationTriggers.normalTrigger);
            animator.ResetTrigger(m_AnimationTriggers.highlightedTrigger);
            animator.ResetTrigger(m_AnimationTriggers.pressedTrigger);
            animator.ResetTrigger(m_AnimationTriggers.selectedTrigger);
            animator.ResetTrigger(m_AnimationTriggers.disabledTrigger);

            animator.SetTrigger(triggername);
#endif
        }
        /// <summary>
        /// 状态切换
        /// </summary>
        /// <param name="state">要切换的状态</param>
        /// <param name="instant">是否立刻切换</param>
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            //如果Gameobject没有激活那就不进行操作
            if (!gameObject.activeInHierarchy)
                return;

            //获取变化的量
            Color tintColor;
            Sprite transitionSprite;
            string triggerName;

            //根据状态获取值
            switch (state)
            {
                case SelectionState.Normal:
                    tintColor = colors.normalColor;
                    transitionSprite = null;
                    triggerName = animationTriggers.normalTrigger;
                    break;
                case SelectionState.Highlighted:
                    tintColor = colors.highlightedColor;
                    transitionSprite = spriteState.highlightedSprite;
                    triggerName = animationTriggers.highlightedTrigger;
                    break;
                case SelectionState.Pressed:
                    tintColor = colors.pressedColor;
                    transitionSprite = spriteState.pressedSprite;
                    triggerName = animationTriggers.pressedTrigger;
                    break;
                case SelectionState.Selected:
                    tintColor = colors.selectedColor;
                    transitionSprite = spriteState.selectedSprite;
                    triggerName = animationTriggers.selectedTrigger;
                    break;
                case SelectionState.Disabled:
                    tintColor = colors.disabledColor;
                    transitionSprite = spriteState.disabledSprite;
                    triggerName = animationTriggers.disabledTrigger;
                    break;
                default:
                    tintColor = Color.black;
                    transitionSprite = null;
                    triggerName = string.Empty;
                    break;
            }
            
            //根据切换类型，进行不同的切换
            switch (transition)
            {
                case Transition.ColorTint:
                    StartColorTween(tintColor * colors.colorMultiplier, instant);
                    break;
                case Transition.SpriteSwap:
                    DoSpriteSwap(transitionSprite);
                    break;
                case Transition.Animation:
                    TriggerAnimation(triggerName);
                    break;
            }
        }
        
        /// <summary>
        /// 更新UI的状态
        /// </summary>
        /// <param name="state">要切换的UI状态</param>
        /// <param name="instant">立刻切换</param>
        public virtual void UpdateUIState(SelectState state, bool instant)
        {
            switch (state)
            {
                case SelectState.Normal:
                    UpdateNormalUI(instant);
                    break;
                case SelectState.Pressed:
                    UpdatePressUI(instant);
                    break;
                case SelectState.Selected:
                    UpdateSelectedUI(instant);
                    break;
                case SelectState.Disabled:
                    UpdateDisableUI(instant);
                    break;
            }
        }
        /// <summary>
        /// 更新UI到普通状态
        /// </summary>
        public virtual void UpdateNormalUI(bool instant)
        {
            DoStateTransition(SelectionState.Normal, instant);
        }
        /// <summary>
        /// 更新UI到被按下状态
        /// </summary>
        public virtual void UpdatePressUI(bool instant)
        {
            DoStateTransition(SelectionState.Pressed, instant);
        }
        /// <summary>
        /// 更新UI到被选择状态
        /// </summary>
        public virtual void UpdateSelectedUI(bool instant)
        {
            DoStateTransition(SelectionState.Selected, instant);
        }
        /// <summary>
        /// 更新UI到被禁用状态
        /// </summary>
        public virtual void UpdateDisableUI(bool instant)
        {
            DoStateTransition(SelectionState.Disabled, instant);
        }

        #endregion

    
    }
}
