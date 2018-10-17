﻿using System.Collections;
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