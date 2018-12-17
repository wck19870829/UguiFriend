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
    public class UguiZoomObject : UguiGestureInputBase
    {
        protected override void DoChange()
        {
            //缩小为-,放大为+
            var prevDist = 0f;
            var currentDist = 0f;
            foreach (var item in worldPointInfoList)
            {
                prevDist += Vector3.Distance(item.prev, worldCenter);
                currentDist += Vector3.Distance(item.current,worldCenter);
            }
            prevDist /= worldPointInfoList.Count;
            prevDist = Mathf.Clamp(prevDist,0.00001f,prevDist);
            currentDist/= worldPointInfoList.Count;
            transform.localScale *= currentDist / prevDist;
        }
    }
}