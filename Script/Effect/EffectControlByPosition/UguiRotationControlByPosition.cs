using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 通过位置权重控制旋转
    /// </summary>
    public class UguiRotationControlByPosition : UguiEffectControlByPosition<Transform, Vector3>
    {
        protected override void UpdateItem(Transform item, float weight)
        {
            var angle= Vector3.Lerp(valueFrom, valueTo, weight);
            if (isCenterMirror)
            {
                var normal = Vector3.up;
                angle = Vector3.Reflect(angle, normal);
            }
            item.localEulerAngles = angle;
        }
    }
}