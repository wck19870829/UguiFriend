﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        /// 是否为有效数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValidNumber(Vector2 value)
        {
            var isNaN = float.IsNaN(value.x) ||
                        float.IsNaN(value.y);

            return !isNaN;
        }

        /// <summary>
        /// 是否为有效数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValidNumber(Vector3 value)
        {
            var isNaN = float.IsNaN(value.x) ||
                        float.IsNaN(value.y) ||
                        float.IsNaN(value.z);

            return !isNaN;
        }

        /// <summary>
        /// 是否为有效数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValidNumber(Vector4 value)
        {
            var isNaN = float.IsNaN(value.x) ||
                        float.IsNaN(value.y) ||
                        float.IsNaN(value.z) ||
                        float.IsNaN(value.w);

            return !isNaN;
        }

        /// <summary>
        /// 返回最小的日期
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static DateTime Min(params DateTime[] values)
        {
            try
            {
                var value = values[0];
                for (var i=0;i<values.Length;i++)
                {
                    value = values[i] < value ? values[i] : value;
                }

                return value;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        /// <summary>
        /// 返回最大的日期
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static DateTime Max(params DateTime[] values)
        {
            try
            {
                var value = values[0];
                for (var i = 0; i < values.Length; i++)
                {
                    value = values[i] > value ? values[i] : value;
                }

                return value;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        /// <summary>
        /// 获取中心点
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Vector2 GetCenter(List<PointerEventData>points)
        {
            if (points == null)
                throw new Exception("Points is null.");
            if (points.Count == 0)
                throw new Exception("Points'count is zero.");

            if (points.Count == 1)
                return points[0].position;

            var xMin = points[0].position.x;
            var xMax = xMin;
            var yMin = points[0].position.y;
            var yMax = yMin;
            foreach (var point in points)
            {
                xMin = Mathf.Min(xMin, point.position.x);
                xMax = Mathf.Max(xMax, point.position.x);
                yMin = Mathf.Min(yMin, point.position.y);
                yMax = Mathf.Max(yMax, point.position.y);
            }

            var center = new Vector2(
                        Mathf.Lerp(xMin, xMax, 0.5f),
                        Mathf.Lerp(yMin, yMax, 0.5f)
                        );
            return center;
        }

        /// <summary>
        /// 获取中心点
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Vector2 GetCenter(List<Vector2>points)
        {
            if (points == null)
                throw new Exception("Points is null.");
            if (points.Count == 0)
                throw new Exception("Points'count is zero.");

            if (points.Count == 1)
                return points[0];

            var xMin = points[0].x;
            var xMax = xMin;
            var yMin = points[0].y;
            var yMax = yMin;
            foreach (var point in points)
            {
                xMin = Mathf.Min(xMin, point.x);
                xMax = Mathf.Max(xMax, point.x);
                yMin = Mathf.Min(yMin, point.y);
                yMax = Mathf.Max(yMax, point.y);
            }

            var center = new Vector3(
                        Mathf.Lerp(xMin, xMax, 0.5f),
                        Mathf.Lerp(yMin, yMax, 0.5f)
                        );
            return center;
        }

        /// <summary>
        /// 获取中心点
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Vector3 GetCenter(List<Vector3>points)
        {
            if (points == null)
                throw new Exception("Points is null.");
            if (points.Count == 0)
                throw new Exception("Points'count is zero.");

            if (points.Count == 1)
                return points[0];

            var xMin = points[0].x;
            var xMax = xMin;
            var yMin= points[0].y;
            var yMax = yMin;
            var zMin= points[0].z;
            var zMax = zMin;
            foreach (var point in points)
            {
                xMin = Mathf.Min(xMin, point.x);
                xMax = Mathf.Max(xMax, point.x);
                yMin = Mathf.Min(yMin, point.y);
                yMax = Mathf.Max(yMax, point.y);
                zMin = Mathf.Min(zMin, point.z);
                zMax = Mathf.Max(zMax, point.z);
            }

            var center= new Vector3(
                        Mathf.Lerp(xMin,xMax,0.5f),
                        Mathf.Lerp(yMin, yMax, 0.5f),
                        Mathf.Lerp(zMin, zMax, 0.5f)
                        );
            return center;
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

        #region PointerData

        /// <summary>
        /// 获取屏幕坐标下中心点
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public static Vector2? GetScreenPos(List<PointerEventData> pointerList)
        {
            if (pointerList != null|| pointerList.Count>0)
            {
                var screenPos = pointerList[0].position;
                for (var i = 1; i < pointerList.Count; i++)
                {
                    screenPos = Vector2.Lerp(screenPos, pointerList[i].position, 0.5f);
                }
                return screenPos;
            }

            return null;
        }

        #endregion

        #region Vector

        /// <summary>
        /// 获取带符号的角度
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float VectorSignedAngle(Vector2 from,Vector2 to)
        {
            var angle = Vector2.Angle(from, to);
            float sign = Mathf.Sign(from.x * to.y - from.y * to.x);

            return angle*sign;
        }

        #endregion

        #region Transform

        /// <summary>
        /// 相对于世界中心点的缩放
        /// </summary>
        /// <param name="target"></param>
        /// <param name="worldCenter"></param>
        /// <param name="newScale"></param>
        public static void TransformScaleAround(Transform target,Vector3 worldCenter,Vector3 newScale)
        {
            if (!target) return;

            var pivot = target.parent.InverseTransformPoint(worldCenter);
            Vector3 A = target.transform.localPosition;
            Vector3 B = pivot;
            Vector3 C = A - B; // diff from object pivot to desired pivot/origin

            float RS = newScale.x / target.transform.localScale.x; // relataive scale factor

            // calc final position post-scale
            Vector3 FP = B + C * RS;

            // finally, actually perform the scale/translation
            target.transform.localScale = newScale;
            target.transform.localPosition = FP;
        }

        #endregion

        #region RectTransform

        /// <summary>
        /// 是否在屏幕视图坐标中
        /// </summary>
        /// <param name="worldPoint"></param>
        /// <param name="canvas"></param>
        /// <param name="viewRect"></param>
        /// <returns></returns>
        public static bool InScreenViewRect(Vector3 worldPoint, Canvas canvas, Rect viewRect)
        {
            if (canvas == null)
                throw new Exception("Canvas is null.");

            Camera cam = null;
            if (canvas.rootCanvas.worldCamera != null)
            {
                if (canvas.rootCanvas.renderMode == RenderMode.ScreenSpaceCamera
                    ||canvas.rootCanvas.renderMode == RenderMode.WorldSpace)
                {
                    cam = canvas.rootCanvas.worldCamera;
                }
            }

            var screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPoint);
            var viewportPoint = UguiMathf.ScreenPoint2ViewportPoint(screenPoint);

            return viewRect.Contains(viewportPoint);
        }

        /// <summary>
        /// 获取全局坐标系ui元素尺寸
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Rect? GetGlobalGraphicRectIncludeChildren(RectTransform content)
        {
            if (content == null)
                throw new Exception("Content is null.");

            var children = content.GetComponentsInChildren<Graphic>();
            if (children.Length == 0) return null;

            Rect? globalRect = null;
            var cornersArr = new Vector3[4];
            foreach (var graphic in children)
            {
                graphic.rectTransform.GetWorldCorners(cornersArr);
                var childRect = GetRect(cornersArr);

                var mask = graphic.GetComponentInParent<Mask>();
                if (mask != null)
                {
                    mask.rectTransform.GetWorldCorners(cornersArr);
                    var maskRect = GetRect(cornersArr);
                    var overlap = UguiMathf.RectOverlap(childRect, maskRect);
                    if (overlap != null)
                    {
                        childRect = (Rect)overlap;
                    }
                    else continue;
                }

                if (globalRect == null) globalRect = childRect;
                globalRect = UguiMathf.RectCombine(childRect, (Rect)globalRect);
            }

            return globalRect;
        }

        /// <summary>
        /// 获取物体的世界坐标系边界(递归包含所有子物体)
        /// </summary>
        /// <param name="content"></param>
        /// <param name="includeContent">是否包含容器自身</param>
        /// <returns></returns>
        public static Rect? GetGlobalRectIncludeChildren(RectTransform content, bool includeContent)
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
                    contentRect = GetRect(cornersArr);
                }
                child.GetWorldCorners(cornersArr);
                var childRect = GetRect(cornersArr);
                contentRect = UguiMathf.RectCombine((Rect)contentRect, childRect);
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
        public static Rect? GetLocalRectIncludeChildren(RectTransform content, RectTransform relative, bool includeContent)
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
            rect = GetRect(corners);

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
            if (content == null)
                throw new Exception("Content is null.");
            if (globalPoints == null)
                throw new Exception("Global points is null.");

            var localPoints = new Vector3[globalPoints.Length];
            for (var i = 0; i < localPoints.Length; i++)
            {
                localPoints[i] = content.InverseTransformPoint(globalPoints[i]);
            }

            return localPoints;
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
                case AnchorPresets.TopLeft:
                    target.anchorMin = new Vector2(0, 1);
                    target.anchorMax = new Vector2(0, 1);
                    break;

                case AnchorPresets.TopCenter:
                    target.anchorMin = new Vector2(0.5f, 1);
                    target.anchorMax = new Vector2(0.5f, 1);
                    break;

                case AnchorPresets.TopRight:
                    target.anchorMin = new Vector2(1, 1);
                    target.anchorMax = new Vector2(1, 1);
                    break;

                case AnchorPresets.MiddleLeft:
                    target.anchorMin = new Vector2(0, 0.5f);
                    target.anchorMax = new Vector2(0, 0.5f);
                    break;

                case AnchorPresets.MiddleCenter:
                    target.anchorMin = new Vector2(0.5f, 0.5f);
                    target.anchorMax = new Vector2(0.5f, 0.5f);
                    break;

                case AnchorPresets.MiddleRight:
                    target.anchorMin = new Vector2(1, 0.5f);
                    target.anchorMax = new Vector2(1, 0.5f);
                    break;

                case AnchorPresets.BottomLeft:
                    target.anchorMin = new Vector2(0, 0);
                    target.anchorMax = new Vector2(0, 0);
                    break;

                case AnchorPresets.BottonCenter:
                    target.anchorMin = new Vector2(0.5f, 0);
                    target.anchorMax = new Vector2(0.5f, 0);
                    break;

                case AnchorPresets.BottomRight:
                    target.anchorMin = new Vector2(1, 0);
                    target.anchorMax = new Vector2(1, 0);
                    break;

                case AnchorPresets.HorStretchTop:
                    target.anchorMin = new Vector2(0, 1);
                    target.anchorMax = new Vector2(1, 1);
                    break;

                case AnchorPresets.HorStretchMiddle:
                    target.anchorMin = new Vector2(0, 0.5f);
                    target.anchorMax = new Vector2(1, 0.5f);
                    break;

                case AnchorPresets.HorStretchBottom:
                    target.anchorMin = new Vector2(0, 0);
                    target.anchorMax = new Vector2(1, 0);
                    break;

                case AnchorPresets.VertStretchLeft:
                    target.anchorMin = new Vector2(0, 0);
                    target.anchorMax = new Vector2(0, 1);
                    break;

                case AnchorPresets.VertStretchCenter:
                    target.anchorMin = new Vector2(0.5f, 0);
                    target.anchorMax = new Vector2(0.5f, 1);
                    break;

                case AnchorPresets.VertStretchRight:
                    target.anchorMin = new Vector2(1, 0);
                    target.anchorMax = new Vector2(1, 1);
                    break;

                case AnchorPresets.StretchAll:
                    target.anchorMin = new Vector2(0, 0);
                    target.anchorMax = new Vector2(1, 1);
                    break;
            }
        }

        /// <summary>
        /// 获取轴心点
        /// </summary>
        /// <param name="pivot"></param>
        /// <returns></returns>
        public static Vector2 GetPivotValue(UguiPivot pivot)
        {
            switch (pivot)
            {
                case (UguiPivot.TopLeft): return new Vector2(0, 1);
                case (UguiPivot.Top): return new Vector2(0.5f, 1);
                case (UguiPivot.TopRight): return new Vector2(1, 1);
                case (UguiPivot.Left): return new Vector2(0, 0.5f);
                case (UguiPivot.Center): return new Vector2(0.5f, 0.5f);
                case (UguiPivot.Right): return new Vector2(1, 0.5f);
                case (UguiPivot.BottomLeft): return new Vector2(0, 0);
                case (UguiPivot.Bottom): return new Vector2(0.5f, 0);
                case (UguiPivot.BottomRight): return new Vector2(1, 0);
            }

            return Vector2.zero;
        }

        /// <summary>
        /// 获取屏幕点对应的轴点
        /// </summary>
        /// <param name="target"></param>
        /// <param name="screenPos"></param>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public static Vector2 GetPivot(RectTransform target,Vector2 screenPos,Camera camera)
        {
            var pivot = target.pivot;
            var localPos = Vector2.zero;
            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(target,screenPos,camera,out localPos))
            {
                var rect = target.rect;
                pivot = new Vector2((localPos.x-rect.x)/rect.width,(localPos.y-rect.y)/rect.height);
            }

            return pivot;
        }

        /// <summary>
        /// 设置轴心点(不改变状态)
        /// </summary>
        /// <param name="target"></param>
        public static void SetPivot(RectTransform target,List<PointerEventData> dataList)
        {
            if (dataList == null || dataList.Count == 0) return;

            //获取屏幕中心点
            var points = new Vector2[dataList.Count];
            for (var i=0;i<dataList.Count;i++)
            {
                points[i] = dataList[i].position;
            }
            var rect = GetRect(points);
            var pivot = GetPivot(target,rect.center,dataList[0].enterEventCamera);

            SetPivot(target, pivot);
        }

        /// <summary>
        /// 设置轴心点(不改变状态)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="pivot"></param>
        public static void SetPivot(RectTransform target, UguiPivot pivot)
        {
            var pivotValue = GetPivotValue(pivot);
            SetPivot(target, pivotValue);
        }

        /// <summary>
        /// 设置轴心点(不改变状态)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="pivot"></param>
        public static void SetPivot(RectTransform target, Vector2 pivot)
        {
            if (!target) return;

            var offset=pivot - target.pivot;
            offset.Scale(target.rect.size);
            var wordlPos= target.position + target.TransformVector(offset);

            target.pivot = pivot;
            target.position = wordlPos;
        }

        /// <summary>
        /// 获取相对矩形
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Rect GetLocalRect(RectTransform parent, RectTransform target)
        {
            if (parent == null)
                throw new Exception("Parent is null.");
            if (target == null)
                throw new Exception("Target is null.");

            var corners = new Vector3[4];
            target.GetWorldCorners(corners);
            for (var i = 0; i < 4; i++)
            {
                corners[i] = parent.transform.InverseTransformPoint(corners[i]);
            }
            var rect = GetRect(corners);

            return rect;
        }

        /// <summary>
        /// 缩放RectTransform
        /// </summary>
        /// <param name="scaleMode"></param>
        /// <param name="rect"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void ScaleRectTransform(RectTransform rect, ScaleMode scaleMode, float top,float bottom,float left,float right)
        {
            var parent = rect.parent as RectTransform;
            var parentRect = parent.rect;
            var limitRect = parentRect;
            limitRect.yMax -= top;
            limitRect.yMin += bottom;
            limitRect.xMax -= right;
            limitRect.xMin += left;
            var localRect = GetLocalRect(parent, rect);
            var scaleRect = RectScale(localRect, limitRect, scaleMode);
            top = Mathf.Abs(parentRect.yMax - scaleRect.yMax)*(scaleRect.yMax>parentRect.yMax?-1:1);
            bottom = Mathf.Abs(parentRect.yMin - scaleRect.yMin) * (scaleRect.yMin>parentRect.yMin?1:-1);
            left = Mathf.Abs(parentRect.xMin - scaleRect.xMin)*(scaleRect.xMin>parentRect.xMin?1:-1);
            right = Mathf.Abs(parentRect.xMax - scaleRect.xMax) * (scaleRect.xMax > parentRect.xMax ? -1 : 1);

            AdjustRectTransform(rect, top, bottom, left, right);
        }

        /// <summary>
        /// 缩放RectTransform
        /// </summary>
        /// <param name="scaleMode"></param>
        /// <param name="rect"></param>
        public static void ScaleRectTransform(RectTransform rect, ScaleMode scaleMode)
        {
            ScaleRectTransform(rect, scaleMode, 0,0,0,0);
        }

        /// <summary>
        /// 获取相对坐标
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="worldPoint"></param>
        /// <returns></returns>
        public static Vector2 GetLocalPointInRectangle(Transform rect,Vector3 worldPoint)
        {
            var plane = GetPlane(rect);
            worldPoint = GetProjectOnPlane(plane, worldPoint);
            var localPoint=rect.InverseTransformPoint(worldPoint);

            return localPoint;
        }

        /// <summary>
        /// 以目标当前旋转为基准，约束目标在相对父级框内(直接改变位置缩放)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="relative"></param>
        public static void LimitRectTransformWithMoveAndScale(RectTransform target,RectTransform relative)
        {
            var localPosition=Vector3.zero;
            var localScale = Vector3.zero;
            LimitRectTransformWithMoveAndScale(target, relative,ref localPosition,ref localScale);
            target.localPosition = localPosition;
            target.localScale = localScale;
        }

        /// <summary>
        /// 以目标当前旋转为基准，约束目标在相对父级框内(不直接改变位置缩放)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="relative"></param>
        /// <param name="newLocalPostion"></param>
        /// <param name="newLocalScale"></param>
        public static void LimitRectTransformWithMoveAndScale(RectTransform target, RectTransform relative,ref Vector3 newLocalPostion,ref Vector3 newLocalScale)
        {
            var relativeRect = UguiMathf.GetLocalRect(target, relative);
            var scaleRect = LimitRect(relativeRect, target.rect);
            var scale = relativeRect.width / scaleRect.width;
            newLocalScale= target.localScale * scale;
            var worldPos = target.position + target.TransformVector(relativeRect.center - scaleRect.center);
            newLocalPostion = target.parent.InverseTransformPoint(worldPos);
        }

        /// <summary>
        /// 限制在父级框内
        /// </summary>
        /// <param name="target"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void LimitRectTransform(RectTransform target, float top, float bottom, float left, float right)
        {
            if (!target) return;

            var edgeDist = GetRectTransformEdgeDistance(target);
            top = Mathf.Max(edgeDist.x, top);
            bottom = Mathf.Max(edgeDist.y,bottom);
            left = Mathf.Max(edgeDist.z,left);
            right = Mathf.Max(edgeDist.w,right);

            AdjustRectTransform(target, top, bottom, left, right);
        }

        /// <summary>
        /// 限制在父级框内
        /// </summary>
        /// <param name="target"></param>
        /// <param name="scaleMode"></param>
        /// <param name="aspectRatio">宽高比</param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void LimitRectTransform(RectTransform target,ScaleMode scaleMode,float aspectRatio, float top, float bottom, float left, float right)
        {
            if (aspectRatio == 0f)
                throw new Exception("Aspect ratio is zero.");

            var rect = target.rect;
            var width = rect.width;
            var height = width/aspectRatio;
            target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            ScaleRectTransform(target, scaleMode, top, bottom, left, right);
        }

        /// <summary>
        /// 调整
        /// </summary>
        /// <param name="target"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void AdjustRectTransform(RectTransform target, float top, float bottom, float left, float right)
        {
            if (!target) return;

            var parent = target.parent as RectTransform;
            target.offsetMin = new Vector2(left, bottom);
            target.offsetMax = new Vector2(-right,-top);
        }

        /// <summary>
        /// 到边的距离
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="worldPoint"></param>
        /// <returns>返回Vector4(top,bottom,left,right)</returns>
        public static Vector4 GetRectTransformEdgeDistance(RectTransform rect,Vector3 worldPoint)
        {
            if (!rect)
                throw new Exception("Rect is null.");

            var r = rect.rect;
            var localPoint = GetLocalPointInRectangle(rect, worldPoint);
            var edgeDist = new Vector4(
                            r.yMax-localPoint.y,
                            r.yMin-localPoint.y,
                            r.xMin-localPoint.x,
                            r.xMax-localPoint.x);

            return edgeDist;
        }

        /// <summary>
        /// 到父级容器的边距离
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Vector4 GetRectTransformEdgeDistance(RectTransform rect)
        {
            return GetRectTransformEdgeDistance(rect,rect.parent as RectTransform);
        }

        /// <summary>
        /// 相对容器坐标系下到相对容器的边距离
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="relative"></param>
        /// <returns>返回Vector4(top,bottom,left,right)</returns>
        public static Vector4 GetRectTransformEdgeDistance(RectTransform rect, RectTransform relative)
        {
            if (!rect)
                throw new Exception("Rect is null.");
            if (!relative)
                throw new Exception("Relative is null.");

            var relativeRect = relative.rect;
            var corners = new Vector3[4];
            var cornersV2 = new Vector2[4];
            rect.GetWorldCorners(corners);
            for (var i = 0; i < 4; i++)
            {
                cornersV2[i] = GetLocalPointInRectangle(relative, corners[i]);
            }
            var r = GetRect(cornersV2);
            var edgeDist = new Vector4(
                            Mathf.Abs(relativeRect.yMax - r.yMax) * (r.yMax > relativeRect.yMax ? -1 : 1),
                            Mathf.Abs(relativeRect.yMin - r.yMin) * (r.yMin > relativeRect.yMin ? 1 : -1),
                            Mathf.Abs(relativeRect.xMin - r.xMin) * (r.xMin > relativeRect.xMin ? 1 : -1),
                            Mathf.Abs(relativeRect.xMax - r.xMax) * (r.xMax > relativeRect.xMax ? -1 : 1));

            return edgeDist;
        }

        #endregion

        #region Bounds

        /// <summary>
        /// 获取Bounds
        /// </summary>
        /// <param name="target"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        public static Bounds GetBounds(RectTransform target, Space space)
        {
            var corners = new Vector3[4];
            if (space == Space.World)
            {
                target.GetWorldCorners(corners);
            }
            else
            {
                target.GetLocalCorners(corners);
            }
            var bounds = new Bounds(corners[0], Vector3.zero);
            foreach (var point in corners)
            {
                bounds.Encapsulate(point);
            }

            return bounds;
        }

        /// <summary>
        /// 获取Bounds(包含子物体)
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="includeInactive">是否包含未激活物体</param>
        /// <returns></returns>
        public static Bounds GetGlobalBoundsIncludeChildren(RectTransform target, bool includeInactive=false)
        {
            var bounds = GetBounds(target, Space.World);

            var children = target.GetComponentsInChildren<RectTransform>(includeInactive);
            foreach (var child in children)
            {
                var mask = child.GetComponent<Mask>();
                if (mask != null && mask.showMaskGraphic == false) continue;

                var childBounds = GetBounds(child, Space.World);
                var parentMask = child.GetComponentInParent<Mask>();
                if (parentMask != null)
                {
                    var maskBounds = GetBounds(parentMask.rectTransform, Space.World);
                    var overlap = BoundsOverlap(maskBounds, childBounds);
                    if (overlap != null)
                    {
                        bounds.Encapsulate((Bounds)overlap);
                    }
                    continue;
                }
                bounds.Encapsulate(childBounds);
            }

            return bounds;
        }

        /// <summary>
        /// 限制Bounds在另外一个Bounds中
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="limitRect"></param>
        /// <returns></returns>
        public static Bounds LimitBounds(Bounds bounds,Bounds content)
        {
            //限制尺寸
            var sizeX = Mathf.Min(content.size.x, bounds.size.x);
            var sizeY = Mathf.Min(content.size.y, bounds.size.y);
            var sizeZ = Mathf.Min(content.size.z, bounds.size.z);
            bounds.size = new Vector3(sizeX,sizeY,sizeZ);

            //外框并集
            var outBounds = content;
            outBounds.Encapsulate(bounds);

            //偏移
            var offsetX = Mathf.Sign(content.center.x - bounds.center.x) * Mathf.Abs(outBounds.size.x - content.size.x);
            var offsetY = Mathf.Sign(content.center.y - bounds.center.y) * Mathf.Abs(outBounds.size.y - content.size.y);
            var offsetZ = Mathf.Sign(content.center.z - bounds.center.z) * Mathf.Abs(outBounds.size.z - content.size.z);
            bounds.center += new Vector3(offsetX,offsetY,offsetZ);

            return bounds;
        }
        
        /// <summary>
        /// 获取两个Bounds交集,无交集返回null
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Bounds? BoundsOverlap(Bounds a,Bounds b)
        {
            if (a.Intersects(b))
            {
                var xMin = Mathf.Max(a.min.x, b.min.x);
                var yMin = Mathf.Max(a.min.y, b.min.y);
                var zMin = Mathf.Max(a.min.z, b.min.z);
                var xMax= Mathf.Min(a.max.x, b.max.x);
                var yMax = Mathf.Min(a.max.y, b.max.y);
                var zMax = Mathf.Min(a.max.z, b.max.z);

                var bounds = new Bounds();
                bounds.SetMinMax(new Vector3(xMin,yMin,zMin),new Vector3(xMax,yMax,zMax));

                return bounds;
            }

            return null;
        }

        #endregion

        #region Rect

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
        /// 获取在屏幕上的投影矩形
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static Rect GetScreenRect(Bounds bounds,Camera camera)
        {
            var p1=RectTransformUtility.WorldToScreenPoint(camera, bounds.min);
            var p2= RectTransformUtility.WorldToScreenPoint(camera, bounds.max);
            var points = new Vector2[]
            {
                p1,
                p2
            };
            var rect = GetRect(points);

            return rect;
        }

        /// <summary>
        /// 屏幕坐标转视图坐标
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public static Vector2 ScreenPoint2ViewportPoint(Vector2 screenPoint)
        {
            var viewportPoint = new Vector2(
                                screenPoint.x/Screen.width,
                                screenPoint.y/Screen.height);

            return viewportPoint;
        }

        /// <summary>
        /// 获取包含所有点的矩形
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Rect GetRect(Vector3[]points)
        {
            if (points == null)
                throw new Exception("Points is null.");
            if (points.Length == 0)
                throw new Exception("Points length is zero.");

            var rect = new Rect(points[0], Vector2.zero);
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
        /// 获取包含所有点的矩形
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Rect GetRect(Vector2[] points)
        {
            if (points == null)
                throw new Exception("Points is null.");
            if (points.Length == 0)
                throw new Exception("Points length is zero.");

            var rect = new Rect(points[0], Vector2.zero);
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
        /// 获取包含所有点的矩形
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Rect GetRect(List<Vector2> points)
        {
            if (points == null)
                throw new Exception("Points is null.");
            if (points.Count == 0)
                throw new Exception("Points length is zero.");

            var rect = new Rect(points[0], Vector2.zero);
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
        /// 获取两矩形相交区域,无相交区域返回null
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="rect2"></param>
        /// <returns></returns>
        public static Rect? RectOverlap(Rect rect, Rect rect2)
        {
            if (rect.Overlaps(rect2, true))
            {
                var overlap=Rect.MinMaxRect(
                    Mathf.Max(rect.xMin,rect2.xMin),
                    Mathf.Max(rect.yMin,rect2.yMin),
                    Mathf.Min(rect.xMax,rect2.xMax),
                    Mathf.Min(rect.yMax,rect2.yMax));

                return overlap;
            }

            return null;
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
                Mathf.Max(rect.yMax, rect2.yMax));

            return combine;
        }

        /// <summary>
        /// 限制矩形在另外一个矩形中
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Rect LimitRect(Rect rect,Rect content)
        {
            var scaleRect = RectScale(rect, content, ScaleMode.ScaleToFit);
            rect.width = Mathf.Min(rect.width,scaleRect.width);
            rect.height = Mathf.Min(rect.height,scaleRect.height);
            var width = Mathf.Min(Mathf.Abs(rect.width),Mathf.Abs(content.width));
            var height = Mathf.Min(Mathf.Abs(rect.height), Mathf.Abs(content.height));
            rect.xMin = Mathf.Clamp(rect.xMin,content.xMin, content.xMax-width);
            rect.yMin = Mathf.Clamp(rect.yMin, content.yMin, content.yMax - height);
            rect.xMax = rect.xMin + width;
            rect.yMax = rect.yMin + height;

            return rect;
        }

        /// <summary>
        /// 规格化矩形为width>=0,height>=0
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rect RectNormalize(Rect rect)
        {
            rect = Rect.MinMaxRect(
                    Mathf.Min(rect.xMin,rect.xMax),
                    Mathf.Min(rect.yMin, rect.yMax),
                    Mathf.Max(rect.xMin, rect.xMax),
                    Mathf.Max(rect.yMin, rect.yMax));

            return rect;
        }

        /// <summary>
        /// 填充容器
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="content"></param>
        /// <param name="scaleMode"></param>
        /// <returns></returns>
        public static Rect RectScale(Rect rect,Rect content, ScaleMode scaleMode)
        {
            if (content.width == 0 || content.height == 0)
                throw new Exception("无效的容器矩形:"+content);

            rect = RectNormalize(rect);
            content = RectNormalize(content);

            switch (scaleMode)
            {
                case ScaleMode.ScaleToFit:
                    var ratio = Mathf.Max(rect.width/content.width, rect.height/content.height);
                    rect.width /= ratio;
                    rect.height /= ratio;
                    rect.x=content.x-(rect.width - content.width) * 0.5f;
                    rect.y = content.y - (rect.height - content.height) * 0.5f;
                    break;

                case ScaleMode.ScaleAndCrop:
                    var ratio2 = Mathf.Min(rect.width / content.width, rect.height / content.height);
                    rect.width /= ratio2;
                    rect.height /= ratio2;
                    rect.x = content.x - (rect.width - content.width) * 0.5f;
                    rect.y = content.y - (rect.height - content.height) * 0.5f;
                    break;

                case ScaleMode.StretchToFill:
                    rect = content;
                    break;
            }

            return rect;
        }

        /// <summary>
        /// 包含进去点
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Rect RectEncapsulate(Rect rect,Vector2 point)
        {
            rect.xMin = Mathf.Min(point.x, rect.xMin);
            rect.xMax = Mathf.Max(point.x,rect.xMax);
            rect.yMin = Math.Min(point.y,rect.yMin);
            rect.yMax = Math.Max(point.y,rect.yMax);

            return rect;
        }

        #endregion

        #region Plane
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
            var isEnter=plane.Raycast(ray, out enter);
            if (isEnter == false && enter == 0)
            {
                //平行返回原始点
                return point;
            }

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
            /// 点到直线距离(有可能在延长线上投影)
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
            /// 点到线段最小距离
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public float ClosestDistance(Vector2 point)
            {
                var cross = (m_End.x - m_Start.x) * (point.x - m_Start.x) + (m_End.y - m_Start.y) * (point.y - m_Start.y);
                if (cross <= 0) return Mathf.Sqrt((point.x - m_Start.x) * (point.x - m_Start.x) + (point.x - m_Start.y) * (point.x - m_Start.y));

                var d2 = (m_End.x - m_Start.x) * (m_End.x - m_Start.x) + (m_End.y - m_Start.y) * (m_End.y - m_Start.y);
                if (cross >= d2) return Mathf.Sqrt((point.x - m_End.x) * (point.x - m_End.x) + (point.x - m_End.y) * (point.x - m_End.y));

                var r = cross / d2;
                var px = m_Start.x + (m_End.x - m_Start.x) * r;
                var py = m_Start.y + (m_End.y - m_Start.y) * r;
                return Mathf.Sqrt((point.x - px) * (point.x - px) + (py - m_Start.y) * (py - m_Start.y));
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
                var projectVector = (Vector2)Vector3.Project(vector, normal.normalized);
                var projectPoint = m_Start + projectVector;

                return projectPoint;
            }

            /// <summary>
            /// 判断点是否在直线上
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public bool PointIsInLine(Vector2 point)
            {
                return false;
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

            /// <summary>
            /// 获取最近的点
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public Vector3 GetClosestPoint(Vector3 point)
            {
                return Vector3.zero;
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
}