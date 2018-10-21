using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ScrollRect))]
    /// <summary>
    /// 循环容器
    /// 
    /// 1.如果使用效果（如位移特效，旋转特效，缩放特效），需子物体实现IUguiChildControl接口
    /// 2.如果使用颜色改变效果，需子物体挂载UguiColorTint组件
    /// </summary>
    public class UguiWrapContent : UIBehaviour
    {
        protected readonly Vector2 contentDefaultPivot = new Vector2(0,1);

        [SerializeField] protected int minIndex = -9999;
        [SerializeField] protected int maxIndex = 9999;      
        [SerializeField] protected float spacing = 100;                                                 //元素间距
        [SerializeField] protected Axis m_StartAxis;

        protected Comparison<RectTransform> m_DepthSortComparison;
        protected ScrollRect scrollRect;
        protected Mask mask;
        protected RectTransform content;
        protected List<RectTransform> items;
        protected Vector3[] maskCorners;
        protected bool firstTime;
        protected float itemOffset;

        public Action<RectTransform, int, int> OnInitItem;                                              //子元素初始回调

        UguiWrapContent()
        {
            items = new List<RectTransform>();
            maskCorners = new Vector3[4];
        }

        protected override void Start()
        {
            Realign();
        }

        protected virtual void Update()
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

            if(mask==null)
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
            if (items==null||items.Count == 0) return;
            if (scrollRect == null || content == null || mask == null) return;

            content.pivot = contentDefaultPivot;
            content.localScale = Vector3.one;
            content.localRotation = Quaternion.identity;
            mask.rectTransform.GetWorldCorners(maskCorners);
            var maskCenter = Vector3.Lerp(maskCorners[0], maskCorners[2], 0.5f);
            var maskLocalCenter = mask.transform.InverseTransformPoint(maskCenter);
            var contentPoint = content.InverseTransformPoint(maskCenter);
            var extents = spacing * items.Count * 0.5f;
            var ext2 = extents * 2f;
            if (m_StartAxis == Axis.Vertical)
            {
                itemOffset = Application.isPlaying ?
                            minIndex * spacing - spacing * 0.5f :
                            -spacing * 0.5f;
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, spacing * (maxIndex - minIndex+1));
                content.localPosition = new Vector3(0,content.localPosition.y,0);

                //限制位置到视图内
                if (content.localPosition.y< maskLocalCenter.y)
                {
                    scrollRect.StopMovement();
                    var pos = content.localPosition;
                    pos.y = maskLocalCenter.y;
                    content.localPosition = pos;
                }
                else if (content.localPosition.y-content.rect.height> maskLocalCenter.y)
                {
                    scrollRect.StopMovement();
                    var pos = content.localPosition;
                    pos.y = maskLocalCenter.y+ content.rect.height;
                    content.localPosition = pos;
                }

                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var localPos = item.localPosition;            
                    var distance = localPos.y - contentPoint.y;
                    var isUpdateItem = false;
                    if (firstTime)
                    {
                        localPos = new Vector3(contentPoint.x, -i * spacing+ itemOffset);
                        isUpdateItem = true;
                    }
                    else if (distance < -extents)
                    {
                        localPos.y += ext2;
                        var realIndex = GetRealIndex(localPos);
                        if (minIndex <= realIndex && realIndex <= maxIndex)
                        {
                            isUpdateItem = true;
                        }
                    }
                    else if (distance > extents)
                    {
                        localPos.y -= ext2;
                        var realIndex = GetRealIndex(localPos);
                        if (minIndex <= realIndex && realIndex <= maxIndex)
                        {
                            isUpdateItem = true;
                        }
                    }
                    if (isUpdateItem)
                    {
                        item.localPosition = localPos;
                        UpdateItem(item, i);
                    }
                }
            }
            else
            {
                itemOffset = Application.isPlaying ?
                            -minIndex * spacing + spacing * 0.5f :
                            spacing * 0.5f;
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, spacing * (maxIndex - minIndex + 1));
                content.localPosition = new Vector3(content.localPosition.x, 0, 0);

                //限制位置到视图内
                if (content.localPosition.x > maskLocalCenter.x)
                {
                    scrollRect.StopMovement();
                    var pos = content.localPosition;
                    pos.x = maskLocalCenter.x;
                    content.localPosition = pos;
                }
                else if (content.localPosition.x + content.rect.width < maskLocalCenter.x)
                {
                    scrollRect.StopMovement();
                    var pos = content.localPosition;
                    pos.x = maskLocalCenter.x - content.rect.width;
                    content.localPosition = pos;
                }

                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var localPos = item.localPosition;
                    var distance = localPos.x - contentPoint.x;
                    var isUpdateItem = false;
                    if (firstTime)
                    {
                        localPos = new Vector3(i * spacing + itemOffset,contentPoint.y);
                        isUpdateItem = true;
                    }
                    else if (distance < -extents)
                    {
                        localPos.x += ext2;
                        var realIndex = GetRealIndex(localPos);
                        if (minIndex <= realIndex && realIndex <= maxIndex)
                        {
                            isUpdateItem = true;
                        }
                    }
                    else if (distance > extents)
                    {
                        localPos.x -= ext2;
                        var realIndex = GetRealIndex(localPos);
                        if (minIndex <= realIndex && realIndex <= maxIndex)
                        {
                            isUpdateItem = true;
                        }
                    }
                    if (isUpdateItem)
                    {
                        item.localPosition = localPos;
                        UpdateItem(item, i);
                    }
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

        public virtual void ForceUpdateItems()
        {
            Init();

            if (OnInitItem!=null)
            {
                for(var i=0;i<items.Count;i++)
                {
                    var realIndex = GetRealIndex(items[i].localPosition);
                    OnInitItem.Invoke(items[i], i, realIndex);
                }
            }
        }

        protected virtual int GetRealIndex(Vector3 localPosition)
        {
            var realIndex = 0;
            if (m_StartAxis == Axis.Vertical)
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
        /// 深度层级排序规则
        /// 设置此值按自定义排序
        /// </summary>
        public Comparison<RectTransform> DepthSortComparison
        {
            get
            {
                return m_DepthSortComparison;
            }
            set
            {
                m_DepthSortComparison = value;
            }
        }

        /// <summary>
        /// 轴向
        /// </summary>
        public Axis StartAxis
        {
            get
            {
                return m_StartAxis;
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