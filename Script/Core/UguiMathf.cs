using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 数学运算
    /// </summary>
    public static class UguiMathf
    {
        static readonly Quaternion rotation90 = Quaternion.FromToRotation(Vector2.up, Vector2.right);

        /// <summary>
        /// 投射点到线上
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <returns></returns>
        public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            var normal = lineEnd - lineStart;
            var vector = point - lineStart;
            var projectVector = Vector3.Project(vector, normal);
            var projectPoint = lineStart + projectVector;

            return projectPoint;
        }

        /// <summary>
        /// 获取不相等的随机数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="current"></param>
        public static void Random(int min, int max, ref int current)
        {
            if (min == max && max == current) return;

            var value = UnityEngine.Random.Range(min, max);
            if (value == current)
            {
                current = value;
            }
            else
            {
                Random(min, max, ref current);
            }
        }

        /// <summary>
        /// 获取垂线
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetVertical(Vector2 start, Vector2 end)
        {
            var dir = (end - start).normalized;
            return (Vector2)(rotation90 * dir);
        }

        /// <summary>
        /// 获取垂线
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetVertical(Vector3 dir)
        {
            return rotation90 * dir;
        }

        #region 结构

        /// <summary>
        /// 
        /// </summary>
        public struct Line
        {
            public Line(Vector2 start,Vector2 end)
            {

            }
        }

        [Serializable]
        /// <summary>
        /// 贝塞尔曲线
        /// </summary>
        public sealed class Bezier
        {
            const float tangentPercent = 0.3f;                  //切线百分比
            public const int defaultSubdivide = 10;             //

            List<Segment> m_Segments;
            List<Vector3> m_ThroughPoints;
            float m_Length;
            int m_Subdivide;

            public Bezier()
            {
                m_Segments = new List<Segment>();
                m_ThroughPoints = new List<Vector3>();
            }

            public Bezier(List<Vector3> keyPoints, int subdivide = defaultSubdivide)
                :this()
            {
                Set(keyPoints, subdivide);
            }

            public void Set(List<Vector3> keyPoints,int subdivide)
            {
                m_Subdivide = Mathf.Max(subdivide, defaultSubdivide);
                m_Length = 0;
                m_Segments.Clear();
                m_ThroughPoints.Clear();

                if (keyPoints.Count>=2)
                {
                    if (keyPoints.Count==2)
                    {
                        var start = keyPoints[0];
                        var end = keyPoints[1];
                        var dist = Vector3.Distance(start,end);
                        var segment = new Segment(
                            start,
                            (end-start).normalized* dist* tangentPercent,
                            end,
                            (start-end).normalized * dist * tangentPercent,
                            m_Subdivide
                        );
                        m_Segments.Add(segment);
                    }
                    else
                    {
                        var lastTangent = Vector3.zero;
                        for (var i = 1; i < keyPoints.Count - 1; i++)
                        {
                            var current = keyPoints[i];
                            var left = keyPoints[i - 1];
                            var right = keyPoints[i + 1];
                            var leftDir = left - current;
                            var rightDir = right - current;
                            var dist = Vector3.Distance(left, current);
                            var medianDir = Vector3.Slerp(leftDir, rightDir,0.5f);
                            var tangent = UguiMathf.GetVertical(medianDir).normalized * dist * tangentPercent;
                            if (Vector3.Dot(tangent, leftDir) < 0) tangent = -tangent;

                            var segment = new Segment(
                                left,
                                current,
                                lastTangent.normalized* dist * tangentPercent+left,
                                tangent+current,
                                m_Subdivide
                            );
                            m_Segments.Add(segment);
                            lastTangent = -tangent;
                        }

                        //添加最后一段
                        var lastPoint = keyPoints[keyPoints.Count - 1];
                        var lastSegment=m_Segments[m_Segments.Count - 1];
                        var lastDist = Vector3.Distance(lastPoint,lastSegment.EndPosition);
                        lastSegment = new Segment(
                            lastSegment.EndPosition,
                            lastPoint,
                            lastTangent.normalized* lastDist* tangentPercent+ lastSegment.EndPosition,
                            lastPoint,
                            m_Subdivide
                        );
                        m_Segments.Add(lastSegment);
                    }

                    foreach (var segment in m_Segments)
                    {
                        m_Length += segment.Length;
                        m_ThroughPoints.AddRange(segment.KeyPoints);
                    }
                }
            }

            /// <summary>
            /// 线段集合
            /// </summary>
            public List<Segment> Segments { get { return m_Segments; } }

            /// <summary>
            /// 经过贝塞尔曲线上的点集合
            /// </summary>
            public List<Vector3> ThroughPoints { get { return m_ThroughPoints; } }

            /// <summary>
            /// 长度
            /// </summary>
            public float Length { get { return m_Length; } }

            /// <summary>
            /// 获取某一时刻贝塞尔曲线上的点
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public Vector3 GetPositionByT(float t)
            {
                return Vector3.zero;
            }

            /// <summary>
            /// 获取切线
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public Vector3 GetTangentByT(float t)
            {
                return Vector3.zero;
            }

            Segment GetSegmentByT(float t)
            {


                return null;
            }

            [Serializable]
            /// <summary>
            /// 贝塞尔曲线线段
            /// </summary>
            public sealed class Segment
            {
                Vector3 m_StartPosition;
                Vector3 m_StartTangent;
                Vector3 m_EndPosition;
                Vector3 m_EndTangent;
                float m_Length;
                Vector3[] m_KeyPoints;
                int m_Subdivide;

                public Segment(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent,  Vector3 endTangent,int subdivide)
                {
                    Set(startPosition, endPosition, startTangent, endTangent, subdivide);
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="startPosition"></param>
                /// <param name="endPosition"></param>
                /// <param name="startTangent"></param>
                /// <param name="endTangent"></param>
                /// <param name="subdivide"></param>
                public void Set(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent,int subdivide)
                {
                    m_StartPosition = startPosition;
                    m_EndPosition = endPosition;
                    m_StartTangent = startTangent;
                    m_EndTangent = endTangent;
                    m_Subdivide = subdivide;

                    m_KeyPoints = GetKeyPoints();
                    m_Length = GetLength();
                }

                /// <summary>
                /// 获取某一时刻片段上的点
                /// </summary>
                /// <param name="t">range:0-1</param>
                /// <returns></returns>
                public Vector3 GetPositionByT(float t)
                {
                    t = Mathf.Clamp01(t);

                    var oneMinusT = 1 - t;
                    var position = Mathf.Pow(oneMinusT, 3) * m_StartPosition +
                                    3 * Mathf.Pow(oneMinusT, 2) * t * m_StartTangent +
                                    3 * oneMinusT * Mathf.Pow(t, 2) * m_EndTangent +
                                    Mathf.Pow(t, 3) * m_EndPosition;

                    return position;
                }

                /// <summary>
                /// 获取某一时刻切线
                /// </summary>
                /// <param name="t"></param>
                /// <returns></returns>
                public Vector3 GetTangentByT(float t)
                {
                    var oneMinusT = 1 - t;
                    var tangent = 3 * Mathf.Pow(oneMinusT, 2) * (m_StartTangent - m_StartPosition) +
                                    6 * (m_EndTangent - m_StartTangent) * t * oneMinusT +
                                    3 * Mathf.Pow(t, 2) * (m_EndPosition - m_EndTangent);

                    return tangent.normalized;
                }

                /// <summary>
                /// 开始点
                /// </summary>
                public Vector3 StartPosition { get { return m_StartPosition; } }

                /// <summary>
                /// 开始切线
                /// </summary>
                public Vector3 StartTangent { get { return m_StartTangent; } }

                /// <summary>
                /// 结束点
                /// </summary>
                public Vector3 EndPosition { get { return m_EndPosition; } }

                /// <summary>
                /// 结束切线
                /// </summary>
                public Vector3 EndTangent { get { return m_EndTangent; } }

                /// <summary>
                /// 长度
                /// </summary>
                public float Length { get { return m_Length; } }

                /// <summary>
                /// 关键点
                /// </summary>
                public Vector3[] KeyPoints { get { return m_KeyPoints; } }

                Vector3[] GetKeyPoints()
                {
                    var points = new Vector3[m_Subdivide];
                    var interval = 1f / (points.Length - 1);
                    for (var i = 0; i < points.Length; i++)
                    {
                        points[i] = GetPositionByT(interval * i);
                    }

                    return points;
                }

                float GetLength()
                {
                    var len = 0f;
                    for (var i = 0; i < m_KeyPoints.Length - 1; i++)
                    {
                        len += Vector3.Distance(m_KeyPoints[i], m_KeyPoints[i + 1]);
                    }

                    return len;
                }
            }
        }

        #endregion
    }
}