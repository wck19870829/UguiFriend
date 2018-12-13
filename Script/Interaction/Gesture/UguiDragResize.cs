using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

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

        protected Vector3 offset;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (target != null)
            {
                var targetBounds = UguiMathf.GetBounds(target, Space.World);
                var bounds = UguiMathf.GetBounds((RectTransform)transform,Space.World);
                var closestPoint = targetBounds.ClosestPoint(bounds.center);
                offset = closestPoint - bounds.center;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (target != null)
            {
                var targetBounds = UguiMathf.GetBounds(target, Space.World);
                var bounds = UguiMathf.GetBounds((RectTransform)transform, Space.World);
                var closestPoint = bounds.center + offset;

            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {

        }

        public void SetMinMax(int minWidth,int maxWidth,int minHeight,int maxHeight)
        {

        }
    }
}