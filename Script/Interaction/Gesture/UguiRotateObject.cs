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

            var cam = pointerList[0].enterEventCamera;
            var screenPos = RectTransformUtility.WorldToScreenPoint(cam, m_Target.position);
            var rotationDelta = 0f;
            foreach (var pointer in pointerList)
            {
                var prevFramePos= pointer.position - pointer.delta;
                var prevFrameDir = prevFramePos - screenPos;
                var dir = pointer.position - screenPos;
                rotationDelta += UguiMathf.VectorSignedAngle(prevFrameDir, dir);
            }

            rotationDelta /= pointerList.Count;
            m_Target.localRotation *= Quaternion.Euler(0, 0, rotationDelta);
        }
    }
}