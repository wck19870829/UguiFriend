using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 尺寸约束
    /// </summary>
    public class UguiSizeConstraint : UIBehaviour
    {
        [SerializeField] protected int m_MinWidth = 100;
        [SerializeField] protected int m_MaxWidth = 9999;
        [SerializeField] protected int m_MinHeight = 100;
        [SerializeField] protected int m_MaxHeight = 9999;

        private void Update()
        {
            var safeSize = GetSafeSize();
            (transform as RectTransform).sizeDelta = safeSize;
        }

        /// <summary>
        /// 获取安全尺寸
        /// </summary>
        /// <returns></returns>
        public Vector2 GetSafeSize()
        {
            var rect = (transform as RectTransform).rect;
            var size = new Vector2(
                        Mathf.Clamp(rect.width, m_MinWidth, m_MaxWidth),
                        Mathf.Clamp(rect.height, m_MinHeight, m_MaxHeight)
                        );
            return size;
        }
    }
}