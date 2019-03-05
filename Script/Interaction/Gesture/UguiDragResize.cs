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
        public Vector2 pivot;
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
            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Target,eventData.position+ screenOffset, eventData.pressEventCamera,out localPoint))
            {
                var rect = m_Target.rect;
                var cachePivot = m_Target.pivot;
                switch (m_Pivot)
                {
                    case UguiPivot.Bottom:
                        rect.yMin = Mathf.Clamp(localPoint.y, rect.yMax - maxHeight, rect.yMax-minHeight);
                        UguiTools.SetPivot(m_Target,UguiPivot.Top);
                        break;

                    case UguiPivot.BottomLeft:
                        rect.yMin = Mathf.Clamp(localPoint.y, rect.yMax - maxHeight, rect.yMax - minHeight);
                        rect.xMin = Mathf.Clamp(localPoint.x, rect.xMax - maxWidth, rect.xMax - minWidth);
                        UguiTools.SetPivot(m_Target, UguiPivot.TopRight);
                        break;

                    case UguiPivot.BottomRight:
                        rect.yMin = Mathf.Clamp(localPoint.y, rect.yMax - maxHeight, rect.yMax - minHeight);
                        rect.xMax = Mathf.Clamp(localPoint.x, rect.xMin + minWidth, rect.xMin + maxWidth);
                        UguiTools.SetPivot(m_Target, UguiPivot.TopLeft);
                        break;

                    case UguiPivot.Center:

                        break;

                    case UguiPivot.Left:
                        rect.xMin = Mathf.Clamp(localPoint.x, rect.xMax - maxWidth, rect.xMax - minWidth);
                        UguiTools.SetPivot(m_Target, UguiPivot.Right);
                        break;

                    case UguiPivot.Right:
                        rect.xMax = Mathf.Clamp(localPoint.x, rect.xMin + minWidth, rect.xMin + maxWidth);
                        UguiTools.SetPivot(m_Target, UguiPivot.Left);
                        break;

                    case UguiPivot.Top:
                        rect.yMax = Mathf.Clamp(localPoint.y, rect.yMin + minHeight, rect.yMin + maxHeight);
                        UguiTools.SetPivot(m_Target, UguiPivot.Bottom);
                        break;

                    case UguiPivot.TopLeft:
                        rect.yMax = Mathf.Clamp(localPoint.y, rect.yMin + minHeight, rect.yMin + maxHeight);
                        rect.xMin = Mathf.Clamp(localPoint.x, rect.xMax - maxWidth, rect.xMax - minWidth);
                        UguiTools.SetPivot(m_Target, UguiPivot.BottomRight);
                        break;

                    case UguiPivot.TopRight:
                        rect.yMax = Mathf.Clamp(localPoint.y, rect.yMin + minHeight, rect.yMin + maxHeight);
                        rect.xMax = Mathf.Clamp(localPoint.x, rect.xMin + minWidth, rect.xMin + maxWidth);
                        UguiTools.SetPivot(m_Target, UguiPivot.BottomLeft);
                        break;
                }

                m_Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
                m_Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
                UguiTools.SetPivot(m_Target, cachePivot);

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