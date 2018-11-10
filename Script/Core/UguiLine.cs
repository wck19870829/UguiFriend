﻿using UnityEngine;
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
    public class UguiLine : RawImage
    {
        [SerializeField] protected List<Vector2> m_Points;
        [SerializeField] protected LineStyle m_LineStyle;
        [SerializeField] protected float m_Thickness = 1;
        [SerializeField] protected int m_SimpleDistance = UguiMathf.Bezier.defaultSimpleDistance;
        [SerializeField] protected Gradient m_Gradient;
        protected UguiMathf.Bezier m_Bezier;
        protected List<Quad> m_Quads;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            vh.Clear();
            if (m_Points != null && m_Points.Count >= 2)
            {
                if (m_Quads == null)
                    m_Quads = new List<Quad>(1024);
                if (m_LineStyle == LineStyle.Straight)
                {
                    CreateLineMesh(ref vh,ref m_Quads, m_Points, m_Gradient,color, m_Thickness,uvRect);
                }
                else if (m_LineStyle == LineStyle.Bezier)
                {
                    var bezierPoints = new List<Vector3>(m_Points.Count);
                    for (var i=0;i< m_Points.Count;i++)
                    {
                        bezierPoints.Add(m_Points[i]);
                    }
                    if (m_Bezier==null)
                        m_Bezier = new UguiMathf.Bezier();
                    m_Bezier.Set(bezierPoints, m_SimpleDistance);
                    var throughPoints = new List<Vector2>(m_Bezier.ThroughPoints.Count);
                    for (var i=0;i< m_Bezier.ThroughPoints.Count;i++)
                    {
                        throughPoints.Add(m_Bezier.ThroughPoints[i]);
                    }
                    CreateLineMesh(ref vh,ref m_Quads, throughPoints, m_Gradient,color, m_Thickness,uvRect);
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

        /// <summary>
        /// 点集
        /// </summary>
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

        /// <summary>
        /// 由点创建网格
        /// </summary>
        /// <param name="vh"></param>
        /// <param name="points"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="thickness"></param>
        public static void CreateLineMesh(ref VertexHelper vh,ref List<Quad> quads, List<Vector2> points, Gradient gradient,Color color, float thickness,Rect uvRect)
        {
            vh.Clear();
            quads.Clear();

            if (points != null && points.Count >= 2)
            {
                var pointsLength = 0f;
                for (var i=1;i<points.Count;i++)
                {
                    pointsLength+=Vector2.Distance(points[i], points[i - 1]);
                }
                if (points.Count == 2)
                {
                    var quad = CreateQuad(points[0], points[1], thickness);
                    quad.startValue = 0;
                    quad.endValue = 1;
                    quads.Add(quad);
                }
                else
                {
                    var lastValue = 0f;
                    for (var i = 1; i < points.Count; i++)
                    {
                        var start = points[i - 1];
                        var end = points[i];
                        var dist = Vector2.Distance(start,end);
                        var value = dist / pointsLength;
                        var quad = CreateQuad(start, end, thickness);
                        quad.startValue = lastValue;
                        quad.endValue = lastValue + value;
                        quads.Add(quad);

                        lastValue += value;
                    }

                    #region 旧代码
                    //UIVertex[] lastQuad=null;
                    //var lastOffset = Vector2.zero;
                    //for (var i=1;i<points.Count-1;i++)
                    //{
                    //    var current = points[i];
                    //    var prev = points[i - 1];
                    //    var next = points[i + 1];
                    //    var offset = Vector2.Lerp(prev - current, next - current, 0.5f).normalized;
                    //    offset *= thickness/Mathf.Sin(Mathf.Deg2Rad*0.5f * Vector2.Angle(prev - current, next - current));
                    //    var vertical = GetVertical(prev, current);
                    //    offset *= Mathf.Sign(Vector2.Dot(offset, vertical));
                    //    lastOffset=offset;

                    //    var currentPointUp = new UIVertex();
                    //    currentPointUp.position = current + offset;
                    //    var currentPointDown = new UIVertex();
                    //    currentPointDown.position = current - offset;

                    //    if (lastQuad == null)
                    //    {
                    //        //添加第一段
                    //        var startPoint = points[0];
                    //        var startPointVertical = GetVertical(startPoint, current)*thickness;
                    //        startPointVertical*= Mathf.Sign(Vector2.Dot(lastOffset, startPointVertical));
                    //        var startQuadPointUp = new UIVertex();
                    //        startQuadPointUp.position = startPoint + lastOffset;
                    //        var startQuadPointDown = new UIVertex();
                    //        startQuadPointDown.position = startPoint - lastOffset;
                    //        lastQuad = new UIVertex[] {
                    //            startQuadPointUp,
                    //            startQuadPointDown,
                    //            currentPointDown,
                    //            currentPointUp
                    //        };
                    //        vertexexTemp.Add(lastQuad);
                    //    }
                    //    else
                    //    {
                    //        var quad = new UIVertex[] 
                    //        {
                    //            lastQuad[3],
                    //            lastQuad[2],
                    //            currentPointDown,
                    //            currentPointUp
                    //        };
                    //        vertexexTemp.Add(quad);
                    //        lastQuad = quad;
                    //    }
                    //}

                    ////添加最后一段
                    //var lastPoint = points[points.Count-1];
                    //var lastPointVertical = GetVertical(points[points.Count - 2], lastPoint)*thickness;
                    //lastPointVertical *= Mathf.Sign(Vector2.Dot(lastOffset,lastPointVertical));
                    //var lastQuadPointUp = new UIVertex();
                    //lastQuadPointUp.position = lastPoint + lastPointVertical;
                    //var lastQuadPointDown = new UIVertex();
                    //lastQuadPointDown.position = lastPoint - lastPointVertical;
                    //lastQuad = new UIVertex[] {
                    //    lastQuad[3],
                    //    lastQuad[2],
                    //    lastQuadPointDown,
                    //    lastQuadPointUp
                    //};
                    //vertexexTemp.Add(lastQuad);
                    #endregion
                }
                for (var i = 0; i < quads.Count; i++)
                {
                    var quad = quads[i];
                    var startColor=gradient.Evaluate(quad.startValue)*color;
                    var endColor = gradient.Evaluate(quad.endValue)*color;
                    quad.SetColor(startColor, startColor, endColor, endColor);
                    quad.SetUV0(
                        new Vector2(quad.startValue,0),
                        new Vector2(quad.startValue,1),
                        new Vector2(quad.endValue, 0),
                        new Vector2(quad.endValue, 1)
                    );
                    vh.AddUIVertexQuad(quad.GetVertexes());
                }
            }
        }

        /// <summary>
        /// 创建直线的网格
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static Quad CreateQuad(Vector2 start, Vector2 end, float thickness)
        {
            var offset = UguiTools.GetVertical(start, end) * thickness;

            var startUp = new UIVertex();
            startUp.position = start + offset;

            var startDown = new UIVertex();
            startDown.position = start - offset;

            var endUp = new UIVertex();
            endUp.position = end + offset;

            var endDown = new UIVertex();
            endDown.position = end - offset;

            var quad = new Quad();
            quad.startUp = startUp;
            quad.startDown = startDown;
            quad.endUp = endUp;
            quad.endDown = endDown;
            quad.start = start;
            quad.end = end;

            return quad;
        }

        /// <summary>
        /// 样条线样式
        /// </summary>
        public enum LineStyle
        {
            Straight,          //直线
            Bezier             //贝塞尔曲线
        }

        /// <summary>
        /// 方形面片
        /// </summary>
        public struct Quad
        {
            public Vector2 start;
            public Vector2 end;
            public UIVertex startUp;
            public UIVertex startDown;
            public UIVertex endUp;
            public UIVertex endDown;
            public float startValue;
            public float endValue;

            public void SetUV0(Vector2 startUpValue, Vector2 startDownValue, Vector2 endUpValue, Vector2 endDownValue)
            {
                startUp.uv0 = startUpValue;
                startDown.uv0 = startDownValue;
                endUp.uv0 = endUpValue;
                endDown.uv0 = endDownValue;
            }

            public void SetUV1(Vector2 startUpValue, Vector2 startDownValue, Vector2 endUpValue, Vector2 endDownValue)
            {
                startUp.uv1 = startUpValue;
                startDown.uv1 = startDownValue;
                endUp.uv1 = endUpValue;
                endDown.uv1 = endDownValue;
            }

            public void SetColor(Color startUpColor,Color startDownColor,Color endUpColor,Color endDownColor)
            {
                startUp.color = startUpColor;
                startDown.color = startDownColor;
                endUp.color = endUpColor;
                endDown.color = endDownColor;
            }

            public UIVertex[] GetVertexes()
            {
                var vertexes = new UIVertex[]
                {
                    startUp,startDown,endDown,endUp
                };

                return vertexes;
            }
        }
    }
}