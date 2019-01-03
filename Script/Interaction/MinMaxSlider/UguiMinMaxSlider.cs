using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 区间滑块
    /// </summary>
    public class UguiMinMaxSlider : Selectable,
        IEventSystemHandler,
        IInitializePotentialDragHandler,
        IDragHandler,
        ICanvasElement
    {
        [SerializeField] protected RectTransform m_SliderBlockA;
        [SerializeField] protected RectTransform m_SliderBlockB;
        [SerializeField] protected RectTransform m_HandleSlideArea;
        [SerializeField] protected float m_MinValue;
        [SerializeField] protected float m_MaxValue;
        [SerializeField] protected float m_MinLimit;
        [SerializeField] protected float m_MaxLimit;
        [SerializeField] protected bool m_WholeNUmbers;
        [SerializeField] protected Direction m_Direction;

        protected RectTransform currentTarget;

        public Action OnValueChange;

        protected UguiMinMaxSlider()
        {
            m_MinLimit = m_MaxValue = 10;
            m_WholeNUmbers = true;
        }

        protected override void Awake()
        {
            base.Awake();

            Set(m_MinValue, m_MaxValue, m_MinLimit, m_MaxLimit);
        }

        public void GraphicUpdateComplete()
        {

        }

        public void LayoutComplete()
        {

        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {

        }

        public void Rebuild(CanvasUpdate executing)
        {

        }

        public void OnDrag(PointerEventData eventData)
        {
            UpdateBlockPosition(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            var screenPointA = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, m_SliderBlockA.position);
            var screenPointB = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, m_SliderBlockB.position);
            var distA = Vector2.Distance(screenPointA, eventData.position);
            var distB = Vector2.Distance(screenPointB, eventData.position);

            currentTarget = distA < distB ? m_SliderBlockA : m_SliderBlockB;
            UpdateBlockPosition(eventData);
        }

        protected void UpdateBlockPosition(PointerEventData eventData)
        {
            if (currentTarget == null) return;

            var localPoint = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleSlideArea, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                var minValue = m_MinValue;
                var maxValue = m_MaxValue;
                minValue=maxValue=localPoint[(int)m_Direction];

                Set(minValue,maxValue);
            }
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public void Set(float minValue, float maxValue)
        {
            Set(minValue, maxValue, m_MinLimit, m_MaxLimit);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="minLimit"></param>
        /// <param name="maxLimit"></param>
        public void Set(float minValue, float maxValue, float minLimit, float maxLimit)
        {
            minLimit = Mathf.Min(minLimit, maxLimit);
            minValue = Mathf.Clamp(minValue, minLimit, maxLimit);
            maxValue = Mathf.Clamp(maxValue, minLimit, maxLimit);
            m_MinValue = minValue;
            m_MaxValue = maxValue;
            m_MinLimit = minLimit;
            m_MaxLimit = maxLimit;

            SetSliderPos(m_SliderBlockA, m_HandleSlideArea,m_MinValue);
            SetSliderPos(m_SliderBlockB, m_HandleSlideArea, m_MaxValue);

            if (OnValueChange != null)
            {
                OnValueChange.Invoke();
            }
        }

        protected void SetSliderPos(RectTransform slider, RectTransform slideArea,float value)
        {
            if (slider == null || slideArea == null) return;

            value = Mathf.Clamp(value, m_MinValue, m_MaxValue);
            var percent = (value - m_MinLimit) / (m_MaxLimit - m_MinLimit);

            var anchorMin = Vector2.zero;
            var anchorMax = Vector2.one;
            anchorMin[(int)m_Direction] = anchorMax[(int)m_Direction] = percent;
            slider.anchorMin = anchorMin;
            slider.anchorMax = anchorMax;
        }

        /// <summary>
        /// 最小值
        /// </summary>
        public float MinValue { get { return m_MinValue; } }

        /// <summary>
        /// 最大值
        /// </summary>
        public float MaxValue { get { return m_MaxValue; } }

        /// <summary>
        /// 下限
        /// </summary>
        public float MinLimit { get { return m_MinLimit; } }

        /// <summary>
        /// 上限
        /// </summary>
        public float MaxLimit { get { return m_MaxLimit; } }

        /// <summary>
        /// 拖拽方向
        /// </summary>
        public enum Direction
        {
            Horizontal=0,
            Vertical=1
        }
    }
}