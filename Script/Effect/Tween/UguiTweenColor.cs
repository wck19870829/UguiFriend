using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 颜色缓动(单个物体)
    /// </summary>
    public class UguiTweenColor : UguiTween<Color,Graphic>
    {
        protected override Color Lerp(Color from, Color to, float t)
        {
            return Color.Lerp(from, to, t);
        }

        protected override void RefrashView(Color value)
        {
            if (m_Component != null)
            {
                m_Component.color = value;
            }
        }
    }
}