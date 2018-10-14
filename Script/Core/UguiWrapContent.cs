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
        [SerializeField] protected int minIndex = -999999;
        [SerializeField] protected int maxIndex = 999999;
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
            //scrollRect.movementType = ScrollRect.MovementType.Unrestricted;

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
                        item.localPosition = localPos;
                        UpdateItem(item, i);
                    }
                    else if (distance < -extents)
                    {
                        localPos.y += ext2;
                        item.localPosition = localPos;
                        UpdateItem(item, i);
                    }
                    else if (distance > extents)
                    {
                        localPos.y -= ext2;
                        item.localPosition = localPos;
                        UpdateItem(item, i);
                    }
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

            //中间元素置顶
            if (keepCenterFront&&Application.isPlaying)
            {
                itemsTemp.Sort((a, b) => {
                    var distA = Vector3.Distance(localCenter, a.localPosition);
                    var distB = Vector3.Distance(localCenter, b.localPosition);
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
                int realIndex = (axis == Axis.Vertical) ?
                    -Mathf.CeilToInt(item.localPosition.y / spacing) :
                    Mathf.CeilToInt(item.localPosition.x / spacing);
                OnInitItem.Invoke(item, index, realIndex);
            }
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