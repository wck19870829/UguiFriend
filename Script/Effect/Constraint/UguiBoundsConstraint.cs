using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 边界约束
    /// </summary>
    public class UguiBoundsConstraint : UIBehaviour
    {
        [SerializeField] protected RectTransform m_Content;

        private void Update()
        {
            if (m_Content!=null)
            {
                var safeBounds = GetGlobalSafeBounds();
                transform.position = safeBounds.center;
            }
        }

        /// <summary>
        /// 获取全局坐标系安全边界
        /// </summary>
        /// <returns></returns>
        public Bounds GetGlobalSafeBounds()
        {
            if (m_Content == null)
                throw new Exception("Content is null.");

            var contentBounds = UguiMathf.GetBounds(m_Content, Space.World);
            var bounds = UguiMathf.GetGlobalBoundsIncludeChildren(transform as RectTransform);
            bounds = UguiMathf.LimitBounds(bounds, contentBounds);
            return bounds;
        }
    }
}