using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 矩形约束
    /// </summary>
    public class UguiRectConstraint : UIBehaviour
    {
        [SerializeField]protected RectTransform m_Content;
        protected RectTransform m_RectTransform;

        protected override void Awake()
        {
            m_RectTransform = transform as RectTransform;
        }

        private void Update()
        {
            if (m_Content!=null)
            {
                var safeRect = GetSafeRect();
                m_RectTransform.sizeDelta = safeRect.size;
                m_RectTransform.localPosition = safeRect.position;
            }
        }

        /// <summary>
        /// 获取相对于容器的矩形
        /// </summary>
        /// <returns></returns>
        public Rect GetSafeRect()
        {
            if (m_Content == null)
                throw new Exception("Content is null.");

            var contentRect = m_Content.rect;
            var rect = (transform as RectTransform).rect;
            var safeRect = UguiMathf.LimitRect(rect,contentRect);

            return safeRect;
        }
    }
}