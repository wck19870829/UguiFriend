using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 颜色染色缓动(更改自身及子物体)
    /// </summary>
    public class UguiTweenColorTint : UguiTween<Color,UguiColorTint>
    {
        protected override Color RefrashView(Color from, Color to, float t)
        {
            var value = Color.Lerp(from, to, t);
            if (m_Component != null)
            {
                m_Component.Color = value;
            }

            return value;
        }
    }
}