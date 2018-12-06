﻿using UnityEngine;
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
        const int listInitSize = 2048;
        static readonly Quaternion rotation90;
        static readonly List<Vector2> vector2List;
        static readonly List<Vector3> vector3List;

        static UguiMathf()
        {
            rotation90 = Quaternion.FromToRotation(Vector2.up, Vector2.right);
            vector2List = new List<Vector2>(listInitSize);
            vector3List = new List<Vector3>(listInitSize);
        }

        /// <summary>
        /// 旋转向量
        /// </summary>
        /// <param name="inputDir"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 Rotation(Vector2 inputDir,float angle)
        {
            var outDir = Quaternion.AngleAxis(angle, Vector3.back)*inputDir;

            return outDir;
        }

        #region 矩形

        /// <summary>
        /// UV偏移
        /// </summary>
        /// <param name="uv"></param>
        /// <param name="uvRect"></param>
        /// <returns></returns>
        public static Vector2 UVOffset(Vector2 uv,Rect uvRect)
        {
            uv.x = uvRect.x + uvRect.width * uv.x;
            uv.y = uvRect.y + uvRect.height * uv.y;

            return uv;
        }

        /// <summary>
        /// 获取包含所有点的矩形
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Rect GetRect(IEnumerable<Vector2> points)
        {
            if (points == null)
                throw new Exception("Points is null.");

            var rect = new Rect();
            foreach (var point in points)
            {
                rect.xMax = Mathf.Max(rect.xMax, point.x);
                rect.xMin = Mathf.Min(rect.xMin, point.x);
                rect.yMax = Mathf.Max(rect.yMax, point.y);
                rect.yMin = Math.Min(rect.yMin, point.y);
            }

            return rect;
        }

        /// <summary>
        /// 合并矩形
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="rect2"></param>
        /// <returns></returns>
        public static Rect RectCombine(Rect rect, Rect rect2)
        {
            var combine = Rect.MinMaxRect(
                Mathf.Min(rect.xMin, rect2.xMin),
                Mathf.Min(rect.yMin, rect2.yMin),
                Mathf.Max(rect.xMax, rect2.xMax),
                Mathf.Max(rect.yMax, rect2.yMax)
                );

            return combine;
        }

        #endregion


        #region 面
        /// <summary>
        /// 点到面上的投影点
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 GetProjectOnPlane(Plane plane,Vector3 point)
        {
            var ray = new Ray(point, plane.normal);
            var enter = 0f;
            plane.Raycast(ray, out enter);
            var projectPoint = point + plane.normal.normalized * enter;

            return projectPoint;
        }

        public static Plane GetPlane(Transform target)
        {
            var plane = new Plane(target.forward.normalized,target.position);

            return plane;
        }

        #endregion

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
            if (value != current)
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

        private static List<Vector3> GetVector3List(List<Vector2>list)
        {
            vector3List.Clear();
            foreach (var item in list)
            {
                vector3List.Add(item);
            }

            return vector3List;
        }

        private static List<Vector2> GetVector2List(List<Vector3> list)
        {
            vector2List.Clear();
            foreach (var item in list)
            {
                vector2List.Add(item);
            }

            return vector2List;
        }

        #region 结构

        [Serializable]
        /// <summary>
        /// 线段
        /// </summary>
        public struct Line2
        {
            [SerializeField]Vector2 m_Start;
            [SerializeField] Vector2 m_End;

            public Line2(Vector2 start,Vector2 end)
                :this()
            {
                Set(start,end);
            }

            /// <summary>
            /// 设置值
            /// </summary>
            /// <param name="start">开始点</param>
            /// <param name="end">结束点</param>
            public void Set(Vector2 start, Vector2 end)
            {
                m_Start = start;
                m_End = end;
            }

            /// <summary>
            /// 获取两线段相交的点
            /// 平行或者共线视为不相交
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static Vector2? GetIntersectPoint(Line2 a,Line2 b)
            {
                var pointA = a.Start;
                var pointB = a.End;
                var pointC = b.Start;
                var pointD = b.End;

                var areaABC = (pointA.x - pointC.x) * (pointB.y - pointC.y) - (pointA.y - pointC.y) * (pointB.x-pointC.x);
                var areaABD = (pointA.x - pointD.x) * (pointB.y - pointD.y) - (pointA.y - pointD.y) * (pointB.x-pointD.x);
                if (areaABC * areaABD >= 0)
                    return null;

                var areaCDA = (pointC.x - pointA.x) * (pointD.y - pointA.y) - (pointC.y - pointA.y) * (pointD.x-pointA.x);
                var areaCDB = areaCDA + areaABC - areaABD;
                if (areaCDA * areaCDB >= 0)
                    return null;

                var t = areaCDA / (areaABD-areaABC);
                var dx = t * (pointB.x-pointA.x);
                var dy = t * (pointB.y-pointA.y);
                var intersect = new Vector2(pointA.x+dx,pointA.y+dy);

                return intersect;
            }

            /// <summary>
            /// 开始点
            /// </summary>
            public Vector2 Start
            {
                get
                {
                    return m_Start;
                }
                set
                {
                    m_Start = value;
                    Set(m_Start,m_End);
                }
            }

            /// <summary>
            /// 结束点
            /// </summary>
            public Vector2 End
            {
                get
                {
                    return m_End;
                }
                set
                {
                    m_End = value;
                    Set(m_Start, m_End);
                }
            }

            /// <summary>
            /// 点到直线距离
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public float Distance(Vector2 point)
            {
                var project = (Vector2)Vector3.Project((point - m_Start), (m_End - m_Start));
                var projectPoint = m_Start + project;

                return Vector2.Distance(point,projectPoint);
            }

            /// <summary>
            /// 获取点到线上的投射点
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public Vector2 ProjectPoint(Vector2 point)
            {
                var normal = m_End - m_Start;
                var vector = point - m_Start;
                var projectVector = (Vector2)Vector3.Project(vector, normal);
                var projectPoint = m_Start + projectVector;

                return projectPoint;
            }
        }

        [Serializable]
        /// <summary>
        /// 三角形
        /// </summary>
        public struct Triangle
        {
            [SerializeField]Vector2 m_A;
            [SerializeField] Vector2 m_B;
            [SerializeField] Vector2 m_C;

            public Triangle(Vector2 a,Vector2 b,Vector2 c)
                :this()
            {
                Set(a,b,c);
            }

            public void Set(Vector2 a, Vector2 b, Vector2 c)
            {
                m_A = a;
                m_B = b;
                m_C = c;
            }

            /// <summary>
            /// 是否包含点
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public bool Contains(Vector2 point)
            {
                var tABP = new Triangle(point,m_A,m_B);
                var tACP = new Triangle(point, m_A, m_C);
                var tBCP = new Triangle(point, m_B, m_C);
                var isContains = (tABP.Area + tACP.Area + tBCP.Area == Area)
                                ? true
                                : false;

                return isContains;
            }

            /// <summary>
            /// 面积
            /// </summary>
            public float Area
            {
                get
                {
                    var lineAB = m_A - m_B;
                    var lineBC = m_B - m_C;
                    return Vector2.Dot(lineAB,lineBC)*Vector2.Angle(lineAB,lineBC)*0.5f;
                }
            }

            /// <summary>
            /// A点
            /// </summary>
            public Vector2 A { get { return m_A; } set { m_A = value; } }

            /// <summary>
            /// B点
            /// </summary>
            public Vector2 B { get { return m_B; } set { m_B = value; } }

            /// <summary>
            /// C点
            /// </summary>
            public Vector2 C { get { return m_C; } set { m_C = value; } }

        }

        /// <summary>
        /// 圆
        /// </summary>
        [Serializable]
        public struct Circle
        {
            [SerializeField]Vector2 m_Center;
            [SerializeField]float m_Radius;

            public Circle(Vector2 center,float radius)
                :this()
            {
                Set(center,radius);
            }

            /// <summary>
            /// 设置值
            /// </summary>
            /// <param name="center">圆心</param>
            /// <param name="radius">半径</param>
            public void Set(Vector2 center,float radius)
            {
                m_Center = center;
                m_Radius = radius;
            }

            /// <summary>
            /// 获取两圆之间的外边连线
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static Line2[] GetExternalLines(Circle a,Circle b)
            {
                if (a.Center != b.Center)
                {
                    var dir = a.Center-b.Center;
                    var vertical = UguiMathf.GetVertical(a.Center, b.Center);
                    var tangentLine = new Line2(a.Center + vertical.normalized * a.Radius, b.Center + vertical.normalized * b.Radius);
                    var tangentLine2 = new Line2(a.Center - vertical.normalized * a.Radius, b.Center - vertical.normalized * b.Radius);

                    return new Line2[] { tangentLine, tangentLine2 };
                }

                return null;
            }

            /// <summary>
            /// 获取圆的相切的点
            /// </summary>
            /// <param name="outsidePoint">圆外的一点</param>
            /// <returns></returns>
            public Vector2[] GetTangentPoints(Vector2 outsidePoint)
            {
                var dist = Vector2.Distance(outsidePoint, m_Center);
                if (dist>m_Radius)
                {
                    var centerVector = m_Center - outsidePoint;
                    var tangentLength = Mathf.Sqrt(dist * dist - m_Radius * m_Radius);
                    var centerVectorAngle = Mathf.Asin(tangentLength / dist)*Mathf.Rad2Deg;
                    var p1 = outsidePoint + Rotation(centerVector, centerVectorAngle);
                    var p2 = outsidePoint + Rotation(centerVector, -centerVectorAngle);

                    return new Vector2[] {p1,p2};
                }

                return null;
            }

            /// <summary>
            /// 圆心
            /// </summary>
            public Vector2 Center
            {
                get
                {
                    return m_Center;
                }
                set
                {
                    m_Center = value;
                }
            }

            /// <summary>
            /// 半径
            /// </summary>
            public float Radius
            {
                get
                {
                    return m_Radius;
                }
                set
                {
                    m_Radius = value;
                    Set(m_Center, m_Radius);
                }
            }

            /// <summary>
            /// 周长
            /// </summary>
            public float Circumference
            {
                get
                {
                    return 2 * Mathf.PI * m_Radius;
                }
                set
                {
                    m_Radius = value / (2 * Mathf.PI);
                    Set(m_Center,m_Radius);
                }
            }

            /// <summary>
            /// 直径
            /// </summary>
            public float Diameter
            {
                get
                {
                    return m_Radius * 2;
                }
                set
                {
                    m_Radius = value * 0.5f;
                    Set(m_Center,m_Radius);
                }
            }

            /// <summary>
            /// 面积
            /// </summary>
            public float Area
            {
                get
                {
                    return Mathf.PI * m_Radius * m_Radius;
                }
                set
                {
                    m_Radius = Mathf.Sqrt(value / Mathf.PI);
                    Set(m_Center,m_Radius);
                }
            }
        }

        [Serializable]
        /// <summary>
        /// 贝塞尔曲线
        /// </summary>
        public sealed class Bezier
        {
            public const float defaultTangentPercent = 0.3f;                    //切线百分比
            public const int defaultSimpleDistance = 5;                         //采样距离
            public const int minSimpleDistance = 1;
            public const int maxSimpleDistance = 20;

            List<Segment> m_Segments;
            List<Vector3> m_ThroughPoints;
            List<Vector3> m_KeyPoints;
            Dictionary<Vector2, Segment> m_SegmentPercentDict;
            float m_Length;
            int m_SimpleDistance;
            float m_TangentPercent;

            public Bezier()
            {
                m_Segments = new List<Segment>();
                m_ThroughPoints = new List<Vector3>();
                m_KeyPoints = new List<Vector3>();
                m_SegmentPercentDict = new Dictionary<Vector2, Segment>();
            }

            public Bezier(List<Vector3> keyPoints, int simpleDistance = defaultSimpleDistance, float tangentPercent = defaultTangentPercent)
                :this()
            {
                Set(keyPoints, simpleDistance, tangentPercent);
            }

            public Bezier(List<Vector2> keyPoints, int simpleDistance = defaultSimpleDistance, float tangentPercent = defaultTangentPercent)
                : this()
            {
                Set(keyPoints, simpleDistance, tangentPercent);
            }

            public void Set(List<Vector2> keyPoints, int simpleDistance = defaultSimpleDistance, float tangentPercent=defaultTangentPercent)
            {
                Set(GetVector3List(keyPoints),simpleDistance, tangentPercent);
            }

            public void Set(List<Vector3> keyPoints, int simpleDistance = defaultSimpleDistance, float tangentPercent = defaultTangentPercent)
            {
                m_TangentPercent = Mathf.Clamp(tangentPercent,0f,0.5f);
                m_Length = 0;
                m_KeyPoints.Clear();
                m_Segments.Clear();
                m_ThroughPoints.Clear();
                m_SegmentPercentDict.Clear();
                m_SimpleDistance = Mathf.Clamp(simpleDistance, minSimpleDistance, maxSimpleDistance);

                m_KeyPoints.AddRange(keyPoints);

                if (m_KeyPoints.Count>=2)
                {
                    if (m_KeyPoints.Count==2)
                    {
                        var start = m_KeyPoints[0];
                        var end = m_KeyPoints[1];
                        var dist = Vector3.Distance(start,end);
                        var segment = new Segment(
                            start,
                            (end-start).normalized* dist* m_TangentPercent,
                            end,
                            (start-end).normalized * dist * m_TangentPercent,
                            m_SimpleDistance
                        );
                        m_Segments.Add(segment);
                    }
                    else
                    {
                        var lastTangent = Vector3.zero;
                        for (var i = 1; i < m_KeyPoints.Count - 1; i++)
                        {
                            var current = m_KeyPoints[i];
                            var left = m_KeyPoints[i - 1];
                            var right = m_KeyPoints[i + 1];
                            var leftDir = left - current;
                            var rightDir = right - current;
                            var dist = Vector3.Distance(left, current);
                            var medianDir = Vector3.Slerp(leftDir, rightDir,0.5f);
                            var tangent = UguiMathf.GetVertical(medianDir).normalized * dist * m_TangentPercent;
                            if (Vector3.Dot(tangent, leftDir) < 0) tangent = -tangent;

                            var segment = new Segment(
                                left,
                                current,
                                lastTangent.normalized* dist * m_TangentPercent + left,
                                tangent+current,
                                m_SimpleDistance
                            );
                            m_Segments.Add(segment);
                            lastTangent = -tangent;
                        }

                        //添加最后一段
                        var lastPoint = m_KeyPoints[m_KeyPoints.Count - 1];
                        var lastSegment=m_Segments[m_Segments.Count - 1];
                        var lastDist = Vector3.Distance(lastPoint,lastSegment.EndPosition);
                        lastSegment = new Segment(
                            lastSegment.EndPosition,
                            lastPoint,
                            lastTangent.normalized* lastDist* m_TangentPercent + lastSegment.EndPosition,
                            lastPoint,
                            m_SimpleDistance
                        );
                        m_Segments.Add(lastSegment);
                    }

                    foreach (var segment in m_Segments)
                    {
                        m_Length += segment.Length;
                        m_ThroughPoints.AddRange(segment.KeyPoints);
                    }
                    var lastPercent = 0f;
                    for(var i=0;i<m_Segments.Count-1;i++)
                    {
                        var percent = m_Segments[i].Length / m_Length;
                        m_SegmentPercentDict.Add(new Vector2(lastPercent, percent), m_Segments[i]);
                        lastPercent = percent;
                    }
                    m_SegmentPercentDict.Add(new Vector2(lastPercent, 1), m_Segments[m_Segments.Count-1]);
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
            /// 操控轴百分比
            /// </summary>
            public float TangentPercent
            {
                get
                {
                    return m_TangentPercent;
                }
                set
                {
                    m_TangentPercent = value;
                    Set(m_KeyPoints, m_SimpleDistance, m_TangentPercent);
                }
            }

            /// <summary>
            /// 原始关键点
            /// </summary>
            public List<Vector3> KeyPoints { get { return m_KeyPoints; } }

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
                foreach (var item in m_SegmentPercentDict)
                {
                    var range = item.Key;
                    if (range.x <= t && t <= range.y)
                    {
                        var percent = (t- range.x)/(range.y - range.x);
                        return item.Value.GetPositionByT(percent);
                    }
                }

                throw new Exception("查找错误.");
            }

            /// <summary>
            /// 获取切线
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public Vector3 GetTangentByT(float t)
            {
                foreach (var item in m_SegmentPercentDict)
                {
                    var range = item.Key;
                    if (range.x <= t && t <= range.y)
                    {
                        var percent = (t - range.x) / (range.y - range.x);
                        return item.Value.GetTangentByT(percent);
                    }
                }

                throw new Exception("查找错误.");
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
                int m_SimpleDistance;

                public Segment(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent,  Vector3 endTangent,int simpleDistance)
                {
                    Set(startPosition, endPosition, startTangent, endTangent,simpleDistance);
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="startPosition"></param>
                /// <param name="endPosition"></param>
                /// <param name="startTangent"></param>
                /// <param name="endTangent"></param>
                /// <param name="subdivide"></param>
                public void Set(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, int simpleDistance)
                {
                    m_StartPosition = startPosition;
                    m_EndPosition = endPosition;
                    m_StartTangent = startTangent;
                    m_EndTangent = endTangent;
                    m_SimpleDistance = Mathf.Clamp(simpleDistance,minSimpleDistance,maxSimpleDistance);

                    m_KeyPoints = GetKeyPoints();
                    m_Length = GetLength(m_KeyPoints);
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
                    //预采样计算长度
                    var simplePoints = new Vector3[10];
                    var simpleInterval = 1f / (simplePoints.Length - 1);
                    for (var i = 0; i < simplePoints.Length; i++)
                    {
                        simplePoints[i] = GetPositionByT(simpleInterval * i);
                    }
                    var simpleLength = GetLength(simplePoints);

                    var count = (int)(simpleLength/m_SimpleDistance);
                    count = Mathf.Max(2,count);
                    var points = new Vector3[count];
                    var interval = 1f / (points.Length - 1);
                    for (var i = 0; i < points.Length; i++)
                    {
                        points[i] = GetPositionByT(interval * i);
                    }

                    return points;
                }

                float GetLength(Vector3[] points)
                {
                    var len = 0f;
                    for (var i = 0; i < points.Length - 1; i++)
                    {
                        len += Vector3.Distance(points[i], points[i + 1]);
                    }

                    return len;
                }
            }
        }

        #endregion
    }
}