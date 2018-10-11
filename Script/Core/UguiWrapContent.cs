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
        [SerializeField]protected float spacing = 100;
        [SerializeField]protected Axis axis;
        protected ScrollRect scrollRect;
        protected Mask mask;
        protected List<RectTransform> items;
        protected Vector3[] maskCorners;
        protected bool firstTime=true;

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

            maskCorners = new Vector3[4];
            items = new List<RectTransform>();
            for (var i=0;i< scrollRect.content.childCount;i++)
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

        protected virtual void WrapContent()
        {
            if (scrollRect == null || scrollRect.content == null||mask==null) return;

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
                        localPos = new Vector3(localCenter.x, -i * spacing-spacing*0.5f);
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
                        localPos = new Vector3(i * spacing+spacing * 0.5f, localCenter.y);
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
        }
    
        public enum Axis
        {
            Horizontal,
            Vertical
        }
    }
}