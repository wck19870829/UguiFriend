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
        [SerializeField] protected RectTransform m_Bounds;
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
            if (m_Bounds)
            {
                //限制在框内
                foreach (var child in m_ChildSet)
                {
                    var plane = new Plane(m_Bounds.transform.forward, m_Bounds.transform.position);
                    var projectPoint=UguiMathf.GetProjectOnPlane(plane,child.transform.position);
                    var localPoint = m_Bounds.parent.InverseTransformPoint(projectPoint);
                    if (!m_Bounds.rect.Contains(localPoint))
                    {
                        m_RemoveList.Add(child);
                    }
                }
            }
            else
            {
                //相对于屏幕视图坐标
                foreach (var child in m_ChildSet)
                {
                    if (!UguiTools.InScreenViewRect(child.transform.position, m_Canvas, m_ViewPortDisplayRect))
                    {
                        m_RemoveList.Add(child);
                    }
                }
            }
            foreach (var child in m_RemoveList)
            {
                m_ChildSet.Remove(child);
            }
            foreach (var child in m_RemoveList)
            {
                ProcessItemBeforeRecycle(child);

                if (OnRecycle != null)
                    OnRecycle.Invoke(child);

                ProcessItemAfterRecycle(child);
            }
            m_RemoveList.Clear();
        }

        public Rect ViewPortDisplayRect
        {
            get
            {
                return m_ViewPortDisplayRect;
            }
            set
            {
                m_ViewPortDisplayRect = value;
            }
        }

        public enum ClipKind
        {

        }
    }
}