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
    public class UguiLine : RawImage
    {
        [SerializeField] protected List<Vector2> m_Points;
        [SerializeField] protected LineStyle m_LineStyle;
        [SerializeField] protected float m_Thickness = 1;
        [SerializeField] protected AnimationCurve m_ThicknessCurve=AnimationCurve.Linear(0,1,1,1);
        [SerializeField] protected int m_SimpleDistance = UguiMathf.Bezier.defaultSimpleDistance;
        [SerializeField] protected Gradient m_Gradient;
        protected List<Vector2> m_SimplePoints;
        protected UguiMathf.Bezier m_Bezier;
        protected DrivenRectTransformTracker m_DrivenRectTransformTracker;

        protected override void Awake()
        {
            base.Awake();

            m_DrivenRectTransformTracker = new DrivenRectTransformTracker();
            rectTransform.pivot = Vector2.zero;
            m_DrivenRectTransformTracker.Add(
                gameObject,
                rectTransform,
                DrivenTransformProperties.Pivot | DrivenTransformProperties.SizeDelta
            );
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            vh.Clear();
            if (m_Points != null && m_Points.Count >= 2)
            {
                if (m_SimplePoints == null)
                    m_SimplePoints = new List<Vector2>(256);

                if (m_LineStyle == LineStyle.Straight)
                {
                    m_SimplePoints.Clear();
                    for (var i = 1; i < m_Points.Count; i++)
                    {
                        //细分
                        var currentPoint = m_Points[i];
                        var prevPoint = m_Points[i-1];
                        var currentDist = Vector2.Distance(currentPoint, prevPoint);
                        var interval = m_SimpleDistance / currentDist;
                        var count = (int)(currentDist / m_SimpleDistance);
                        for (var j=0;j<count;j++)
                        {
                            var point = Vector2.Lerp(prevPoint, currentPoint, interval * j);
                            m_SimplePoints.Add(point);
                        }
                    }
                    CreateLineMesh(ref vh, m_SimplePoints, m_Gradient,color, m_Thickness, m_ThicknessCurve, uvRect);
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
                    CreateLineMesh(ref vh, throughPoints, m_Gradient,color, m_Thickness,m_ThicknessCurve, uvRect);
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
        public static void CreateLineMesh(ref VertexHelper vh, List<Vector2> points, Gradient gradient,Color color, float thickness, AnimationCurve thicknessCurve, Rect uvRect)
        {
            vh.Clear();

            if (points != null && points.Count >= 2)
            {
                var pointsLength = 0f;
                var minThickness = 0.01f;
                for (var i=1;i<points.Count;i++)
                {
                    pointsLength+=Vector2.Distance(points[i], points[i - 1]);
                }

                var lastValue = 0f;
                var lastForwardDir = UguiMathf.GetVertical(points[0], points[1]).normalized *
                                    Mathf.Max(thicknessCurve.Evaluate(lastValue) * thickness, minThickness);
                var lastForwardPoint = points[0] + lastForwardDir;
                var lastBackPoint = points[0] - lastForwardDir;
                var lastIndex = points.Count - 1;
                for (var i = 1; i < lastIndex; i++)
                {
                    var currentPoint = points[i];
                    var prevPoint = points[i - 1];
                    var nextPoint = points[i + 1];
                    var currentDir = prevPoint - currentPoint;
                    var nextDir = nextPoint - currentPoint;

                    var currentStartValue = lastValue;
                    var currentEndValue = lastValue + currentDir.magnitude / pointsLength;
                    var currentForwardDir = UguiMathf.GetVertical(prevPoint, currentPoint).normalized *
                                            Mathf.Max(thicknessCurve.Evaluate(currentEndValue) * thickness, minThickness);
                    var currentForwardPoint = currentPoint + currentForwardDir;
                    var currentBackPoint = currentPoint - currentForwardDir;

                    var nextForwardDir = UguiMathf.GetVertical(currentPoint, nextPoint).normalized *
                                        Mathf.Max(thicknessCurve.Evaluate(currentEndValue + nextDir.magnitude / pointsLength) * thickness, minThickness);
                    var nextForwardPoint = nextPoint + nextForwardDir;
                    var nextBackPoint = nextPoint - nextForwardDir;

                    var currentForwardLine = new UguiMathf.Line(lastForwardPoint, currentForwardPoint);
                    var currentBackLine = new UguiMathf.Line(lastBackPoint,currentBackPoint);
                    var nextForwardLine = new UguiMathf.Line(nextForwardPoint, currentForwardPoint);
                    var nextBackLine = new UguiMathf.Line(nextBackPoint, currentBackPoint);
                    var forwardIntersectPoint = UguiMathf.Line.GetIntersectPoint(currentForwardLine, nextForwardLine);
                    var backIntersectPoint = UguiMathf.Line.GetIntersectPoint(currentBackLine, nextBackLine);
                    if (forwardIntersectPoint != null)
                    {
                        currentForwardPoint = (Vector2)forwardIntersectPoint;
                    }
                    else
                    {
                        //填充三角形
                    }
                    if (backIntersectPoint!=null)
                    {
                        currentBackPoint = (Vector2)backIntersectPoint;
                    }
                    else
                    {
                        //填充三角形

                    }

                    lastForwardPoint = currentForwardPoint;
                    lastBackPoint = currentBackPoint;
                    lastValue = currentEndValue;
                    lastForwardDir = currentForwardDir;
                }
            }
        }

        /// <summary>
        /// 样条线样式
        /// </summary>
        public enum LineStyle
        {
            Straight,          //直线
            Bezier             //贝塞尔曲线
        }
    }
}