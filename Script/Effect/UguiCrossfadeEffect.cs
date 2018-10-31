using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 淡入淡出切换图片
    /// </summary>
    public class UguiCrossfadeEffect
    {
        [SerializeField] protected float interval=5;                //每张图片显示时间
        [SerializeField] protected RectTransform content;
        [SerializeField] protected WrapMode wrapMode;
        protected int m_Index;
        protected List<GridLayoutGroup> imageList;

        public virtual void Show(int index)
        {
            m_Index = Mathf.Clamp(index,0, imageList.Count-1);

        }

        public virtual void Next()
        {

        }

        public virtual void Prev()
        {

        }

        /// <summary>
        /// 循环模式
        /// </summary>
        public enum WrapMode
        {
            Loop,
            Once,
            Random
        }
    }
}
