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
        protected readonly Vector2 contentDefaultPivot = new Vector2(0,1);

        [SerializeField] protected int minIndex = -9999;
        [SerializeField] protected int maxIndex = 9999;
        [SerializeField] protected bool keepCenterFront=true;                                           //保持中间元素置顶显示        
        [SerializeField] protected float spacing = 100;                                                 //元素间距
        [SerializeField] protected Axis axis;

        [Header("-Rotation control")]
        [SerializeField] protected AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 0);    //旋转曲线，根据元素到中心点位置旋转
        [SerializeField] protected Vector3 sideAngle;
        [SerializeField] protected Vector3 centerAngle;

        [Header("-Scale control")]
        [SerializeField] protected AnimationCurve scaleCurve = AnimationCurve.Linear(0, 1, 1, 1);       //缩放曲线，根据元素到中心点位置缩放

        protected Comparison<RectTransform> m_SortComparison;
        protected ScrollRect scrollRect;
        protected Mask mask;
        protected RectTransform content;
        protected List<RectTransform> items;
        protected Vector3[] maskCorners;
        protected bool firstTime;
        protected float itemOffset;
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
            content = scrollRect.content;
            if (content == null)
                throw new Exception("Scroll rect content is null!");
            if(scrollRect.movementType == ScrollRect.MovementType.Unrestricted)
                scrollRect.movementType = ScrollRect.MovementType.Elastic;

            mask = scrollRect.GetComponentInChildren<Mask>();
            if (mask == null)
                throw new Exception("Mask is null!");
        }

        /// <summary>
        /// 重新排列
        /// </summary>
        public void Realign()
        {
            Init();

            firstTime = true;
            items.Clear();
            for (var i = 0; i < content.childCount; i++)
            {
                items.Add(content.GetChild(i) as RectTransform);
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
            if (scrollRect == null || content == null || mask == null) return;

            content.pivot = contentDefaultPivot;
            mask.rectTransform.GetWorldCorners(maskCorners);
            var maskCenter = Vector3.Lerp(maskCorners[0], maskCorners[2], 0.5f);
            var contentPoint = content.InverseTransformPoint(maskCenter);
            var extents = spacing * items.Count * 0.5f;
            var ext2 = extents * 2f;
            if (axis == Axis.Vertical)
            {
                itemOffset = Application.isPlaying ? minIndex * spacing - spacing * 0.5f : -spacing * 0.5f;
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, spacing * (maxIndex - minIndex+1));
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
                        var realIndex = GetRealIndex(localPos);
                        if (minIndex <= realIndex && realIndex <= maxIndex)
                        {
                            item.localPosition = localPos;
                            UpdateItem(item, i);
                        }
                    }
                    else if (distance > extents)
                    {
                        localPos.y -= ext2;
                        var realIndex = GetRealIndex(localPos);
                        if (minIndex <= realIndex && realIndex <= maxIndex)
                        {
                            item.localPosition = localPos;
                            UpdateItem(item, i);
                        }
                    }
                }
            }
            else
            {
                itemOffset = Application.isPlaying ? -minIndex * spacing + spacing * 0.5f : spacing * 0.5f;
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, spacing * (maxIndex - minIndex + 1));
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var localPos = item.localPosition;
                    var distance = localPos.x - contentPoint.x;
                    if (firstTime)
                    {
                        localPos = new Vector3(i * spacing + itemOffset,contentPoint.y);
                        item.localPosition = localPos;
                        UpdateItem(item, i);
                    }
                    else if (distance < -extents)
                    {
                        localPos.x += ext2;
                        var realIndex = GetRealIndex(localPos);
                        if (minIndex <= realIndex && realIndex <= maxIndex)
                        {
                            item.localPosition = localPos;
                            UpdateItem(item, i);
                        }
                    }
                    else if (distance > extents)
                    {
                        localPos.x -= ext2;
                        var realIndex = GetRealIndex(localPos);
                        if (minIndex <= realIndex && realIndex <= maxIndex)
                        {
                            item.localPosition = localPos;
                            UpdateItem(item, i);
                        }
                    }
                }
            }

            //控制子元素
            foreach (var item in items)
            {
                //缩放
                var dist = Vector3.Distance(item.localPosition, contentPoint);
                var time = 1 - Mathf.Clamp01(dist / extents);
                var scale = scaleCurve.Evaluate(time);
                item.localScale = new Vector3(scale, scale, scale);

                //旋转
                var angle = Vector3.Lerp(centerAngle,sideAngle,rotationCurve.Evaluate(1 - time));
                switch (axis)
                {
                    case Axis.Horizontal:
                        angle *= Mathf.Sign(item.localPosition.x - contentPoint.x);
                        item.localEulerAngles = angle;
                        break;

                    case Axis.Vertical:
                        angle *= Mathf.Sign(item.localPosition.y - contentPoint.y);
                        item.localEulerAngles = angle;
                        break;
                }
            }

            //中间元素置顶
            if (keepCenterFront && Application.isPlaying)
            {
                itemsTemp.Sort((a, b) =>
                {
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
                var realIndex = GetRealIndex(item.localPosition);
                OnInitItem.Invoke(item, index, realIndex);
            }
        }

        protected virtual int GetRealIndex(Vector3 localPosition)
        {
            var realIndex = 0;
            if (axis==Axis.Vertical)
            {
                realIndex = -Mathf.RoundToInt((localPosition.y - itemOffset) / spacing);
            }
            else
            {
                realIndex = Mathf.RoundToInt((localPosition.x - itemOffset) / spacing);
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