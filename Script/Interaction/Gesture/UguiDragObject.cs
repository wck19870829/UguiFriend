using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 拖拽
    /// </summary>
    public class UguiDragObject : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform contentRect;
        [Range(0,1)]public float decelerationRate = 0.1f;
        protected Vector2 cacheScreenPos;
        protected RectTransform m_RectTransform;
        protected Canvas m_Canvas;
        protected Vector3 targetPos;
        protected bool m_IsDrag;

        public Action OnBeginDragEvent;
        public Action OnDragEvent;
        public Action OnEndDragEvent;

        protected override void OnEnable()
        {
            base.OnEnable();
            targetPos = transform.position;
        }

        protected virtual void Update()
        {
            Move();
        }

        protected virtual void Move()
        {
            if (m_IsDrag)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.6f);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, decelerationRate);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            m_IsDrag = true;
            m_RectTransform = GetComponent<RectTransform>();
            m_Canvas = GetComponentInParent<Canvas>();
            cacheScreenPos = RectTransformUtility.WorldToScreenPoint(m_Canvas.worldCamera, transform.position);

            if (OnBeginDragEvent != null)
            {
                OnBeginDragEvent.Invoke();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            cacheScreenPos += eventData.delta;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(m_RectTransform, cacheScreenPos, m_Canvas.rootCanvas.worldCamera, out targetPos);

            if (OnDragEvent != null)
            {
                OnDragEvent.Invoke();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_IsDrag = false;
            cacheScreenPos += eventData.delta*10;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(m_RectTransform, cacheScreenPos, m_Canvas.rootCanvas.worldCamera, out targetPos);

            if (OnEndDragEvent != null)
            {
                OnEndDragEvent.Invoke();
            }
        }
    }
}