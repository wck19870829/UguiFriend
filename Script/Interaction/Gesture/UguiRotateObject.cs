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

            //点集合按照移动距离从小到大排序,取移动距离最小的点和最大的点作为参考点计算偏移
            pointerList.Sort((a,b)=> {
                if (a.delta.sqrMagnitude == b.delta.sqrMagnitude) return 0;
                return a.delta.sqrMagnitude > b.delta.sqrMagnitude ? 1 : -1;
            });
            var min = pointerList[0];
            var max = pointerList[pointerList.Count - 1];
            var minPrevPoint = min.position - min.delta;
            var maxPrevPoint = max.position - max.delta;
            var prevDir = maxPrevPoint - minPrevPoint;
            var dir = max.position - min.position;
            var sum = min.delta.sqrMagnitude + max.delta.sqrMagnitude;
            if (sum == 0) return;
            var center = minPrevPoint + prevDir * (min.delta.sqrMagnitude / sum);

            var ray = RectTransformUtility.ScreenPointToRay(pointerList[0].enterEventCamera, center);
            var plane = new Plane(ray.direction.normalized, m_Target.position);
            float enter;
            plane.Raycast(ray, out enter);
            var worldPos = ray.GetPoint(enter);
            m_Target.RotateAround(worldPos, ray.direction.normalized, UguiMathf.VectorSignedAngle(prevDir,dir));
        }
    }
}