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
    public class UguiCrossfadeEffect:UIBehaviour
    {
        [SerializeField] protected float interval=5;                //每张图片显示时间
        [SerializeField] protected RectTransform content;
        [SerializeField] protected WrapMode wrapMode;
        protected int m_Index;
        protected List<UguiColorTint> colorTintList;
        protected Comparison<UguiColorTint> m_SortComparison;

        protected UguiCrossfadeEffect()
        {
            colorTintList = new List<UguiColorTint>();
        }

        protected override void Awake()
        {
            base.Awake();

            if (content == null)
                throw new Exception("Content is null.");
        }

        public virtual void Rebuild()
        {
            if (content == null) return;

            colorTintList.Clear();
            foreach(Transform child in content)
            {
                var colorTint=child.GetComponent<UguiColorTint>();
                if (colorTint!=null)
                {
                    colorTintList.Add(colorTint);
                }
            }
            if (m_SortComparison != null)
            {
                colorTintList.Sort(m_SortComparison);
            }
        }

        public virtual void Show(int index)
        {
            m_Index = Mathf.Clamp(index,0, colorTintList.Count-1);
            colorTintList[m_Index].transform.SetAsFirstSibling();
            colorTintList[m_Index].Color = Color.white;
        }

        public virtual void Next()
        {
            Show(m_Index+1);
        }

        public virtual void Prev()
        {
            Show(m_Index - 1);
        }

        /// <summary>
        /// 自定义排序规则
        /// 设置此值按自定义排序
        /// </summary>
        public Comparison<UguiColorTint> SortComparison
        {
            get
            {
                return m_SortComparison;
            }
            set
            {
                m_SortComparison = value;
                Rebuild();
            }
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
