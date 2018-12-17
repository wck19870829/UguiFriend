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
    public class UguiRotateObject : UguiGestureInputBase
    {
        protected override void DoChange()
        {
            var eulerAnglesOffset = Vector3.zero;
            foreach (var item in worldPointInfoList)
            {
                var prevDir = item.prev-worldCenter;
                var currentDir = item.current - worldCenter;
                var rotation = Quaternion.FromToRotation(prevDir, currentDir);
                eulerAnglesOffset += rotation.eulerAngles;
            }
            eulerAnglesOffset /= worldPointInfoList.Count;
            transform.eulerAngles = transform.eulerAngles + eulerAnglesOffset;
        }
    }
}