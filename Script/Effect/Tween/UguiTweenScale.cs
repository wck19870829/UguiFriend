using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 缩放缓动
    /// </summary>
    public class UguiTweenScale : UguiTween<Vector3,Transform>
    {
        protected override Vector3 RefrashView(Vector3 from, Vector3 to, float t)
        {
            var value = Vector3.Lerp(from, to, t);
            if (m_Component != null)
            {
                m_Component.localScale = value;
            }

            return value;
        }
    }
}