using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(CanvasRenderer))]
    /// <summary>
    /// 线段
    /// </summary>
    public class UguiLine : MaskableGraphic
    {
        [SerializeField] protected List<Vector3> m_Points;
        [SerializeField] protected LineStyle m_LineStyle;
        [SerializeField] protected float m_Thickness = 1;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            if (m_Points != null && m_Points.Count >= 2)
            {
                vh.Clear();
                if (m_LineStyle==LineStyle.Straight)
                {
                    if (m_Points.Count==2)
                    {
                        var quad = UguiTools.CreateQuad(m_Points[0], m_Points[1], color, m_Thickness);
                        vh.AddUIVertexQuad(quad);
                    }
                    else if(m_Points.Count>2)
                    {
                        for (var i = 0; i < m_Points.Count - 1; i++)
                        {
                            var verts = new UIVertex[4];
                            var quad = UguiTools.CreateQuad(m_Points[i], m_Points[i + 1], color, m_Thickness);
                            vh.AddUIVertexQuad(quad);
                        }
                    }
                }
                else if (m_LineStyle == LineStyle.Bezier)
                {
                    for (var i = 0; i < m_Points.Count; i++)
                    {

                    }
                }
            }
        }

        /// <summary>
        /// 线段样式
        /// </summary>
        public virtual LineStyle StartLineStyle
        {
            get
            {
                return m_LineStyle;
            }
            set
            {
                m_LineStyle = value;
                SetVerticesDirty();
            }
        }

        public virtual List<Vector3> Points
        {
            get
            {
                return m_Points;
            }
            set
            {
                if(value==null||value.Count<2)
                {
                    throw new Exception("值不能为null,且点数量需大于等于2.");
                }

                m_Points = value;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// 粗细
        /// </summary>
        public float Thickness
        {
            get
            {
                return m_Thickness;
            }
            set
            {
                m_Thickness = value;
                SetVerticesDirty();
            }
        }

        public enum LineStyle
        {
            Straight,          //直线
            Bezier             //贝塞尔曲线
        }
    }
}