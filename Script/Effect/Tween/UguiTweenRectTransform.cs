using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 两个物体间变换的缓动
    /// </summary>
    public class UguiTweenRectTransform : UguiTween<RectTransform, RectTransform>
    {
        protected override RectTransform RefrashView(RectTransform from, RectTransform to, float t)
        {
            if (from != null && to != null&&m_Component!=null)
            {
                m_Component.position = Vector3.Lerp(from.position, to.position, t);
                m_Component.eulerAngles = Vector3.Lerp(from.eulerAngles, to.eulerAngles, t);
                m_Component.localScale = Vector3.Lerp(from.localScale, to.localScale, t);
                m_Component.pivot = Vector2.Lerp(from.pivot, to.pivot, t);
                m_Component.anchorMax = Vector2.Lerp(from.anchorMax, to.anchorMax, t);
                m_Component.anchorMin = Vector2.Lerp(from.anchorMin,to.anchorMin,t);
                m_Component.anchoredPosition = Vector2.Lerp(from.anchoredPosition,to.anchoredPosition,t);
                m_Component.anchoredPosition3D = Vector3.Lerp(from.anchoredPosition3D,to.anchoredPosition3D,t);
            }

            return null;
        }
    }
}