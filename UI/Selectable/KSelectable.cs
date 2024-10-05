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
using UnityEngine.Events;
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
        [SerializeField] private List<Graphic> m_TargetGraphics = new();
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
        /// <summary>
        /// 选择事件
        /// </summary>
        [Serializable]
        public class SelectableSelectEvent : UnityEvent<KSelectable> {}
        /// <summary>
        /// 选择事件
        /// </summary>
        [SerializeField]
        private SelectableSelectEvent onSelect = new SelectableSelectEvent();
        /// <summary>
        /// 选择事件
        /// </summary>
        public SelectableSelectEvent OnSelectEvent
        {
            get => onSelect;
            set => onSelect = value;
        }

        #region 生命周期

        protected override void Awake()
        {
            //读取全局配置的颜色配置
            UIGlobalConfig config = UIGlobalConfig.Instance;
            colors = new ColorBlock()
            {
                colorMultiplier = colors.colorMultiplier,
                disabledColor = config.DisabledColor,
                fadeDuration = colors.fadeDuration,
                highlightedColor = config.SelectedColor,
                normalColor = config.NormalColor,
                pressedColor = config.PressColor,
                selectedColor = config.SelectedColor,
            };
            
            base.Awake();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            UISelectSystem.DeselectUI(this);
        }

        #endregion
        
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
        
        /// <summary>
        /// 当被选择的时候调用
        /// </summary>
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            
            //选择当前UI
            UISelectSystem.SelectUI(this);
            onSelect.Invoke(this);
        }
        /// <summary>
        /// 当被取消选择的时候调用
        /// </summary>
        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            
            //取消选择当前UI
            UISelectSystem.DeselectUI(this);
        }

        #endregion

        #region 导航
        
        /// <summary>
        /// 寻找上方的可选项
        /// </summary>
        public override Selectable FindSelectableOnUp()
        {
            //如果是指定的
            if (navigation.mode == Navigation.Mode.Explicit)
            {
                //如果上方选项为空，那就返回空
                if (navigation.selectOnUp == null)
                {
                    return null;
                }
                //如果上方选项不可以选那就找上方选项的上方选项
                else if (!navigation.selectOnUp.IsActive() || !navigation.selectOnUp.interactable)
                {
                    return navigation.selectOnUp.FindSelectableOnUp();
                }
                //上方选项可选那就选上方选项
                else
                {
                    return navigation.selectOnUp;
                }
            }
            //否则就用原来的
            else
            {
                return base.FindSelectableOnUp();
            }
        }
        /// <summary>
        /// 寻找下方的可选项
        /// </summary>
        public override Selectable FindSelectableOnDown()
        {
            //如果是指定的
            if (navigation.mode == Navigation.Mode.Explicit)
            {
                //如果下方选项为空，那就返回空
                if (navigation.selectOnDown == null)
                {
                    return null;
                }
                //如果下方选项不可以选那就找下方选项的下方选项
                else if (!navigation.selectOnDown.IsActive() || !navigation.selectOnDown.interactable)
                {
                    return navigation.selectOnDown.FindSelectableOnDown();
                }
                //下方选项可选那就选下方选项
                else
                {
                    return navigation.selectOnDown;
                }
            }
            //否则就用原来的
            else
            {
                return base.FindSelectableOnDown();
            }
        }
        /// <summary>
        /// 寻找左方的可选项
        /// </summary>
        public override Selectable FindSelectableOnLeft()
        {
            //如果是指定的
            if (navigation.mode == Navigation.Mode.Explicit)
            {
                //如果左边选项为空，那就返回空
                if (navigation.selectOnLeft == null)
                {
                    return null;
                }
                //如果左边选项不可以选那就找左边选项的左边选项
                else if (!navigation.selectOnLeft.IsActive() || !navigation.selectOnLeft.interactable)
                {
                    return navigation.selectOnLeft.FindSelectableOnLeft();
                }
                //左边选项可选那就选左边选项
                else
                {
                    return navigation.selectOnLeft;
                }
            }
            //否则就用原来的
            else
            {
                return base.FindSelectableOnLeft();
            }
        }
        /// <summary>
        /// 寻找右方的可选项
        /// </summary>
        public override Selectable FindSelectableOnRight()
        {
            //如果是指定的
            if (navigation.mode == Navigation.Mode.Explicit)
            {
                //如果右边选项为空，那就返回空
                if (navigation.selectOnRight == null)
                {
                    return null;
                }
                //如果右边选项不可以选那就找右边选项的右边选项
                else if (!navigation.selectOnRight.IsActive() || !navigation.selectOnRight.interactable)
                {
                    return navigation.selectOnRight.FindSelectableOnRight();
                }
                //右边选项可选那就选右边选项
                else
                {
                    return navigation.selectOnRight;
                }
            }
            //否则就用原来的
            else
            {
                return base.FindSelectableOnRight();
            }
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
            //如果GameObject没有激活那就不进行操作
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
