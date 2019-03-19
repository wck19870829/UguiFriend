using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [DisallowMultipleComponent]
    /// <summary>
    /// 缩放物体
    /// </summary>
    public class UguiZoomObject : UguiMultPointInputBase
    {
        protected override void DoChange()
        {
            if (pointerList.Count <= 1) return;

            var center = UguiMathf.GetCenter(pointerList);
            var scaleDelta = 0f;
            foreach (var pointer in pointerList)
            {
                var dir = pointer.position -center;
                var normal = dir.normalized;
                var project = Vector3.Project(pointer.delta, normal);
                var deltaDist = project.magnitude;
                var prevFramePos = pointer.position - pointer.delta;
                var prevFrameDist = Vector2.Distance(prevFramePos, center);
                var scaleSign = Mathf.Sign(Vector2.Dot(project, dir));
                var dist= prevFrameDist + scaleSign * deltaDist;
                scaleDelta += deltaDist*scaleSign/dist;
            }
            scaleDelta /= pointerList.Count;

            var ray = RectTransformUtility.ScreenPointToRay(pointerList[0].enterEventCamera, center);
            var plane = new Plane(ray.direction.normalized, m_Target.position);
            float enter;
            plane.Raycast(ray, out enter);
            var worldPos = ray.GetPoint(enter);
            var newScale = m_Target.localScale.x + scaleDelta;
            UguiMathf.TransformScaleAround(m_Target, worldPos, new Vector3(newScale,newScale,newScale));
        }
    }
}