using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 宽高缓动
    /// </summary>
    public class UguiTweenSizeDelta : UguiTween<Vector2, RectTransform>
    {
        protected override Vector2 RefrashView(Vector2 from, Vector2 to, float t)
        {
            var value = Vector2.Lerp(from, to, t);
            if (m_Component != null)
            {
                m_Component.sizeDelta = value;
            }

            return value;
        }
    }
}