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
        [SerializeField] protected int minIndex = -5;
        [SerializeField] protected int maxIndex = 5;
        [SerializeField] protected bool keepCenterFront=true;                                           //保持中间元素置顶显示
        [SerializeField] protected AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 0);    //旋转曲线，根据元素到中心点位置旋转
        [SerializeField] protected AnimationCurve scaleCurve = AnimationCurve.Linear(0, 1, 1, 1);       //缩放曲线，根据元素到中心点位置缩放      
        [SerializeField] protected float spacing = 100;                                                 //元素间距
        [SerializeField] protected Axis axis;
        protected Comparison<RectTransform> m_SortComparison;
        protected ScrollRect scrollRect;
        protected Mask mask;
        protected List<RectTransform> items;
        protected Vector3[] maskCorners;
        protected bool firstTime;
        List<Transform> itemsTemp;

        public Action<RectTransform, int, int> OnInitItem;                                  //子元素初始回调

        UguiWrapContent()
        {
            items = new List<RectTransform>();
            maskCorners = new Vector3[4];
            itemsTemp = new List<Transform>();
        }

        protected virtual void Start()
        {
            Realign();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                Realign();
            }
            WrapContent();
        }

        void Init()
        {
            if (scrollRect == null)
                scrollRect = GetComponentInChildren<ScrollRect>();
            if (scrollRect == null)
                throw new Exception("Scroll rect is null!");
            if (scrollRect.content == null)
                throw new Exception("Scroll rect content is null!");
            if(scrollRect.movementType == ScrollRect.MovementType.Unrestricted)
                scrollRect.movementType = ScrollRect.MovementType.Elastic;

            mask = scrollRect.GetComponentInChildren<Mask>();
            if (mask == null)
                throw new Exception("Mask is null!");
            //scrollRect.content.pivot = new Vector2(0,1);
        }

        /// <summary>
        /// 重新排列
        /// </summary>
        public void Realign()
        {
            Init();

            firstTime = true;
            items.Clear();
            for (var i = 0; i < scrollRect.content.childCount; i++)
            {
                items.Add(scrollRect.content.GetChild(i) as RectTransform);
            }
            if (m_SortComparison != null)
            {
                items.Sort(m_SortComparison);
            }
            itemsTemp.Clear();
            foreach (var item in items)
            {
                itemsTemp.Add(item);
            }

            WrapContent();
            if (Application.isPlaying)
            {
                firstTime = false;
            }
        }

        /// <summary>
        /// 计算元素循环
        /// </summary>
        protected virtual void WrapContent()
        {
            if (items.Count == 0) return;
            if (scrollRect == null || scrollRect.content == null || mask == null) return;

            mask.rectTransform.GetWorldCorners(maskCorners);
            var maskCenter = Vector3.Lerp(maskCorners[0], maskCorners[2], 0.5f);
            var extents = spacing * items.Count * 0.5f;
            var ext2 = extents * 2f;
            if (axis == Axis.Vertical)
            {
                var maskHeight = Mathf.Abs(maskCorners[0].y - maskCorners[1].y);
                var itemOffset = Application.isPlaying?spacing * 0.5f+minIndex*spacing: -spacing * 0.5f;
                scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, spacing * (maxIndex - minIndex));
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var localPos = item.localPosition;
                    var distance = localPos.y - contentPoint.y;
                    if (firstTime)
                    {
                        localPos = new Vector3(contentPoint.x, -i * spacing+ itemOffset);
                        item.localPosition = localPos;
                        UpdateItem(item, i);
                    }
                    else if (distance < -extents)
                    {
                        localPos.y += ext2;
                        var realIndex = GetRealIndex(item);
                        if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                        {
                            item.localPosition = localPos;
                            UpdateItem(item, i);
                        }
                    }
                    else if (distance > extents)
                    {
                        localPos.y -= ext2;
                        var realIndex = GetRealIndex(item);
                        if (minIndex == maxIndex || (minIndex <= realIndex && realIndex <= maxIndex))
                        {
                            item.localPosition = localPos;
                            UpdateItem(item, i);
                        }
                    }
                }
            }
            else
            {
                scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, spacing * (maxIndex - minIndex)+ ext2);
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var localPos = item.localPosition;
                    var distance = localPos.x - contentPoint.x;
                    if (firstTime)
                    {
                        localPos = new Vector3(i * spacing + spacing * 0.5f, contentPoint.y);
                        item.localPosition = localPos;
                        UpdateItem(item, i);
                    }
                    else if (distance < -extents)
                    {
                        localPos.x += ext2;
                        item.localPosition = localPos;
                        UpdateItem(item, i);
                    }
                    else if (distance > extents)
                    {
                        localPos.x -= ext2;
                        item.localPosition = localPos;
                        UpdateItem(item, i);
                    }
                }
            }

            //控制子元素
            foreach (var item in items)
            {
                ////缩放
                //var dist = Vector3.Distance(item.localPosition,localCenter);
                //var time = 1-Mathf.Clamp01(dist / extents);
                //var scale = scaleCurve.Evaluate(time);
                //item.localScale = new Vector3(scale,scale,scale);

                ////旋转
                //var rotation = rotationCurve.Evaluate(1-time);
                //rotation *= 90;
                //switch (axis)
                //{
                //    case Axis.Horizontal:
                //        rotation *= Mathf.Sign(item.localPosition.x-localCenter.x);
                //        item.localEulerAngles = new Vector3(0, rotation, 0);
                //        break;

                //    case Axis.Vertical:
                //        rotation *= Mathf.Sign(item.localPosition.y - localCenter.y);
                //        item.localEulerAngles = new Vector3(rotation, 0, 0);
                //        break;
                //}
            }

            //中间元素置顶
            if (keepCenterFront&&Application.isPlaying)
            {
                itemsTemp.Sort((a, b) => {
                    var distA = Vector3.Distance(maskCenter, a.position);
                    var distB = Vector3.Distance(maskCenter, b.position);
                    if (distA == distB) return 0;
                    return distA > distB ? 1 : -1;
                });
                foreach (var item in itemsTemp)
                {
                    item.SetAsFirstSibling();
                }
            }
        }

        /// <summary>
        /// 更新子元素
        /// 当子元素超过边缘临界，循环移动到另外一侧时调用
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        protected virtual void UpdateItem(RectTransform item, int index)
        {
            if (OnInitItem != null)
            {
                var realIndex = GetRealIndex(item);
                OnInitItem.Invoke(item, index, realIndex);
            }
        }

        protected virtual int GetRealIndex(RectTransform item)
        {
            var realIndex = 0;
            if (axis==Axis.Vertical)
            {
                var itemOffset = Application.isPlaying ? spacing * 0.5f + minIndex * spacing : -spacing * 0.5f;
                realIndex = -Mathf.CeilToInt((item.localPosition.y-itemOffset) / spacing);
            }
            else
            {
                var itemOffset = mask.rectTransform.rect.width * 0.5f - spacing * 0.5f;
                realIndex = Mathf.CeilToInt((item.localPosition.x - itemOffset) / spacing);
            }

            return realIndex;
        }

        /// <summary>
        /// 排序规则
        /// 设置此值按自定义排序
        /// </summary>
        public Comparison<RectTransform> SortComparison
        {
            get
            {
                return m_SortComparison;
            }
            set
            {
                m_SortComparison = value;
                Realign();
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