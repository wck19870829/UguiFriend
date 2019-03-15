using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [DisallowMultipleComponent]
    /// <summary>
    /// 旋转物体
    /// </summary>
    public class UguiRotateObject : UguiMultPointInputBase
    {
        protected override void DoChange()
        {
            if (pointerList.Count <= 1) return;

            var rotationDelta = 0f;
            foreach (var pointer in pointerList)
            {
                var prevFramePos= pointer.position - pointer.delta;
                var prevFrameDir = prevFramePos - screenPos;
                var dir = pointer.position - screenPos;
                rotationDelta += UguiMathf.VectorSignedAngle(prevFrameDir, dir);
            }
            rotationDelta /= pointerList.Count;

            var ray = RectTransformUtility.ScreenPointToRay(pointerList[0].enterEventCamera, screenPos);
            var plane = new Plane(ray.direction.normalized, m_Target.position);
            float enter;
            plane.Raycast(ray, out enter);
            var worldPos = ray.GetPoint(enter);
            m_Target.RotateAround(worldPos, ray.direction.normalized, rotationDelta);
        }
    }
}