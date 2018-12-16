using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 拖拽
    /// </summary>
    public class UguiDragObject : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IDropHandler
    {
        public RectTransform contentRect;
        [Range(0,1)]public float decelerationRate = 0.1f;
        protected Vector2 screenPosOffset;
        protected bool m_IsDrag;
        protected bool m_IsRunning;

        public Action<PointerEventData> OnBeginDragEvent;
        public Action<PointerEventData> OnDragEvent;
        public Action<PointerEventData> OnEndDragEvent;
        public Action<PointerEventData> OnDrapEvent;

        protected virtual void Update()
        {
            Move();
        }

        protected virtual void Move()
        {
            //if (m_IsRunning)
            //{
            //    if (m_IsDrag)
            //    {
            //        transform.position = Vector3.Lerp(transform.position, targetPos, 0.6f);
            //    }
            //    else
            //    {
            //        transform.position = Vector3.Lerp(transform.position, targetPos, decelerationRate);
            //    }

            //    if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            //    {
            //        transform.position = targetPos;
            //        m_IsRunning = false;
            //    }
            //}

            //if (contentRect != null)
            //{
            //    var contentBounds = UguiMathf.GetBounds(contentRect, Space.World);
            //    var bounds = UguiMathf.GetGlobalBoundsIncludeChildren((RectTransform)transform,false);
            //    var newBounds = UguiMathf.LimitBounds(bounds, contentBounds);
            //    transform.position = newBounds.center;
            //}
        }

        /// <summary>
        /// 停止运动
        /// </summary>
        public void Stop()
        {
            m_IsRunning = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            m_IsDrag = true;


            if (OnBeginDragEvent != null)
            {
                OnBeginDragEvent.Invoke(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_IsDrag = true;
            m_IsRunning = true;
            //RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, cacheScreenPos, eventData.pressEventCamera, out targetPos);

            Debug.Log(eventData);



            if (OnDragEvent != null)
            {
                OnDragEvent.Invoke(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_IsDrag = false;
            //RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, cacheScreenPos, eventData.pressEventCamera, out targetPos);

            if (OnEndDragEvent != null)
            {
                OnEndDragEvent.Invoke(eventData);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (OnDrapEvent!=null)
            {
                OnDragEvent.Invoke(eventData);
            }
        }
    }
}