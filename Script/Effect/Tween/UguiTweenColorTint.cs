using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 颜色染色缓动(更改自身及子物体)
    /// </summary>
    public class UguiTweenColorTint : UguiTween<Color,UguiColorTint>
    {
        protected override Color Lerp(Color from, Color to, float t)
        {
            return Color.Lerp(from, to, t);
        }

        protected override void RefrashView(Color value)
        {
            if (m_Component != null)
            {
                m_Component.Color = value;
            }
        }
    }
}