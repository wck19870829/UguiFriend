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
    public class UguiDragObject : UguiMultPointInputBase
    {
        protected override void DoChange()
        {
            var cam = pointerList[0].enterEventCamera;
            var screenPos = RectTransformUtility.WorldToScreenPoint(cam, m_Target.position);
            var moveDelta = Vector2.zero;
            foreach (var pointer in pointerList)
            {
                moveDelta+=pointer.delta;
            }
            moveDelta /= pointerList.Count;
            screenPos += moveDelta;
            var ray=RectTransformUtility.ScreenPointToRay(cam, screenPos);
            var plane = new Plane(ray.direction.normalized,m_Target.position);
            float enter;
            plane.Raycast(ray, out enter);
            var worldPos=ray.GetPoint(enter);
            m_Target.transform.position = worldPos;
        }
    }
}