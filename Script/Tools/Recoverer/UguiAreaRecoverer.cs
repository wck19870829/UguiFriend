using UnityEngine;
using System.Collections;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 范围越界回收器
    /// </summary>
    public abstract class UguiAreaRecoverer<T>: UguiConditionRecoverer<T>
        where T:Component
    {
        [SerializeField] protected ClipKinds m_ClipKind;
        [SerializeField] protected Rect m_ViewPortDisplayRect;          //视图坐标系显示区域,显示区域内的物体才会被创建更新
        [SerializeField] protected RectTransform m_LimitObject;
        protected Canvas m_Canvas;

        protected UguiAreaRecoverer()
        {
            m_ViewPortDisplayRect = Rect.MinMaxRect(-0.2f, -0.2f, 1.2f, 1.2f);
            m_ClipKind = ClipKinds.ViewportLimit;
        }

        protected override void Check()
        {
            if (m_Canvas == null)
                m_Canvas = GetComponentInParent<Canvas>();
            if (m_Canvas == null) return;

            m_RemoveList.Clear();
            foreach (var child in m_ChildSet)
            {
                if (!IsInLimit(child.transform.position))
                {
                    m_RemoveList.Add(child);
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

        /// <summary>
        /// 是否在限制范围内
        /// </summary>
        /// <param name="worldPostion"></param>
        /// <returns></returns>
        public virtual bool IsInLimit(Vector3 worldPostion)
        {
            if (m_ClipKind==ClipKinds.ViewportLimit)
            {
                if (m_Canvas == null)
                    m_Canvas = GetComponentInParent<Canvas>();

                if (m_Canvas)
                {
                    return UguiTools.InScreenViewRect(worldPostion, m_Canvas, m_ViewPortDisplayRect);
                }
            }
            else if (m_ClipKind == ClipKinds.RectLimit)
            {
                if (m_LimitObject)
                {
                    var plane = new Plane(m_LimitObject.transform.forward, m_LimitObject.transform.position);
                    var projectPoint = UguiMathf.GetProjectOnPlane(plane, worldPostion);
                    var localPoint = m_LimitObject.InverseTransformPoint(projectPoint);

                    return m_LimitObject.rect.Contains(localPoint);
                }
            }
            else if (m_ClipKind==ClipKinds.Bounds)
            {
                if (m_LimitObject)
                {
                    var bounds=UguiMathf.GetBounds(m_LimitObject, Space.World);

                    return bounds.Contains(worldPostion);
                }
            }

            return false;
        }

        /// <summary>
        /// 剪切类型
        /// </summary>
        public enum ClipKinds
        {
            /// <summary>
            /// 超出屏幕视图坐标外裁切
            /// </summary>
            ViewportLimit=0,

            /// <summary>
            /// 超出物体范围外剪切
            /// </summary>
            RectLimit=1,

            /// <summary>
            /// 超出边界范围外剪切
            /// </summary>
            Bounds=2
        }
    }

    public abstract class UguiAreaRecoverer: UguiAreaRecoverer<Component>
    {

    }
}