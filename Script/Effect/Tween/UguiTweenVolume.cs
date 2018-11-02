using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 声音缓动
    /// </summary>
    public class UguiTweenVolume : UguiTween<float, AudioSource>
    {
        protected override float RefrashView(float from, float to, float t)
        {
            var value = Mathf.Lerp(from, to, t);
            if (m_Component != null)
            {
                m_Component.volume = value;
            }

            return value;
        }
    }
}