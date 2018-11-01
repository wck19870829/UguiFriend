using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 高度缓动
    /// </summary>
    public class UguiTweenHeight : UguiTween<float, RectTransform>
    {
        protected override float RefrashView(float from, float to, float t)
        {
            var value = Mathf.Lerp(from, to, t);
            if (m_Component != null)
            {
                var size = m_Component.sizeDelta;
                size.y = value;
                m_Component.sizeDelta = size;
            }

            return value;
        }
    }
}