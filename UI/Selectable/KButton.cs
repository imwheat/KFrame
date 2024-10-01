//****************** 代码文件声明 ***********************
//* 文件：KButton
//* 作者：wheat
//* 创建时间：2024/09/15 18:18:39 星期日
//* 描述：按钮
//*******************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using KFrame;
using KFrame.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace KFrame.UI
{
    [AddComponentMenu("KUI/Button", 50)]
    public class KButton : KSelectable, IPointerClickHandler, ISubmitHandler
    {
        /// <summary>
        /// 按钮点击事件
        /// </summary>
        [Serializable]
        public class ButtonClickedEvent : UnityEvent {}
        
        /// <summary>
        /// 按钮点击事件
        /// </summary>
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();
        public ButtonClickedEvent OnClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }
        
        /// <summary>
        /// 按下按钮
        /// </summary>
        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_OnClick.Invoke();
        }
        /// <summary>
        /// 被点击
        /// </summary>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            //如果不是左键的话那就无效
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }
        /// <summary>
        /// 接受提交
        /// </summary>
        public virtual void OnSubmit(BaseEventData eventData)
        {
            //触发按钮按下
            Press();

            //如果按钮被禁用了那就不用启动协程
            if (!IsActive() || !IsInteractable())
                return;
            
            //进行状态切换
            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }
        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
    }
}
