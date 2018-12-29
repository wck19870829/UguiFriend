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
        [SerializeField] protected RectTransform m_FillRect;
        [SerializeField] protected RectTransform m_HandleRect;
        [SerializeField] protected Direction m_Direction;
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

            Vector3 worldPosA;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(m_SliderBlockA, eventData.pointerCurrentRaycast.screenPosition, eventData.pressEventCamera, out worldPosA);
            Vector3 worldPosB;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(m_SliderBlockB, eventData.pointerCurrentRaycast.screenPosition, eventData.pressEventCamera, out worldPosB);

            var distA = Vector3.Distance(worldPosA, m_SliderBlockA.position);
            var distB = Vector3.Distance(worldPosB, m_SliderBlockB.position);

            currentTarget = distA > distB ? m_SliderBlockA : m_SliderBlockB;
            UpdateBlockPosition(eventData);
        }

        protected void UpdateBlockPosition(PointerEventData eventData)
        {
            if (currentTarget == null) return;
            if (m_HandleRect == null) return;

            Vector3 worldPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(currentTarget, eventData.pointerCurrentRaycast.screenPosition, eventData.pressEventCamera, out worldPos);
            currentTarget.position = worldPos;
        }

        public enum Direction
        {
            Horizontal,
            Vertical
        }
    }
}