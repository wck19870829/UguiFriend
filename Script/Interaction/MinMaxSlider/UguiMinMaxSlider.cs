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
        [SerializeField] protected RectTransform m_LimitLineStart;
        [SerializeField] protected RectTransform m_LimitLineEnd;
        [SerializeField] protected RectTransform m_FillRect;
        [SerializeField] protected RectTransform m_HandleRect;
        [SerializeField] protected float m_MinValue;
        [SerializeField] protected float m_MaxValue;
        [SerializeField] protected bool m_WholeNUmbers;

        protected RectTransform currentTarget;

        public Action<float, float> OnValueChange;

        protected UguiMinMaxSlider()
        {
            m_MaxValue = 10;
            m_WholeNUmbers = true;
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
            if (m_LimitLineStart == null || m_LimitLineEnd == null) return;

            var limitPointStart = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, m_LimitLineStart.position);
            var limitPointEnd = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, m_LimitLineEnd.position);
            var screenLimitLine = new UguiMathf.Line2(limitPointStart, limitPointEnd);
            var screenPoint = screenLimitLine.ProjectPoint(eventData.position);

            Vector3 worldPos;
            if(RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, screenPoint, eventData.pressEventCamera, out worldPos))
            {
                currentTarget.position = worldPos;

                if (OnValueChange != null)
                {
                    //OnValueChange.Invoke();
                }
            }
        }
    }
}