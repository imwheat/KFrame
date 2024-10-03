//****************** 代码文件声明 ***********************
//* 文件：KScrollbar
//* 作者：wheat
//* 创建时间：2024/09/15 19:08:57 星期日
//* 描述：滚动条
//*******************************************************
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KFrame.UI
{
    [AddComponentMenu("KUI/Scrollbar", 36)]
    public class KScrollbar : KSelectable, IBeginDragHandler, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {

        #region 参数

         /// <summary>
        /// 设置滚动条的滚动方向
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// 从左到右
            /// </summary>
            LeftToRight,

            /// <summary>
            /// 从右到左
            /// </summary>
            RightToLeft,

            /// <summary>
            /// 从下到上
            /// </summary>
            BottomToTop,

            /// <summary>
            /// 从上到下
            /// </summary>
            TopToBottom,
        }

        /// <summary>
        /// 当滚轮滚动的时候的回调事件
        /// </summary>
        [Serializable]
        public class ScrollEvent : UnityEvent<float> {}
        
        /// <summary>
        /// 滚动条的把手
        /// </summary>
        [SerializeField]
        private RectTransform m_HandleRect;

        /// <summary>
        /// 滚动条的把手
        /// </summary>
        public RectTransform HandleRect { get { return m_HandleRect; } set { if (UISetPropertyUtility.SetClass(ref m_HandleRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        /// <summary>
        /// 滚动方向
        /// </summary>
        [SerializeField]
        private Direction m_Direction = Direction.LeftToRight;

        /// <summary>
        /// 滚动方向
        /// </summary>
        public Direction direction { get { return m_Direction; } set { if (UISetPropertyUtility.SetStruct(ref m_Direction, value)) UpdateVisuals(); } }

        protected KScrollbar()
        {}

        /// <summary>
        /// 滚动条值
        /// </summary>
        [Range(0f, 1f)]
        [SerializeField]
        private float m_Value;

        /// <summary>
        /// 当前滚动条的值，范围是[0,1f]
        /// </summary>
        public float Value
        {
            get
            {
                float val = m_Value;
                if (m_NumberOfSteps > 1)
                    val = Mathf.Round(val * (m_NumberOfSteps - 1)) / (m_NumberOfSteps - 1);
                return val;
            }
            set
            {
                Set(value);
            }
        }

        /// <summary>
        /// 把手尺寸
        /// </summary>
        [Range(0f, 1f)]
        [SerializeField]
        private float m_Size = 0.2f;

        /// <summary>
        /// 把手尺寸
        /// </summary>
        public float size { get { return m_Size; } set { if (UISetPropertyUtility.SetStruct(ref m_Size, Mathf.Clamp01(value))) UpdateVisuals(); } }

        [Range(0, 20)]
        [SerializeField]
        private int m_NumberOfSteps = 0;

        /// <summary>
        /// 从0刀1所需要的步数
        /// </summary>
        public int numberOfSteps { get { return m_NumberOfSteps; } set { if (UISetPropertyUtility.SetStruct(ref m_NumberOfSteps, value)) { Set(m_Value); UpdateVisuals(); } } }

        /// <summary>
        /// 滚轮值变化的时候调用事件
        /// </summary>
        [Space(6)]
        [SerializeField]
        private ScrollEvent m_OnValueChanged = new ScrollEvent();

        /// <summary>
        /// 滚轮值变化的时候调用事件
        /// </summary>
        public ScrollEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }

        /// <summary>
        /// 滚动条的容器的Rect
        /// </summary>
        private RectTransform m_ContainerRect;

        /// <summary>
        /// 把手的位移偏量
        /// </summary>
        private Vector2 m_Offset = Vector2.zero;

        /// <summary>
        /// 每一次移动的步值
        /// </summary>
        float stepSize { get { return (m_NumberOfSteps > 1) ? 1f / (m_NumberOfSteps - 1) : 0.05f; } }

        // field is never assigned warning
#pragma warning disable 649
        private DrivenRectTransformTracker m_Tracker;
#pragma warning restore 649
        private Coroutine m_PointerDownRepeat;
        private bool isPointerDownAndNotDragging = false;

        // This "delayed" mechanism is required for case 1037681.
        private bool m_DelayedUpdateVisuals = false;

        #endregion
        

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            m_Size = Mathf.Clamp01(m_Size);

            //如果没有激活Gameobject就不用调用
            if (IsActive())
            {
                UpdateCachedReferences();
                Set(m_Value, false);
                //延迟更新，有些东西需要推迟到下一次update更新，某些东西可能影响到他们更新
                m_DelayedUpdateVisuals = true;
            }

            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(Value);
#endif
        }

        /// <summary>
        /// 详见 ICanvasElement.LayoutComplete.
        /// </summary>
        /// <see cref="ICanvasElement.LayoutComplete"/>
        public virtual void LayoutComplete()
        {}

        /// <summary>
        /// 详见 ICanvasElement.GraphicUpdateComplete.
        /// </summary>
        /// <see cref="ICanvasElement.GraphicUpdateComplete"/>
        public virtual void GraphicUpdateComplete()
        {}

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCachedReferences();
            Set(m_Value, false);
            // 更新视效
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }

        /// <summary>
        /// Update用于执行某些延迟更新
        /// </summary>
        protected virtual void Update()
        {
            if (m_DelayedUpdateVisuals)
            {
                m_DelayedUpdateVisuals = false;
                UpdateVisuals();
            }
        }
        
        /// <summary>
        /// 用来更新缓存的引用对象
        /// </summary>
        void UpdateCachedReferences()
        {
            if (m_HandleRect && m_HandleRect.parent != null)
                m_ContainerRect = m_HandleRect.parent.GetComponent<RectTransform>();
            else
                m_ContainerRect = null;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="input">新的值</param>
        /// <param name="sendCallback">是否调用回调</param>
        private void Set(float input, bool sendCallback = true)
        {
            float currentValue = m_Value;
            
            m_Value = Mathf.Clamp01(input);

            //如果新的值和之前的一样就不用更新
            if (Mathf.Approximately(currentValue, m_Value))
                return;

            UpdateVisuals();
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Scrollbar.value", this);
                m_OnValueChanged.Invoke(Value);
            }
        }
        /// <summary>
        /// 设置滚动条的值不调用回调函数
        /// </summary>
        /// <param name="input">要设置的滚动条的值</param>
        public virtual void SetValueWithoutNotify(float input)
        {
            Set(input, false);
        }
        /// <summary>
        /// 当Rect发生变化
        /// </summary>
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            //如果GameObject没有激活那就不用调用
            if (!IsActive())
                return;
            //更新视效
            UpdateVisuals();
        }

        #region 视觉

        enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }
        
        Axis axis { get { return (m_Direction == Direction.LeftToRight || m_Direction == Direction.RightToLeft) ? Axis.Horizontal : Axis.Vertical; } }
        bool reverseValue { get { return m_Direction == Direction.RightToLeft || m_Direction == Direction.TopToBottom; } }

        /// <summary>
        /// 更新UI视效
        /// </summary>
        private void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif
            m_Tracker.Clear();

            if (m_ContainerRect != null)
            {
                m_Tracker.Add(this, m_HandleRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;

                float movement = Mathf.Clamp01(Value) * (1 - size);
                if (reverseValue)
                {
                    anchorMin[(int)axis] = 1 - movement - size;
                    anchorMax[(int)axis] = 1 - movement;
                }
                else
                {
                    anchorMin[(int)axis] = movement;
                    anchorMax[(int)axis] = movement + size;
                }

                m_HandleRect.anchorMin = anchorMin;
                m_HandleRect.anchorMax = anchorMax;
            }
        }


        #endregion
        
        #region 拖拽操作

         /// <summary>
        /// 更新鼠标拖拽
        /// </summary>
        /// <param name="eventData"></param>
        void UpdateDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (m_ContainerRect == null)
                return;

            Vector2 position = Vector2.zero;
            if (!UIMultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
                return;

            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(m_ContainerRect, position, eventData.pressEventCamera, out localCursor))
                return;

            Vector2 handleCenterRelativeToContainerCorner = localCursor - m_Offset - m_ContainerRect.rect.position;
            Vector2 handleCorner = handleCenterRelativeToContainerCorner - (m_HandleRect.rect.size - m_HandleRect.sizeDelta) * 0.5f;

            float parentSize = axis == 0 ? m_ContainerRect.rect.width : m_ContainerRect.rect.height;
            float remainingSize = parentSize * (1 - size);
            if (remainingSize <= 0)
                return;

            DoUpdateDrag(handleCorner, remainingSize);
        }

        /// <summary>
        /// 更新鼠标拖拽
        /// </summary>
        private void DoUpdateDrag(Vector2 handleCorner, float remainingSize)
        {
            switch (m_Direction)
            {
                case Direction.LeftToRight:
                    Set(Mathf.Clamp01(handleCorner.x / remainingSize));
                    break;
                case Direction.RightToLeft:
                    Set(Mathf.Clamp01(1f - (handleCorner.x / remainingSize)));
                    break;
                case Direction.BottomToTop:
                    Set(Mathf.Clamp01(handleCorner.y / remainingSize));
                    break;
                case Direction.TopToBottom:
                    Set(Mathf.Clamp01(1f - (handleCorner.y / remainingSize)));
                    break;
            }
        }
        /// <summary>
        /// 判断能不能拖拽
        /// </summary>
        /// <returns>不能拖拽返回false</returns>
        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        /// <summary>
        /// 在开始拖拽前处理滚轮的初始值
        /// </summary>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            isPointerDownAndNotDragging = false;

            if (!MayDrag(eventData))
                return;

            if (m_ContainerRect == null)
                return;

            m_Offset = Vector2.zero;
            if (RectTransformUtility.RectangleContainsScreenPoint(m_HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
            {
                Vector2 localMousePos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out localMousePos))
                    m_Offset = localMousePos - m_HandleRect.rect.center;
            }
        }

        /// <summary>
        /// 处理滚轮的值
        /// </summary>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            if (m_ContainerRect != null)
                UpdateDrag(eventData);
        }

        /// <summary>
        /// 当滚轮条被按下的时候触发
        /// </summary>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);
            isPointerDownAndNotDragging = true;
            m_PointerDownRepeat = StartCoroutine(ClickRepeat(eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera));
        }

        protected IEnumerator ClickRepeat(PointerEventData eventData)
        {
            return ClickRepeat(eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera);
        }

        /// <summary>
        /// Coroutine function for handling continual press during Scrollbar.OnPointerDown.
        /// </summary>
        protected IEnumerator ClickRepeat(Vector2 screenPosition, Camera camera)
        {
            while (isPointerDownAndNotDragging)
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(m_HandleRect, screenPosition, camera))
                {
                    Vector2 localMousePos;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleRect, screenPosition, camera, out localMousePos))
                    {
                        var axisCoordinate = axis == 0 ? localMousePos.x : localMousePos.y;

                        // 根据方向处理值

                        float change = axisCoordinate < 0 ? size : -size;
                        Value += reverseValue ? change : -change;
                        Value = Mathf.Clamp01(Value);
                        // 只保证4位小数精确
                        Value = Mathf.Round(Value * 10000f) / 10000f;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            StopCoroutine(m_PointerDownRepeat);
        }

        /// <summary>
        /// 当鼠标放开的时候触发
        /// </summary>
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            isPointerDownAndNotDragging = false;
        }

        /// <summary>
        /// 处理移动
        /// </summary>
        public override void OnMove(AxisEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
            {
                base.OnMove(eventData);
                return;
            }

            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    if (axis == Axis.Horizontal && FindSelectableOnLeft() == null)
                        Set(Mathf.Clamp01(reverseValue ? Value + stepSize : Value - stepSize));
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Right:
                    if (axis == Axis.Horizontal && FindSelectableOnRight() == null)
                        Set(Mathf.Clamp01(reverseValue ? Value - stepSize : Value + stepSize));
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Up:
                    if (axis == Axis.Vertical && FindSelectableOnUp() == null)
                        Set(Mathf.Clamp01(reverseValue ? Value - stepSize : Value + stepSize));
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Down:
                    if (axis == Axis.Vertical && FindSelectableOnDown() == null)
                        Set(Mathf.Clamp01(reverseValue ? Value + stepSize : Value - stepSize));
                    else
                        base.OnMove(eventData);
                    break;
            }
        }


        #endregion
       
        #region 导航重写

        /// <summary>
        /// 寻找左边的可选项
        /// </summary>
        public override Selectable FindSelectableOnLeft()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnLeft();
        }

        /// <summary>
        /// 寻找右边的可选项
        /// </summary>
        public override Selectable FindSelectableOnRight()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnRight();
        }

        /// <summary>
        /// 寻找上边的可选项
        /// </summary>
        public override Selectable FindSelectableOnUp()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
                return null;
            return base.FindSelectableOnUp();
        }

        /// <summary>
        /// 寻找下边的可选项
        /// </summary>
        public override Selectable FindSelectableOnDown()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
                return null;
            return base.FindSelectableOnDown();
        }
        #endregion
        
        /// <summary>
        /// 详见: IInitializePotentialDragHandler.OnInitializePotentialDrag
        /// </summary>
        /// <see cref="IInitializePotentialDragHandler.OnInitializePotentialDrag"/>
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        /// <summary>
        /// 更新UI的方向
        /// </summary>
        /// <param name="direction">方向</param>
        /// <param name="includeRectLayouts">是否要更新Rect布局</param>
        public void SetDirection(Direction direction, bool includeRectLayouts)
        {
            Axis oldAxis = axis;
            bool oldReverse = reverseValue;
            this.direction = direction;

            if (!includeRectLayouts)
                return;

            if (axis != oldAxis)
                RectTransformUtility.FlipLayoutAxes(transform as RectTransform, true, true);

            if (reverseValue != oldReverse)
                RectTransformUtility.FlipLayoutOnAxis(transform as RectTransform, (int)axis, true, true);
        }
    }
}
