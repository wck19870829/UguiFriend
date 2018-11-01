using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 缩放缓动
    /// </summary>
    public class UguiTweenScale : UguiTween<Vector3,Transform>
    {
        protected override Vector3 Lerp(Vector3 from, Vector3 to, float t)
        {
            return Vector3.Lerp(from,to,t);
        }

        protected override void RefrashView(Vector3 value)
        {

        }
    }
}