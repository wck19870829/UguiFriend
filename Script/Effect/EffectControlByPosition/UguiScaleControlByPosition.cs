using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 通过位置权重控制缩放
    /// </summary>
    public class UguiScaleControlByPosition : UguiEffectControlByPosition<Transform, Vector3>
    {
        protected UguiScaleControlByPosition()
        {
            valueFrom = Vector3.one;
            valueTo = Vector3.one;
        }

        protected override void UpdateItem(Transform item, float weight)
        {
            item.localScale = Vector3.Lerp(valueFrom,valueTo, weight);
        }
    }
}