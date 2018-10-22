using System;
using System.Collections;
using System.Collections.Generic;
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

        /// <summary>
        /// 获取物体的边界(包含所有子物体)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Rect GetRectIncludeChildren(RectTransform content,Space space)
        {
            var children = content.GetComponentsInChildren<RectTransform>();
            var cornersArr = new Vector3[4];
            content.GetWorldCorners(cornersArr);
            var contentRect = GetRectContainsPoints(cornersArr);
            foreach (var child in children)
            {
                child.GetWorldCorners(cornersArr);
                contentRect = RectCombine(contentRect, GetRectContainsPoints(cornersArr));
            }

            if (space == Space.Self)
            {
                if (content.parent == null)
                    throw new Exception("Content's parent is null.");

                var localCorners = new Vector3[] {
                    new Vector3(contentRect.xMin,contentRect.yMin),
                    new Vector3(contentRect.xMax,contentRect.yMin),
                    new Vector3(contentRect.xMax,contentRect.yMax),
                    new Vector3(contentRect.xMin,contentRect.yMax)
                };
                for (var i=0;i< localCorners.Length;i++)
                {
                    localCorners[i] = content.parent.worldToLocalMatrix.MultiplyPoint(localCorners[i]);
                }
                contentRect = GetRectContainsPoints(localCorners);
            }

            return contentRect;
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
            if (points == null || points.Length == 0) return new Rect();

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