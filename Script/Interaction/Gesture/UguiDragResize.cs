using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 拖拽改变尺寸
    /// </summary>
    public class UguiDragResize : UIBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        public RectTransform target;
        public int minWidth=100;
        public int minHeight=100;
        public int maxWidth=9999;
        public int maxHeight=9999;

        protected Vector2 screenOffset;

        private void OnDrawGizmos()
        {
            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(transform as RectTransform);
            Gizmos.DrawWireCube(bounds.center,bounds.size);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            var screenPoint = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, transform.position);
            screenOffset = screenPoint-eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (target != null)
            {

            }
            Vector3 worldPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position+screenOffset, eventData.pressEventCamera, out worldPos);
            transform.position = worldPos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {

        }
    }
}