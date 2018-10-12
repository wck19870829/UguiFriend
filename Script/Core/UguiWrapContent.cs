using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ScrollRect))]
    /// <summary>
    /// 循环容器
    /// </summary>
    public class UguiWrapContent : MonoBehaviour
    {
        protected static readonly AnimationCurve defaultScaleCurve= AnimationCurve.Linear(0, 0.5f, 1, 1);
        protected static readonly AnimationCurve defaultRotationCurve = AnimationCurve.Linear(0, 0.5f, 1, 1);

        [SerializeField] protected AnimationCurve rotationCurve = defaultRotationCurve; //旋转曲线，根据元素到中心点位置旋转
        [SerializeField] protected AnimationCurve scaleCurve = defaultScaleCurve;       //缩放曲线，根据元素到中心点位置缩放      
        [SerializeField] protected float spacing = 100;                                 //元素间距
        [SerializeField] protected Axis axis;
        protected ScrollRect scrollRect;
        protected Mask mask;
        protected List<RectTransform> items;
        protected Vector3[] maskCorners = new Vector3[4];
        protected bool firstTime = true;

        protected virtual void Start()
        {
            if (scrollRect == null)
                scrollRect = GetComponentInChildren<ScrollRect>();
            if (scrollRect == null)
                throw new Exception("Scroll rect is null!");
            if (scrollRect.content == null)
                throw new Exception("Scroll rect content is null!");
            scrollRect.movementType = ScrollRect.MovementType.Unrestricted;

            mask = scrollRect.GetComponentInChildren<Mask>();
            if (mask == null)
                throw new Exception("Mask is null!");

            items = new List<RectTransform>();
            for (var i = 0; i < scrollRect.content.childCount; i++)
            {
                items.Add(scrollRect.content.GetChild(i) as RectTransform);
            }

            WrapContent();
            if (Application.isPlaying)
            {
                firstTime = false;
            }
        }

        private void Update()
        {
            WrapContent();
        }

        /// <summary>
        /// 计算元素循环
        /// </summary>
        protected virtual void WrapContent()
        {
            if (scrollRect == null || scrollRect.content == null || mask == null) return;

            mask.rectTransform.GetWorldCorners(maskCorners);
            var center = Vector3.Lerp(maskCorners[0], maskCorners[2], 0.5f);
            var localCenter = scrollRect.content.InverseTransformPoint(center);
            var extents = spacing * items.Count * 0.5f;
            var ext2 = extents * 2f;
            if (axis == Axis.Vertical)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var localPos = item.localPosition;
                    var distance = localPos.y - localCenter.y;
                    if (firstTime)
                    {
                        localPos = new Vector3(localCenter.x, -i * spacing - spacing * 0.5f);
                    }
                    else if (distance < -extents)
                    {
                        localPos.y += ext2;
                    }
                    else if (distance > extents)
                    {
                        localPos.y -= ext2;
                    }
                    item.localPosition = localPos;
                }
            }
            else
            {
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var localPos = item.localPosition;
                    var distance = localPos.x - localCenter.x;
                    if (firstTime)
                    {
                        localPos = new Vector3(i * spacing + spacing * 0.5f, localCenter.y);
                    }
                    else if (distance < -extents)
                    {
                        localPos.x += ext2;
                    }
                    else if (distance > extents)
                    {
                        localPos.x -= ext2;
                    }
                    item.localPosition = localPos;
                }
            }

            //控制子元素
            foreach (var item in items)
            {
                //缩放
                var dist = Vector3.Distance(item.localPosition,localCenter);
                var time = 1-Mathf.Clamp01(dist / extents);
                var scale = scaleCurve.Evaluate(time);
                item.localScale = new Vector3(scale,scale,scale);

                //旋转
                var rotation = rotationCurve.Evaluate(1-time);
                rotation *= 90;
                switch (axis)
                {
                    case Axis.Horizontal:
                        rotation *= Mathf.Sign(item.localPosition.x-localCenter.x);
                        item.localEulerAngles = new Vector3(0, rotation, 0);
                        break;

                    case Axis.Vertical:
                        rotation *= Mathf.Sign(item.localPosition.y - localCenter.y);
                        item.localEulerAngles = new Vector3(rotation, 0, 0);
                        break;
                }
            }
        }

        /// <summary>
        /// 轴向
        /// </summary>
        public enum Axis
        {
            Horizontal,
            Vertical
        }
    }
}