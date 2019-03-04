using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(Graphic))]
    /// <summary>
    /// 拖拽改变尺寸
    /// </summary>
    public class UguiDragResize : UIBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        [SerializeField]protected UguiPivot m_Pivot;
        [SerializeField]protected RectTransform m_Target;
        public int minWidth=100;
        public int minHeight=100;
        public int maxWidth=9999;
        public int maxHeight=9999;

        protected Graphic m_Graphic;
        protected RectTransform m_RectTransform;
        protected Vector2 screenOffset;

        protected override void Awake()
        {
            base.Awake();
            m_RectTransform = transform as RectTransform;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            var screenPoint = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, transform.position);
            screenOffset = screenPoint-eventData.position;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (m_Target != null)
            {
                transform.localPosition = UguiMathf.ResizeRectTransform(m_Target, m_Pivot, transform.localPosition, minWidth, minHeight, maxWidth, maxHeight);
            }
            Vector2 localPoint;
            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Target,eventData.position, eventData.pressEventCamera,out localPoint))
            {
                var rect = m_Target.rect;
                var cachePivot = m_Target.pivot;
                switch (m_Pivot)
                {
                    case UguiPivot.Bottom:
                        rect.yMax = Mathf.Clamp(localPoint.y,rect.yMin+minHeight,rect.yMin+maxHeight);
                        UguiTools.SetPivot(m_Target,m_Pivot);
                        m_Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
                        m_Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,rect.height);
                        m_Target.pivot = cachePivot;
                        break;

                    case UguiPivot.BottomLeft:

                        break;

                    case UguiPivot.BottomRight:

                        break;

                    case UguiPivot.Center:

                        break;

                    case UguiPivot.Left:

                        break;

                    case UguiPivot.Right:

                        break;

                    case UguiPivot.Top:

                        break;

                    case UguiPivot.TopLeft:

                        break;

                    case UguiPivot.TopRight:

                        break;
                }

                transform.localPosition=localPoint;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            
        }

        public UguiPivot Pivot
        {
            get
            {
                return m_Pivot;
            }
            set
            {
                m_Pivot = value;
            }
        }

        public RectTransform Target
        {
            get
            {
                return m_Target;
            }
            set
            {
                m_Target = value;
            }
        }

        public RectTransform RectTransform
        {
            get
            {
                if(m_RectTransform == null)
                {
                    m_RectTransform = transform as RectTransform;
                }
                return m_RectTransform;
            }
        }

        public Graphic Graphic
        {
            get
            {
                if (m_Graphic == null)
                    m_Graphic = GetComponent<Graphic>();

                return m_Graphic;
            }
        }
    }
}