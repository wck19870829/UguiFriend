using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 通过位置权重控制颜色
    /// </summary>
    public class UguiColorControlByPosition : UguiEffectControlByPosition<UguiColorTint, Color>
    {
        protected UguiColorControlByPosition()
        {
            valueFrom = Color.white;
            valueTo = Color.white;
        }

        protected override void UpdateItem(UguiColorTint item, float weight)
        {
            item.color = Color.Lerp(valueFrom,valueTo,weight);
        }
    }
}