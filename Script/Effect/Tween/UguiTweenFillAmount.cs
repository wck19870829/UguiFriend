using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 填充百分比缓动
    /// </summary>
    public class UguiTweenFillAmount : UguiTween<float, Image>
    {
        protected UguiTweenFillAmount()
        {
            m_From = 0;
            m_To = 1;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            m_From = Mathf.Clamp(m_From,0,1);
            m_To= Mathf.Clamp(m_To, 0, 1);
        }
#endif

        protected override float RefrashView(float from, float to, float t)
        {
            var value = Mathf.Lerp(from, to, t);
            if (m_Component != null)
            {
                m_Component.fillAmount = value;
            }

            return value;
        }
    }
}