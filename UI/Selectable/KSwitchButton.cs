//****************** 代码文件声明 ***********************
//* 文件：KSwitchButton
//* 作者：wheat
//* 创建时间：2024/10/02 09:05:29 星期三
//* 描述：可以左右切换的按钮
//*******************************************************

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KFrame.UI
{
    [AddComponentMenu("KUI/SwitchButton", 51)]
    public class KSwitchButton : KSelectable, IPointerClickHandler
    {

        /// <summary>
        /// 按钮点击事件
        /// </summary>
        [Serializable]
        public class SwitchButtonClickedEvent : UnityEvent<int> {}
        
        #region UI配置参数
        
        /// <summary>
        /// 按钮点击事件
        /// </summary>
        [SerializeField]
        private SwitchButtonClickedEvent onClick = new SwitchButtonClickedEvent();
        public SwitchButtonClickedEvent OnClick
        {
            get => onClick;
            set => onClick = value;
        }
        /// <summary>
        /// 减选项按钮
        /// </summary>
        [SerializeField]
        private KButton minusButton;
        /// <summary>
        /// 增选项按钮
        /// </summary>
        [SerializeField]
        private KButton plusButton;
        /// <summary>
        /// 循环
        /// 如果开启左右切换的时候可以来回切换
        /// </summary>
        [SerializeField]
        private bool loop;
        /// <summary>
        /// 循环
        /// 如果开启左右切换的时候可以来回切换
        /// </summary>
        public bool Loop
        {
            get => loop;
            set
            {
                if (UISetPropertyUtility.SetStruct(ref loop, value))
                {
                    ButtonInteractCheck();
                }
            }
        }
        /// <summary>
        /// 值的范围
        /// </summary>
        [SerializeField]
        private Vector2Int range;
        /// <summary>
        /// 值的范围
        /// </summary>
        public Vector2Int Range
        {
            get => range;
            set
            {
                if (UISetPropertyUtility.SetStruct(ref range, value))
                {
                    ButtonInteractCheck();
                }
            }
        }
        /// <summary>
        /// 值
        /// </summary>
        private int value;
        public int Value
        {
            get => value;
            set
            {
                if (UISetPropertyUtility.SetStruct(ref this.value, Math.Clamp(value, range.x, range.y)))
                {
                    OnValueUpdate();
                }
            }
        }
        /// <summary>
        /// 设置滚动条的滚动方向
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// 左减右增
            /// </summary>
            LeftRight,

            /// <summary>
            /// 右减左增
            /// </summary>
            RightLeft,

            /// <summary>
            /// 下减上增
            /// </summary>
            BottomTop,

            /// <summary>
            /// 上减下增
            /// </summary>
            TopBottom,
        }
        enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }
        /// <summary>
        /// 切换方向
        /// </summary>
        [SerializeField]
        private Direction m_Direction = Direction.LeftRight;
        /// <summary>
        /// 增减轴
        /// </summary>
        Axis axis => (m_Direction <= Direction.RightLeft) ? Axis.Horizontal : Axis.Vertical;
        /// <summary>
        /// 是否倒转
        /// </summary>
        bool reverse => m_Direction is Direction.RightLeft or Direction.TopBottom;

        #endregion

        #region 生命周期

        protected override void Awake()
        {
            base.Awake();
            
            minusButton?.OnClick.AddListener(MinusValue);
            plusButton?.OnClick.AddListener(PlusValue);
        }

        #endregion
        
        #region 操作

        /// <summary>
        /// 更新Value
        /// </summary>
        /// <param name="newValue">新的值</param>
        /// <param name="notify">是否要调用事件</param>
        public void SetValue(int newValue,bool notify)
        {
            //限制值的范围
            if (loop)
            {
                if (newValue < range.x)
                {
                    newValue = range.y - 1;
                }
                else if (newValue >= range.y)
                {
                    newValue = range.x;
                }
                    
            }
            newValue = Math.Clamp(newValue, range.x, range.y);

            //如果新的值和旧的一样那就返回
            if(notify) onClick.Invoke(newValue);
            if (newValue == value)
            {
                return;
            }
            
            //更新值
            value = newValue;
            ButtonInteractCheck();
        }
        /// <summary>
        /// 调整检测按钮的可交互性
        /// </summary>
        private void ButtonInteractCheck()
        {
            //如果是循环的那就不用管
            if(loop) return;
            //如果不是循环的，再点击的时候值要越界了，那就关闭按钮交互
            if (plusButton != null) plusButton.interactable = value + 1 < range.y;
            if (minusButton != null) minusButton.interactable = value > range.x;
        }
        /// <summary>
        /// 按下按钮
        /// </summary>
        private void OnValueUpdate()
        {
            //调用事件
            onClick.Invoke(value);

            ButtonInteractCheck();
        }
        /// <summary>
        /// 被点击
        /// </summary>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            //如果不是左键的话那就无效
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            //如果不能交互那就返回
            if (!IsActive() || !IsInteractable())
                return;
            UISystemProfilerApi.AddMarker("SwitchButton.onClick", this);
            PlusValue();
        }
        /// <summary>
        /// 减少值
        /// </summary>
        private void MinusValue()
        {
            SetValue(value - 1 , true);
        }
        /// <summary>
        /// 增加值
        /// </summary>
        private void PlusValue()
        {
            SetValue(value + 1 , true);
        }
        
        #endregion

        #region UI相关重写

        /// <summary>
        /// 寻找左边的可选项
        /// </summary>
        public override Selectable FindSelectableOnLeft()
        {
            if (axis == Axis.Horizontal)
            {
                if(reverse) PlusValue();
                else MinusValue();
                
                return null;
            }
            return base.FindSelectableOnLeft();
        }

        /// <summary>
        /// 寻找右边的可选项
        /// </summary>
        public override Selectable FindSelectableOnRight()
        {
            if (axis == Axis.Horizontal)
            {
                if(reverse) MinusValue();
                else PlusValue();
                
                return null;
            }
            return base.FindSelectableOnRight();
        }

        /// <summary>
        /// 寻找上边的可选项
        /// </summary>
        public override Selectable FindSelectableOnUp()
        {
            if (axis == Axis.Vertical)
            {
                if(reverse) PlusValue();
                else MinusValue();
                
                return null;
            }
            return base.FindSelectableOnUp();
        }

        /// <summary>
        /// 寻找下边的可选项
        /// </summary>
        public override Selectable FindSelectableOnDown()
        {
            if (axis == Axis.Vertical)
            {
                if(reverse) MinusValue();
                else PlusValue();
                
                return null;
            }
            return base.FindSelectableOnDown();
        }

        #endregion

    }
}
