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
    /// 拖拽改变尺寸(单点)
    /// </summary>
    public class UguiDragResize : UIBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        protected static bool s_Dragging;

        [SerializeField] protected Texture2D m_Cursor;
        [SerializeField]protected UguiPivot m_Pivot;
        [SerializeField]protected RectTransform m_Target;
        public int minWidth=100;
        public int minHeight=100;
        public int maxWidth=9999;
        public int maxHeight=9999;

        protected Graphic m_Graphic;
        protected RectTransform m_RectTransform;
        protected Vector2 screenOffset;

        public Action<UguiDragResize> OnResize;
        public Action<UguiDragResize> OnEndResize;

        protected override void Awake()
        {
            base.Awake();

            m_RectTransform = transform as RectTransform;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!m_Target) return;

            var screenPoint = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, transform.position);
            screenOffset = screenPoint-eventData.position;
            s_Dragging = true;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!m_Target) return;

            Vector2 localPoint;
            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Target,eventData.position+ screenOffset, eventData.pressEventCamera,out localPoint))
            {
                s_Dragging = true;
                var rect = m_Target.rect;
                var cachePivot = m_Target.pivot;
                var edgeDist = UguiMathf.GetRectTransformEdgeDistance(m_Target, transform.position);
                switch (m_Pivot)
                {
                    case UguiPivot.Bottom:
                        rect.yMin = Mathf.Clamp(localPoint.y+ edgeDist.y, rect.yMax - maxHeight, rect.yMax-minHeight);
                        UguiMathf.SetPivot(m_Target,UguiPivot.Top);
                        break;

                    case UguiPivot.BottomLeft:
                        rect.yMin = Mathf.Clamp(localPoint.y + edgeDist.y, rect.yMax - maxHeight, rect.yMax - minHeight);
                        rect.xMin = Mathf.Clamp(localPoint.x + edgeDist.z, rect.xMax - maxWidth, rect.xMax - minWidth);
                        UguiMathf.SetPivot(m_Target, UguiPivot.TopRight);
                        break;

                    case UguiPivot.BottomRight:
                        rect.yMin = Mathf.Clamp(localPoint.y + edgeDist.y, rect.yMax - maxHeight, rect.yMax - minHeight);
                        rect.xMax = Mathf.Clamp(localPoint.x + edgeDist.w, rect.xMin + minWidth, rect.xMin + maxWidth);
                        UguiMathf.SetPivot(m_Target, UguiPivot.TopLeft);
                        break;

                    case UguiPivot.Center:

                        break;

                    case UguiPivot.Left:
                        rect.xMin = Mathf.Clamp(localPoint.x+edgeDist.z, rect.xMax - maxWidth, rect.xMax - minWidth);
                        UguiMathf.SetPivot(m_Target, UguiPivot.Right);
                        break;

                    case UguiPivot.Right:
                        rect.xMax = Mathf.Clamp(localPoint.x+edgeDist.w, rect.xMin + minWidth, rect.xMin + maxWidth);
                        UguiMathf.SetPivot(m_Target, UguiPivot.Left);
                        break;

                    case UguiPivot.Top:
                        rect.yMax = Mathf.Clamp(localPoint.y+edgeDist.x, rect.yMin + minHeight, rect.yMin + maxHeight);
                        UguiMathf.SetPivot(m_Target, UguiPivot.Bottom);
                        break;

                    case UguiPivot.TopLeft:
                        rect.yMax = Mathf.Clamp(localPoint.y + edgeDist.x, rect.yMin + minHeight, rect.yMin + maxHeight);
                        rect.xMin = Mathf.Clamp(localPoint.x + edgeDist.z, rect.xMax - maxWidth, rect.xMax - minWidth);
                        UguiMathf.SetPivot(m_Target, UguiPivot.BottomRight);
                        break;

                    case UguiPivot.TopRight:
                        rect.yMax = Mathf.Clamp(localPoint.y + edgeDist.x, rect.yMin + minHeight, rect.yMin + maxHeight);
                        rect.xMax = Mathf.Clamp(localPoint.x + edgeDist.w, rect.xMin + minWidth, rect.xMin + maxWidth);
                        UguiMathf.SetPivot(m_Target, UguiPivot.BottomLeft);
                        break;
                }

                m_Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
                m_Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
                UguiMathf.SetPivot(m_Target, cachePivot);

                if (OnResize != null)
                {
                    OnResize.Invoke(this);
                }
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            s_Dragging = false;
            if (eventData.pointerEnter!=gameObject)
            {
                Cursor.SetCursor(null, new Vector2(m_Cursor.width * 0.5f, m_Cursor.height * 0.5f), CursorMode.Auto);
            }

            if (OnEndResize!=null)
            {
                OnEndResize.Invoke(this);
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!s_Dragging&&m_Cursor)
            {
                Cursor.SetCursor(m_Cursor, new Vector2(m_Cursor.width*0.5f, m_Cursor.height*0.5f), CursorMode.Auto);
            }
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!s_Dragging && m_Cursor)
            {
                Cursor.SetCursor(null, new Vector2(m_Cursor.width * 0.5f, m_Cursor.height * 0.5f), CursorMode.Auto);
            }
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

        public Texture2D CursorIcon
        {
            get
            {
                return m_Cursor;
            }
            set
            {
                m_Cursor = value;
            }
        }
    }
}