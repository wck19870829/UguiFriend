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
        [SerializeField] protected List<Vector2> m_Points;
        [SerializeField] protected LineStyle m_LineStyle;
        [SerializeField] protected float m_Thickness = 1;
        protected UguiMathf.Bezier m_Bezier;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            vh.Clear();
            if (m_Points != null && m_Points.Count >= 2)
            {
                if (m_LineStyle == LineStyle.Straight)
                {
                    UguiTools.CreateLineMesh(ref vh, m_Points, color, m_Thickness);
                }
                else if (m_LineStyle == LineStyle.Bezier)
                {
                    var bezierPoints = new List<Vector3>(m_Points.Count);
                    for (var i=0;i< m_Points.Count;i++)
                    {
                        bezierPoints.Add(m_Points[i]);
                    }
                    m_Bezier = new UguiMathf.Bezier(bezierPoints);
                    var throughPoints = new List<Vector2>(m_Bezier.ThroughPoints.Count);
                    for (var i=0;i< m_Bezier.ThroughPoints.Count;i++)
                    {
                        throughPoints.Add(m_Bezier.ThroughPoints[i]);
                    }
                    UguiTools.CreateLineMesh(ref vh, throughPoints, color, m_Thickness);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (m_Bezier!=null)
            {
                foreach (var segment in m_Bezier.Segments)
                {
                    UnityEditor.Handles.color = Color.red;
                    UnityEditor.Handles.DrawLine(segment.StartPosition, segment.StartTangent);
                    UnityEditor.Handles.DrawSphere(0, segment.StartPosition, Quaternion.identity, 10);

                    UnityEditor.Handles.color = Color.yellow;
                    UnityEditor.Handles.DrawLine(segment.EndPosition,segment.EndTangent);

                    UnityEditor.Handles.color = Color.yellow;
                    UnityEditor.Handles.DrawBezier(segment.StartPosition, segment.EndPosition, segment.StartTangent, segment.EndTangent, Color.white,null, 5);
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

        public virtual List<Vector2> Points
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