using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// UI工具类
    /// </summary>
    public static class UguiTools
    {
        /// <summary>
        /// 销毁所有子元素
        /// </summary>
        /// <param name="go"></param>
        public static void DestroyChildren(GameObject go)
        {
            for (var i = go.transform.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(go.transform.GetChild(i).gameObject);
            }
        }

        static readonly Quaternion rotation90 = Quaternion.FromToRotation(Vector2.up, Vector2.right);

        /// <summary>
        /// 创建直线的网格
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static UIVertex[] CreateQuad(Vector2 start,Vector2 end,Color color,float thickness=1)
        {
            var dir = end - start;
            var offset = (Vector2)(rotation90*dir).normalized*thickness;

            var p1 = new UIVertex();
            p1.position = start + offset;
            p1.color = color;

            var p2 = new UIVertex();
            p2.position = end + offset;
            p2.color = color;

            var p3 = new UIVertex();
            p3.position = end - offset;
            p3.color = color;

            var p4 = new UIVertex();
            p4.position = start - offset;
            p4.color = color;

            var vertexes = new UIVertex[]
            {
                p1,p2,p3,p4
            };

            return vertexes;
        }

        /// <summary>
        /// 获取物体的世界坐标系边界(递归包含所有子物体)
        /// </summary>
        /// <param name="content"></param>
        /// <param name="includeContent">true:包含content。false:不包含content</param>
        /// <returns></returns>
        public static Rect? GetGlobalRectIncludeChildren(RectTransform content,bool includeContent)
        {
            if (content == null)
                throw new Exception("Content is null.");

            var children = content.GetComponentsInChildren<RectTransform>();
            if (children.Length == 1 && !includeContent) return null;

            Rect? contentRect = null;
            var cornersArr = new Vector3[4];
            foreach (var child in children)
            {
                if (!includeContent)
                {
                    if (child == content)
                    {
                        continue;
                    }
                }

                if (contentRect == null)
                {
                    child.GetWorldCorners(cornersArr);
                    contentRect= GetRectContainsPoints(cornersArr);
                }
                child.GetWorldCorners(cornersArr);
                var childRect = GetRectContainsPoints(cornersArr);
                contentRect = RectCombine((Rect)contentRect,childRect);
            }

            return contentRect;
        }

        /// <summary>
        /// 获取相对于其他容器的局部坐标系边界(包含所有子物体)
        /// </summary>
        /// <param name="content">容器</param>
        /// <param name="relative">相对于</param>
        /// <param name="includeContent">true:包含content。false:不包含content</param>
        /// <returns></returns>
        public static Rect? GetLocalRectIncludeChildren(RectTransform content,RectTransform relative,bool includeContent)
        {
            if (relative == null)
                throw new Exception("Relative is null.");

            var contentRect = GetGlobalRectIncludeChildren(content, includeContent);
            if (contentRect == null) return null;

            var rect = (Rect)contentRect;
            var corners = new Vector3[]
            {
                new Vector3(rect.xMin,rect.yMin),
                new Vector3(rect.xMax,rect.yMin),
                new Vector3(rect.xMax,rect.yMax),
                new Vector3(rect.xMin,rect.yMax)
            };
            for (var i = 0; i < corners.Length; i++)
            {
                corners[i] = relative.worldToLocalMatrix.MultiplyPoint(corners[i]);
            }
            rect = GetRectContainsPoints(corners);

            return rect;
        }

        /// <summary>
        /// 合并矩形
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="rect2"></param>
        /// <returns></returns>
        public static Rect RectCombine(Rect rect,Rect rect2)
        {
            var combine = Rect.MinMaxRect(
                Mathf.Min(rect.xMin,rect2.xMin),
                Mathf.Min(rect.yMin, rect2.yMin),
                Mathf.Max(rect.xMax, rect2.xMax),
                Mathf.Max(rect.yMax, rect2.yMax)
                );

            return combine;
        }

        /// <summary>
        /// 获取包含所有点的矩形
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Rect GetRectContainsPoints(Vector3[]points)
        {
            if (points == null)
                throw new Exception("Points is null.");
            if (points.Length == 0)
                throw new Exception("Points length is zero.");

            var rect = new Rect(points[0], Vector3.zero);
            foreach (var point in points)
            {
                rect.xMax = Mathf.Max(rect.xMax,point.x);
                rect.xMin = Mathf.Min(rect.xMin, point.x);
                rect.yMax = Mathf.Max(rect.yMax,point.y);
                rect.yMin = Math.Min(rect.yMin, point.y);
            }

            return rect;
        }

        /// <summary>
        /// 点从局部坐标转换为世界坐标
        /// </summary>
        /// <param name="localPoints"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Vector3[] LocalPoints2GlobalPoints(Vector3[] localPoints, Transform content)
        {
            if (content == null)
                throw new Exception("Content is null.");
            if (localPoints == null)
                throw new Exception("Local points is null.");

            var globalPoints = new Vector3[localPoints.Length];
            for (var i = 0; i < localPoints.Length; i++)
            {
                globalPoints[i] = content.TransformPoint(localPoints[i]);
            }

            return globalPoints;
        }

        /// <summary>
        /// 点从世界坐标转换为局部坐标
        /// </summary>
        /// <param name="globalPoints"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Vector3[] GlobalPoints2LocalPoints(Vector3[] globalPoints, Transform content)
        {
            if(content==null)
                throw new Exception("Content is null.");
            if (globalPoints == null)
                throw new Exception("Global points is null.");

            var localPoints = new Vector3[globalPoints.Length];
            for (var i=0;i<localPoints.Length;i++)
            {
                localPoints[i] = content.InverseTransformPoint(globalPoints[i]);
            }

            return localPoints;
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
        /// 投射点到线上
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <returns></returns>
        public static Vector3 ProjectPointLine(Vector3 point,Vector3 lineStart,Vector3 lineEnd)
        {
            var normal = lineEnd - lineStart;
            var vector = point - lineStart;
            var projectVector=Vector3.Project(vector, normal);
            var projectPoint = lineStart+ projectVector;

            return projectPoint;
        }

        /// <summary>
        /// 设置锚点
        /// </summary>
        /// <param name="target"></param>
        /// <param name="allign"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public static void SetAnchor(RectTransform target, AnchorPresets anchorPresets, int offsetX = 0, int offsetY = 0)
        {
            target.anchoredPosition = new Vector3(offsetX, offsetY, 0);

            switch (anchorPresets)
            {
                case (AnchorPresets.TopLeft):
                    {
                        target.anchorMin = new Vector2(0, 1);
                        target.anchorMax = new Vector2(0, 1);
                        break;
                    }

                case (AnchorPresets.TopCenter):
                    {
                        target.anchorMin = new Vector2(0.5f, 1);
                        target.anchorMax = new Vector2(0.5f, 1);
                        break;
                    }

                case (AnchorPresets.TopRight):
                    {
                        target.anchorMin = new Vector2(1, 1);
                        target.anchorMax = new Vector2(1, 1);
                        break;
                    }

                case (AnchorPresets.MiddleLeft):
                    {
                        target.anchorMin = new Vector2(0, 0.5f);
                        target.anchorMax = new Vector2(0, 0.5f);
                        break;
                    }

                case (AnchorPresets.MiddleCenter):
                    {
                        target.anchorMin = new Vector2(0.5f, 0.5f);
                        target.anchorMax = new Vector2(0.5f, 0.5f);
                        break;
                    }

                case (AnchorPresets.MiddleRight):
                    {
                        target.anchorMin = new Vector2(1, 0.5f);
                        target.anchorMax = new Vector2(1, 0.5f);
                        break;
                    }

                case (AnchorPresets.BottomLeft):
                    {
                        target.anchorMin = new Vector2(0, 0);
                        target.anchorMax = new Vector2(0, 0);
                        break;
                    }

                case (AnchorPresets.BottonCenter):
                    {
                        target.anchorMin = new Vector2(0.5f, 0);
                        target.anchorMax = new Vector2(0.5f, 0);
                        break;
                    }

                case (AnchorPresets.BottomRight):
                    {
                        target.anchorMin = new Vector2(1, 0);
                        target.anchorMax = new Vector2(1, 0);
                        break;
                    }

                case (AnchorPresets.HorStretchTop):
                    {
                        target.anchorMin = new Vector2(0, 1);
                        target.anchorMax = new Vector2(1, 1);
                        break;
                    }

                case (AnchorPresets.HorStretchMiddle):
                    {
                        target.anchorMin = new Vector2(0, 0.5f);
                        target.anchorMax = new Vector2(1, 0.5f);
                        break;
                    }

                case (AnchorPresets.HorStretchBottom):
                    {
                        target.anchorMin = new Vector2(0, 0);
                        target.anchorMax = new Vector2(1, 0);
                        break;
                    }

                case (AnchorPresets.VertStretchLeft):
                    {
                        target.anchorMin = new Vector2(0, 0);
                        target.anchorMax = new Vector2(0, 1);
                        break;
                    }

                case (AnchorPresets.VertStretchCenter):
                    {
                        target.anchorMin = new Vector2(0.5f, 0);
                        target.anchorMax = new Vector2(0.5f, 1);
                        break;
                    }

                case (AnchorPresets.VertStretchRight):
                    {
                        target.anchorMin = new Vector2(1, 0);
                        target.anchorMax = new Vector2(1, 1);
                        break;
                    }

                case (AnchorPresets.StretchAll):
                    {
                        target.anchorMin = new Vector2(0, 0);
                        target.anchorMax = new Vector2(1, 1);
                        break;
                    }
            }
        }

        /// <summary>
        /// 设置轴心点
        /// </summary>
        /// <param name="target"></param>
        /// <param name="preset"></param>
        public static void SetPivot(RectTransform target, PivotPresets pivotPresets)
        {
            switch (pivotPresets)
            {
                case (PivotPresets.TopLeft): target.pivot = new Vector2(0, 1); break;
                case (PivotPresets.TopCenter): target.pivot = new Vector2(0.5f, 1); break;
                case (PivotPresets.TopRight): target.pivot = new Vector2(1, 1); break;
                case (PivotPresets.MiddleLeft): target.pivot = new Vector2(0, 0.5f); break;
                case (PivotPresets.MiddleCenter): target.pivot = new Vector2(0.5f, 0.5f); break;
                case (PivotPresets.MiddleRight): target.pivot = new Vector2(1, 0.5f); break;
                case (PivotPresets.BottomLeft): target.pivot = new Vector2(0, 0); break;
                case (PivotPresets.BottomCenter): target.pivot = new Vector2(0.5f, 0); break;
                case (PivotPresets.BottomRight): target.pivot = new Vector2(1, 0); break;
            }
        }

        /// <summary>
        /// 复制一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T AddChild<T>(T source, Transform parent = null) where T : Component
        {
            if (source == null)
                throw new Exception("Source is null.");

            var clone = GameObject.Instantiate<T>(source);
            if (parent != null)
                clone.transform.SetParent(parent);
            clone.transform.localScale = Vector3.one;
            clone.transform.localRotation = Quaternion.identity;
            clone.transform.localPosition = Vector3.zero;

            return clone;
        }

        #region 其他

        /// <summary>
        /// byte[]转string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Bytes2String(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// string转byte[]
        /// </summary>
        /// <param name="str"></param>
        public static byte[] String2Bytes(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// 二进制序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeObject(object obj)
        {
            if (obj == null)
                return null;

            using (var ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                byte[] bytes = ms.GetBuffer();
                ms.Close();

                return bytes;
            }
        }

        /// <summary>
        /// 二进制反序列化对象
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static object DeserializeObject(byte[] bytes)
        {
            if (bytes == null||bytes.Length==0)
                return null;

            using (var ms = new MemoryStream())
            {
                ms.Position = 0;
                var formatter = new BinaryFormatter();
                var obj = formatter.Deserialize(ms);
                ms.Close();

                return obj;
            }
        }

        #endregion
    }

    /// <summary>
    /// 锚点预设，对应编辑面板中的预设
    /// </summary>
    public enum AnchorPresets
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottonCenter,
        BottomRight,
        BottomStretch,
        VertStretchLeft,
        VertStretchRight,
        VertStretchCenter,
        HorStretchTop,
        HorStretchMiddle,
        HorStretchBottom,
        StretchAll
    }

    /// <summary>
    /// 轴心点预设
    /// </summary>
    public enum PivotPresets
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
}