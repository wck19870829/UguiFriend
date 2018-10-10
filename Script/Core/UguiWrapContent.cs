using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 元素循环
    /// </summary>
    public class UguiWrapContent : MonoBehaviour
    {
        public float spacing = 100;
        [SerializeField] protected ScrollRect scrollRect;
        [SerializeField] protected Axis axis;
        protected GridLayoutGroup gridLayoutGroup;
        protected List<RectTransform> items;

        protected virtual void Start()
        {
            if (scrollRect == null)
                throw new Exception("Scroll rect is null!");
            if (scrollRect.content==null)
                throw new Exception("Scroll rect content is null!");

            items = new List<RectTransform>();
            for (var i=0;i< scrollRect.content.childCount;i++)
            {
                items.Add(scrollRect.content.GetChild(i) as RectTransform);
            }
        }

        private void Update()
        {
            if (scrollRect == null) return;
            if (scrollRect.content == null) return;

            if (axis == Axis.Vertical)
            {
                for (var i=0;i<items.Count;i++)
                {
                    var item = items[i];

                }
            }
            else
            {
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];

                }
            }
        }

        /// <summary>
        /// 方向
        /// </summary>
        public enum Axis
        {
            Horizontal,
            Vertical
        }
    }
}