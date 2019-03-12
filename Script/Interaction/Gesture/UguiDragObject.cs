using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace RedScarf.UguiFriend
{
    [DisallowMultipleComponent]
    /// <summary>
    /// 拖拽
    /// </summary>
    public class UguiDragObject : UIBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        public Transform target;            //控制物体
        protected Vector2 screenOffset;

        public Action OnBeginDragEvent;
        public Action OnDragEvent;
        public Action OnEndDragEvent;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
            {
                target = transform;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            var screenPoint=RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, target.position);
            screenOffset = screenPoint-eventData.position;

            if (OnBeginDragEvent != null)
            {
                OnBeginDragEvent.Invoke();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 worldPoint;
            if(RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform,eventData.position+screenOffset,eventData.pressEventCamera,out worldPoint))
            {
                target.position = worldPoint;

                if (OnDragEvent != null)
                {
                    OnDragEvent.Invoke();
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {


            if (OnEndDragEvent != null)
            {
                OnEndDragEvent.Invoke();
            }
        }
    }
}