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

        protected Vector3 offset;
        protected int closestPointIndex;               //顶点序号(顺时针)
        protected List<Vector3> pointList;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (target != null)
            {
                var targetBounds = UguiMathf.GetBounds(target, Space.World);
                var bounds = UguiMathf.GetBounds((RectTransform)transform,Space.World);
                var distList = new List<float>
                                {
                                    Vector3.Distance(pointList[0],bounds.center),
                                    Vector3.Distance(pointList[1], bounds.center),
                                    Vector3.Distance(pointList[2], bounds.center),
                                    Vector3.Distance(pointList[3], bounds.center)
                                };
                var minDist = Mathf.Min
                            (
                                distList[0],
                                distList[1],
                                distList[2],
                                distList[3]
                            );
                closestPointIndex = distList.IndexOf(minDist);
                offset = pointList[closestPointIndex] - bounds.center;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (target != null)
            {
                switch (closestPointIndex)
                {
                    case 0:
                        //pointList[closestPointIndex]
                        break;

                    case 1:

                        break;

                    case 2:

                        break;

                    case 3:

                        break;
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {

        }
    }
}