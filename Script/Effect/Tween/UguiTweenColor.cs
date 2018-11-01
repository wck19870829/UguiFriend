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
        protected override Color RefrashView(Color from, Color to, float t)
        {
            var value = Color.Lerp(from, to, t);
            if (m_Component != null)
            {
                m_Component.color = value;
            }

            return value;
        }
    }
}