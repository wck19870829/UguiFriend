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
                    m_SimplePoints = new List<Vector2>(512);
                m_SimplePoints.Clear();

                if (m_LineStyle == LineStyle.Straight)
                {
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
                    if (m_Bezier==null)
                        m_Bezier = new UguiMathf.Bezier();
                    m_Bezier.Set(m_Points, m_SimpleDistance);
                    for (var i=0;i< m_Bezier.ThroughPoints.Count;i++)
                    {
                        m_SimplePoints.Add(m_Bezier.ThroughPoints[i]);
                    }
                    CreateLineMesh(ref vh, m_SimplePoints, m_Gradient,color, m_Thickness,m_ThicknessCurve, uvRect);
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
                thickness *= 0.5f;
                var totalLength = 0f;
                for (var i=1;i<points.Count;i++)
                {
                    totalLength+=Vector2.Distance(points[i], points[i - 1]);
                }

                var cacheValue = 0f;
                var lastIndex = points.Count - 1;

                var forwardDir = UguiMathf.GetVertical(points[0],points[1])*thicknessCurve.Evaluate(0)*thickness;
                var forwardVertex = new UIVertex();
                forwardVertex.position = points[0] + forwardDir;
                forwardVertex.color = gradient.Evaluate(0) * color;
                forwardVertex.uv0 = forwardVertex.uv1 = UguiMathf.UVOffset(new Vector2(0,0),uvRect);

                var backVertex = new UIVertex();
                backVertex.position = points[0] - forwardDir;
                backVertex.color = gradient.Evaluate(0) * color;
                backVertex.uv0 = backVertex.uv1 = UguiMathf.UVOffset(new Vector2(0, 1), uvRect);

                var centerVertex = new UIVertex();
                centerVertex.position = points[0];
                centerVertex.color = gradient.Evaluate(0) * color;
                centerVertex.uv0 = centerVertex.uv1 = UguiMathf.UVOffset(new Vector2(0, 0.5f), uvRect);

                for (var i=1;i< lastIndex; i++)
                {
                    var prevPoint = points[i - 1];
                    var currentPoint = points[i];
                    var nextPoint = points[i + 1];
                    var currentDir = currentPoint - prevPoint;
                    var nextDir = nextPoint - currentPoint;

                    var currentValue = Vector2.Distance(prevPoint, currentPoint) / totalLength+cacheValue;
                    var currentThickness = thicknessCurve.Evaluate(currentValue) * thickness;
                    var currentColor = gradient.Evaluate(currentValue) * color;
                    var prevThickness= thicknessCurve.Evaluate(cacheValue) * thickness;
                    var prevColor= gradient.Evaluate(cacheValue) * color;
                    var nextValue = Vector2.Distance(currentPoint,nextPoint);
                    var nextThickness= thicknessCurve.Evaluate(nextValue) * thickness;

                    var prevCircle = new UguiMathf.Circle(prevPoint, prevThickness);
                    var currentCircle = new UguiMathf.Circle(currentPoint,currentThickness);
                    var nextCircle = new UguiMathf.Circle(nextPoint,nextThickness);
                    var currentLines = UguiMathf.Circle.GetExternalLines(prevCircle, currentCircle);
                    var nextLines = UguiMathf.Circle.GetExternalLines(currentCircle,nextCircle);

                    if (currentLines!=null&&nextLines!=null)
                    {
                        var currentForwardPoint = Vector2.zero;
                        var currentBackPoint = Vector2.zero;
                        var currentTangentLineForward = new UguiMathf.Line();
                        var currentTangentLineBack = new UguiMathf.Line();
                        if (Vector2.Dot(UguiMathf.GetVertical(prevPoint,currentPoint), currentLines[0].End-currentPoint) >= 0)
                        {
                            currentForwardPoint = currentLines[0].End;
                            currentBackPoint= currentLines[1].End;
                            currentTangentLineForward = currentLines[0];
                            currentTangentLineBack= currentLines[1];
                        }
                        else
                        {
                            currentForwardPoint = currentLines[1].End;
                            currentBackPoint = currentLines[0].End;
                            currentTangentLineForward = currentLines[1];
                            currentTangentLineBack = currentLines[0];
                        }
                        var nextLineForward = new UguiMathf.Line();
                        var nextLineBack = new UguiMathf.Line();
                        if (Vector2.Dot(UguiMathf.GetVertical(currentPoint,nextPoint), nextLines[0].End-nextPoint) >= 0)
                        {
                            nextLineForward = nextLines[0];
                            nextLineBack = nextLines[1];
                        }
                        else
                        {
                            nextLineForward = nextLines[1];
                            nextLineBack = nextLines[0];
                        }

                        var currentForwardVertex = new UIVertex();
                        currentForwardVertex.position = currentForwardPoint;
                        currentForwardVertex.color = currentColor;
                        currentForwardVertex.uv0 = currentForwardVertex.uv1 = UguiMathf.UVOffset(new Vector2(currentValue,0f),uvRect);

                        var currentBackVertex = new UIVertex();
                        currentBackVertex.position = currentBackPoint;
                        currentBackVertex.color = currentColor;
                        currentBackVertex.uv0 = currentBackVertex.uv1 = UguiMathf.UVOffset(new Vector2(currentValue, 1f), uvRect);

                        var currentCenterVertex = new UIVertex();
                        currentCenterVertex.position = currentPoint;
                        currentCenterVertex.color = currentColor;
                        currentCenterVertex.uv0 = currentCenterVertex.uv1 = UguiMathf.UVOffset(new Vector2(currentValue, 0.5f), uvRect);

                        var intersectPointForward = UguiMathf.Line.GetIntersectPoint(currentTangentLineForward, nextLineForward);
                        if (intersectPointForward!=null)
                        {
                            currentForwardVertex.position = (Vector2)intersectPointForward;
                        }

                        var intersectPointBack= UguiMathf.Line.GetIntersectPoint(currentTangentLineBack, nextLineBack);
                        if (intersectPointBack!=null)
                        {
                            currentBackVertex.position = (Vector2)intersectPointBack;
                        }

                        //创建网格
                        vh.AddVert(forwardVertex);
                        vh.AddVert(centerVertex);
                        vh.AddVert(currentCenterVertex);
                        vh.AddVert(currentForwardVertex);
                        vh.AddTriangle(vh.currentVertCount - 1, vh.currentVertCount - 2, vh.currentVertCount - 4);
                        vh.AddTriangle(vh.currentVertCount - 2, vh.currentVertCount - 3, vh.currentVertCount - 4);
                        if (intersectPointForward == null)
                        {
                            var nextLineForwardStartVertex = new UIVertex();
                            nextLineForwardStartVertex.position = nextLineForward.Start;
                            currentForwardVertex.position = nextLineForward.Start;
                            nextLineForwardStartVertex.color = currentColor;
                            nextLineForwardStartVertex.uv0 = nextLineForwardStartVertex.uv1 = currentForwardVertex.uv0;
                            vh.AddVert(nextLineForwardStartVertex);
                            vh.AddTriangle(vh.currentVertCount - 1, vh.currentVertCount - 2, vh.currentVertCount - 3);
                        }

                        vh.AddVert(centerVertex);
                        vh.AddVert(backVertex);
                        vh.AddVert(currentBackVertex);
                        vh.AddVert(currentCenterVertex);
                        vh.AddTriangle(vh.currentVertCount - 1, vh.currentVertCount - 2, vh.currentVertCount - 4);
                        vh.AddTriangle(vh.currentVertCount - 2, vh.currentVertCount - 3, vh.currentVertCount - 4);
                        if (intersectPointBack == null)
                        {
                            var nextLineBackStartVertex = new UIVertex();
                            nextLineBackStartVertex.position = nextLineBack.Start;
                            currentBackVertex.position = nextLineBack.Start;
                            nextLineBackStartVertex.color = currentColor;
                            nextLineBackStartVertex.uv0 = nextLineBackStartVertex.uv1 = currentBackVertex.uv0;
                            vh.AddVert(nextLineBackStartVertex);
                            vh.AddTriangle(vh.currentVertCount - 1, vh.currentVertCount - 2, vh.currentVertCount - 3);
                        }


                        forwardVertex =currentForwardVertex;
                        backVertex = currentBackVertex;
                        centerVertex = currentCenterVertex;
                        cacheValue = currentValue;
                    }
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