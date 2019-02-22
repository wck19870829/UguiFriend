using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 绘制画布
    /// </summary>
    public class UguiPaint : UIBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        protected virtual void LateUpdate()
        {

        }

        public void OnDrag(PointerEventData eventData)
        {

        }

        protected virtual void UpdateImage()
        {

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            
        }

        public void OnEndDrag(PointerEventData eventData)
        {

        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="samplingList"></param>
        public virtual void Drawing(UguiPaintbrush brush,List<BrushSampling>samplingList)
        {

        }

        /// <summary>
        /// 画笔采样信息
        /// </summary>
        public struct BrushSampling
        {
            /// <summary>
            /// 采样点,相对于画布的位置
            /// </summary>
            public Vector2 track;
            /// <summary>
            /// 采样时间
            /// </summary>
            public float time;
        }
    }
}