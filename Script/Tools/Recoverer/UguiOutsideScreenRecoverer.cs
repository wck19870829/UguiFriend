using UnityEngine;
using System.Collections;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 屏幕越界回收器
    /// </summary>
    public abstract class UguiOutsideScreenRecoverer<T>: UguiConditionRecoverer<T>
        where T:Component
    {
        [SerializeField] protected Rect m_ViewPortDisplayRect;          //视图坐标系显示区域,显示区域内的物体才会被创建更新
        protected Canvas m_Canvas;

        protected UguiOutsideScreenRecoverer()
        {
            m_ViewPortDisplayRect = Rect.MinMaxRect(-0.2f, -0.2f, 1.2f, 1.2f);
        }

        protected override void Check()
        {
            if (m_Canvas == null)
                m_Canvas = GetComponentInParent<Canvas>();
            if (m_Canvas == null) return;

            m_RemoveList.Clear();
            foreach (var child in m_ChildSet)
            {
                var worldPoint = child.transform.position;
                var screenPoint = RectTransformUtility.WorldToScreenPoint(m_Canvas.rootCanvas.worldCamera, worldPoint);
                var viewportPoint = UguiMathf.ScreenPoint2ViewportPoint(screenPoint);
                if (!m_ViewPortDisplayRect.Contains(viewportPoint))
                {
                    m_RemoveList.Add(child);
                }
            }
            foreach (var child in m_RemoveList)
            {
                m_ChildSet.Remove(child);
            }
            if (OnRecycle != null)
            {
                foreach (var child in m_RemoveList)
                {
                    ProcessItemBeforeRecycle(child);
                    OnRecycle.Invoke(child);
                }
            }
            m_RemoveList.Clear();
        }
    }
}