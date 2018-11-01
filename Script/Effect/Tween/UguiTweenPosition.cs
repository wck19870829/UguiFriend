using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 位置缓动
    /// </summary>
    public class UguiTweenPosition : UguiTween<Vector3,Transform>
    {
        [SerializeField] protected Space m_Sapce = Space.Self;

        protected override Vector3 RefrashView(Vector3 from, Vector3 to, float t)
        {
            var value = Vector3.Lerp(from, to, t);
            if (m_Component != null)
            {
                if (m_Sapce == Space.Self) m_Component.localPosition = value;
                else if (m_Sapce == Space.World) m_Component.position = value;
            }

            return value;
        }
    }
}