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

            var cam = pointerList[0].enterEventCamera;
            var screenPos = RectTransformUtility.WorldToScreenPoint(cam, m_Target.position);
            var scaleDelta = 0f;
            foreach (var pointer in pointerList)
            {
                var dir = pointer.position -screenPos;
                var normal = dir.normalized;
                var project = Vector3.Project(pointer.delta, normal);
                var deltaDist = project.magnitude;
                var prevFramePos = pointer.position - pointer.delta;
                var prevFrameDist = Vector2.Distance(prevFramePos, screenPos);
                var dist= prevFrameDist + Mathf.Sign(Vector2.Dot(project, dir)) * deltaDist;
                scaleDelta += dist/ prevFrameDist;
            }

            scaleDelta /= pointerList.Count;
            m_Target.localScale *= scaleDelta;
        }
    }
}