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
        [SerializeField] protected RectTransform m_SliderBlockMin;
        [SerializeField] protected RectTransform m_SliderBlockMax;
        [SerializeField] protected RectTransform m_HandleSlideArea;
        [SerializeField] protected Text m_MinValueText;
        [SerializeField] protected Text m_MaxValueText;
        [SerializeField] protected Text m_MinLimitText;
        [SerializeField] protected Text m_MaxLimitText;
        [SerializeField] protected float m_MinValue;
        [SerializeField] protected float m_MaxValue;
        [SerializeField] protected float m_MinLimit;
        [SerializeField] protected float m_MaxLimit;
        [SerializeField] protected bool m_WholeNumbers;
        [SerializeField] protected Direction m_Direction=Direction.Auto;

        protected RectTransform selectSlider;

        public Action OnValueChange;

        protected UguiMinMaxSlider()
        {
            m_MinLimit = m_MaxValue = 10;
            m_WholeNumbers = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Set(m_MinValue, m_MaxValue);
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

            var minSliderPoint = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, m_SliderBlockMin.position);
            var maxSliderPoint = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, m_SliderBlockMax.position);
            var minSliderDist = Vector2.Distance(eventData.position,minSliderPoint);
            var maxSliderDist = Vector2.Distance(eventData.position,maxSliderPoint);

            selectSlider = minSliderDist < maxSliderDist
                        ? m_SliderBlockMin
                        : m_SliderBlockMax;

            UpdateBlockPosition(eventData);
        }

        protected void UpdateBlockPosition(PointerEventData eventData)
        {
            var screenPointA = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, m_SliderBlockMin.position);
            var screenPointB = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, m_SliderBlockMax.position);
            var distA = Vector2.Distance(screenPointA, eventData.position);
            var distB = Vector2.Distance(screenPointB, eventData.position);

            var localPoint = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleSlideArea, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                if (selectSlider == null) return;
                if (!UguiTools.IsValidNumber(localPoint)) return;

                localPoint -= m_HandleSlideArea.rect.position;
                var percent=localPoint[(int)m_Direction] / m_HandleSlideArea.rect.size[(int)m_Direction];
                var value = percent * (m_MaxLimit-m_MinLimit);
                if (selectSlider == m_SliderBlockMin)
                {
                    m_MinValue = Mathf.Clamp(value,m_MinLimit, m_MaxLimit);
                    m_MinValue = Mathf.Min(m_MaxValue,m_MinValue);
                }
                else
                {
                    m_MaxValue = Mathf.Clamp(value, m_MinLimit, m_MaxLimit);
                    m_MaxValue = Mathf.Max(m_MaxValue, m_MinValue);
                }

                Set(m_MinValue,m_MaxValue);
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
            if (m_HandleSlideArea == null) return;
            if (m_SliderBlockMax == null || m_SliderBlockMin == null) return;

            if (m_Direction == Direction.Auto)
            {
                m_Direction = m_HandleSlideArea.rect.size.x >= m_HandleSlideArea.rect.size.y
                            ? Direction.Horizontal
                            : Direction.Vertical;
            }

            minLimit = Mathf.Min(minLimit, maxLimit);
            maxValue = Mathf.Clamp(maxValue,minLimit,maxLimit);
            minValue = Mathf.Clamp(minValue, minLimit, maxLimit);
            minValue = Mathf.Min(maxValue,minValue);
            m_MinValue = minValue;
            m_MaxValue = maxValue;
            m_MinLimit = minLimit;
            m_MaxLimit = maxLimit;

            if (m_WholeNumbers)
            {
                m_MinValue = Mathf.RoundToInt(m_MinValue);
                m_MaxValue = Mathf.RoundToInt(m_MaxValue);
                m_MinLimit = Mathf.RoundToInt(m_MinLimit);
                m_MaxLimit = Mathf.RoundToInt(m_MaxLimit);
            }

            if (minLimit == maxLimit)
            {
                throw new Exception("Min limit is equal to the max limit!");
            }

            SetSliderPos(m_SliderBlockMin, m_HandleSlideArea,m_MinValue);
            SetSliderPos(m_SliderBlockMax, m_HandleSlideArea, m_MaxValue);
            if (m_MinValueText != null) m_MinValueText.text = m_MinValue.ToString();
            if (m_MaxValueText != null) m_MaxValueText.text = m_MaxValue.ToString();
            if (m_MinLimitText != null) m_MinLimitText.text = m_MinLimit.ToString();
            if (m_MaxLimitText != null) m_MaxLimitText.text = m_MaxLimit.ToString();

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

            var localPos = slider.localPosition;
            localPos[(int)m_Direction] = percent * slideArea.rect.size[(int)m_Direction]+slideArea.rect.position[(int)m_Direction];
            slider.localPosition = localPos;
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
            Vertical=1,
            Auto=3
        }
    }
}