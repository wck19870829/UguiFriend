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
        [SerializeField] protected Color displayColor = Color.white;    //显示颜色
        [SerializeField] protected Color hiddenColor = Color.clear;     //隐藏颜色
        [SerializeField] protected float interval=5;                    //每张图片显示时间
        [SerializeField] protected float fadeTime = 0.2f;               //淡入淡出切换时间
        [SerializeField] protected float delay;
        [SerializeField] protected RectTransform content;
        [SerializeField] protected WrapMode m_WrapMode;
        protected int m_Index;
        protected List<UguiTweenColorTint> tweenColorTintList;
        protected Comparison<UguiTweenColorTint> m_SortComparison;

        protected UguiCrossfadeEffect()
        {
            tweenColorTintList = new List<UguiTweenColorTint>();
        }

        protected override void Awake()
        {
            base.Awake();

            if (content == null)
                throw new Exception("Content is null.");
        }

        protected override void OnValidate()
        {
            interval = Mathf.Max(interval,0.01f);
            fadeTime = interval * 0.5f;
            delay = Mathf.Max(delay,0);
        }

        public virtual void Rebuild()
        {
            if (content == null) return;

            tweenColorTintList.Clear();
            foreach(Transform child in content)
            {
                var colorTint=child.GetComponent<UguiTweenColorTint>();
                if (colorTint!=null)
                {
                    tweenColorTintList.Add(colorTint);
                }
            }
            if (m_SortComparison != null)
            {
                tweenColorTintList.Sort(m_SortComparison);
            }
            foreach (var item in tweenColorTintList)
            {
                item.transform.SetAsFirstSibling();
            }
        }

        protected virtual void Show(int index)
        {
            if (m_Index == index) return;
            if (index < 0 || index >= tweenColorTintList.Count) return;

            tweenColorTintList[m_Index].Play(displayColor, hiddenColor, UguiTween.Direction.Forward, 0, fadeTime);
            tweenColorTintList[m_Index].CurrentPlayStyle = UguiTween.PlayStyle.Once;
            m_Index = index;

            CancelInvoke();
            Invoke("ShowNew", delay);
        }

        protected virtual void ShowNew()
        {
            tweenColorTintList[m_Index].transform.SetAsFirstSibling();
            tweenColorTintList[m_Index].Play(hiddenColor, displayColor, UguiTween.Direction.Forward, 0, fadeTime);
            tweenColorTintList[m_Index].CurrentPlayStyle = UguiTween.PlayStyle.Once;
        }

        public virtual void Next()
        {
            switch (m_WrapMode)
            {
                case WrapMode.Loop:

                    break;

                case WrapMode.Once:

                    break;

                case WrapMode.Random:

                    break;
            }
        }

        public virtual void Prev()
        {
            switch (m_WrapMode)
            {
                case WrapMode.Loop:

                    break;

                case WrapMode.Once:

                    break;

                case WrapMode.Random:

                    break;
            }
        }

        /// <summary>
        /// 自定义排序规则
        /// 设置此值按自定义排序
        /// </summary>
        public Comparison<UguiTweenColorTint> SortComparison
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
