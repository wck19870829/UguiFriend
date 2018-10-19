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
    /// </summary>
    public class UguiWrapContent : UIBehaviour
    {
        protected readonly Vector2 contentDefaultPivot = new Vector2(0,1);

        [SerializeField] protected int minIndex = -9999;
        [SerializeField] protected int maxIndex = 9999;
        [SerializeField] protected bool keepCenterFront=true;                                           //保持中间元素置顶显示        
        [SerializeField] protected float spacing = 100;                                                 //元素间距
        [SerializeField] protected Axis axis;

        [Header("- Position offset effect")]
        [SerializeField] protected bool usePositionEffect;                                              //位移效果参数
        [SerializeField] protected bool isMirrorPositionEffect = true;
        [SerializeField] protected AnimationCurve posOffsetCurve = AnimationCurve.Linear(0, 0, 1, 0);    
        [SerializeField] protected Vector3 positionFrom;
        [SerializeField] protected Vector3 positionTo;

        [Header("- Rotation effect")]
        [SerializeField] protected bool useRotationEffect;                                              //旋转效果参数
        [SerializeField] protected bool isMirrorRotationEffect = true;
        [SerializeField] protected AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 0);    
        [SerializeField] protected Vector3 angleFrom;
        [SerializeField] protected Vector3 angleTo;

        [Header("- Scale effect")]
        [SerializeField] protected bool useScaleEffect;                                                 //缩放效果参数
        [SerializeField] protected bool isMirrorScaleEffect = true;
        [SerializeField] protected AnimationCurve scaleCurve = AnimationCurve.Linear(0, 1, 1, 1);       
        [SerializeField] protected Vector3 scaleFrom=Vector3.one;
        [SerializeField] protected Vector3 scaleTo = Vector3.one;

        [Header("- Color tint effect")]
        [SerializeField] protected bool useColorTintEffect;                                             //颜色效果参数
        [SerializeField] protected bool isMirrorColorTintEffect = true;
        [SerializeField] protected AnimationCurve colorCurve= AnimationCurve.Linear(0, 0, 1, 1);        
        [SerializeField] protected Color colorFrom = Color.white;
        [SerializeField] protected Color colorTo = Color.white;

        protected Comparison<RectTransform> m_DepthSortComparison;
        protected ScrollRect scrollRect;
        protected Mask mask;
        protected RectTransform content;
        protected List<RectTransform> items;
        protected Vector3[] maskCorners;
        protected bool firstTime;
        protected float itemOffset;
        protected List<RectTransform> itemsTemp;
        protected Dictionary<RectTransform, Vector3> itemOriginDict;                                    //子元素原点

        public Action<RectTransform, int, int> OnInitItem;                                              //子元素初始回调

        UguiWrapContent()
        {
            items = new List<RectTransform>();
            maskCorners = new Vector3[4];
            itemsTemp = new List<RectTransform>();
            itemOriginDict = new Dictionary<RectTransform, Vector3>();
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
            if (items==null||items.Count == 0) return;
            if (scrollRect == null || content == null || mask == null) return;

            content.pivot = contentDefaultPivot;
            mask.rectTransform.GetWorldCorners(maskCorners);
            var maskCenter = Vector3.Lerp(maskCorners[0], maskCorners[2], 0.5f);
            var maskLocalCenter = mask.transform.InverseTransformPoint(maskCenter);
            var contentPoint = content.InverseTransformPoint(maskCenter);
            var extents = spacing * items.Count * 0.5f;
            var ext2 = extents * 2f;
            if (axis == Axis.Vertical)
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
                    var localPos = itemOriginDict.ContainsKey(item)?
                                    itemOriginDict[item]:
                                    item.localPosition;
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
                        if (!itemOriginDict.ContainsKey(item))
                            itemOriginDict.Add(item, localPos);
                        itemOriginDict[item] = localPos;

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
                    var localPos = itemOriginDict.ContainsKey(item) ?
                                    itemOriginDict[item] :
                                    item.localPosition;
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
                        if (!itemOriginDict.ContainsKey(item))
                            itemOriginDict.Add(item,localPos);
                        itemOriginDict[item] = localPos;

                        item.localPosition = localPos;
                        UpdateItem(item, i);
                    }
                }
            }

            var maskRectRelativeContent = UguiTools.RectGlobal2Local(content,mask.rectTransform.rect);

            ApplyPositionEffect(maskRectRelativeContent);
            ApplyRotationEffect(maskRectRelativeContent);
            ApplyScaleEffect(maskRectRelativeContent);

            ApplyDepthSort(maskRectRelativeContent);
        }

        /// <summary>
        /// 位移效果
        /// </summary>
        /// <param name="maskRectRelativeContent"></param>
        protected virtual void ApplyPositionEffect(Rect maskRectRelativeContent)
        {
            if (usePositionEffect)
            {
                foreach (var item in items)
                {
                    var weight = GetEffectWeight(item, isMirrorPositionEffect, maskRectRelativeContent);
                    var offset = Vector3.Lerp(positionFrom, positionTo, posOffsetCurve.Evaluate(weight));
                    item.localPosition = itemOriginDict[item] + offset;
                }
            }
        }

        /// <summary>
        /// 旋转效果
        /// </summary>
        /// <param name="maskRectRelativeContent"></param>
        protected virtual void ApplyRotationEffect(Rect maskRectRelativeContent)
        {
            if (useRotationEffect)
            {
                foreach (var item in items)
                {
                    var weight = GetEffectWeight(item, isMirrorRotationEffect, maskRectRelativeContent);
                    var angle = Vector3.Lerp(angleFrom, angleTo, rotationCurve.Evaluate(weight));
                    item.localEulerAngles = angle;
                }
            }
        }

        /// <summary>
        /// 缩放效果
        /// </summary>
        /// <param name="maskRectRelativeContent"></param>
        protected virtual void ApplyScaleEffect(Rect maskRectRelativeContent)
        {
            if (useScaleEffect)
            {
                foreach (var item in items)
                {
                    var weight = GetEffectWeight(item, isMirrorScaleEffect, maskRectRelativeContent);
                    var scale = Vector3.Lerp(scaleFrom, scaleTo, scaleCurve.Evaluate(weight));
                    item.localScale = scale;
                }
            }
        }

        /// <summary>
        /// 颜色控制效果
        /// </summary>
        /// <param name="maskRectRelativeContent"></param>
        protected virtual void ApplyColorTintEffect(Rect maskRectRelativeContent)
        {
            if (useColorTintEffect)
            {
                foreach (var item in items)
                {
                    var weight = GetEffectWeight(item,isMirrorColorTintEffect, maskRectRelativeContent);
                    var colorTint = item.GetComponent<UguiColorTint>();
                    if (colorTint != null)
                    {
                        var col = Color.Lerp(colorFrom, colorTo, colorCurve.Evaluate(weight));
                        colorTint.color = col;
                    }
                }
            }
        }

        /// <summary>
        /// 层级排序
        /// </summary>
        /// <param name="contentPoint"></param>
        protected virtual void ApplyDepthSort(Rect maskRectRelativeContent)
        {
            if (m_DepthSortComparison != null)
            {
                //优先自定义排序
                itemsTemp.Sort(m_DepthSortComparison);
                foreach (var item in itemsTemp)
                {
                    item.SetAsFirstSibling();
                }
            }
            else if (keepCenterFront)
            {
                //默认排序规则：距离中点越近，层级越高
                itemsTemp.Sort((a, b) =>
                {
                    var weightA = GetEffectWeight(a,true,maskRectRelativeContent);
                    var weightB = GetEffectWeight(b, true, maskRectRelativeContent);
                    if (weightA == weightB) return 0;
                    return weightA > weightB ? 1 : -1;
                });
                foreach (var item in itemsTemp)
                {
                    item.SetAsFirstSibling();
                }
            }
        }

        /// <summary>
        /// 获取权重
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isMirror"></param>
        /// <param name="maskRectRelativeContent"></param>
        /// <returns></returns>
        protected float GetEffectWeight(RectTransform item,bool isMirror,Rect maskRectRelativeContent)
        {
            var weight = 0f;
            if (isMirror)
            {
                if (axis == Axis.Horizontal)
                {
                    weight = Mathf.Abs(item.localPosition.x - maskRectRelativeContent.center.x)/(maskRectRelativeContent.width*0.5f);
                }
                else
                {
                    weight = Mathf.Abs(item.localPosition.y - maskRectRelativeContent.center.y) / (maskRectRelativeContent.height * 0.5f);
                }
            }
            else
            {
                if (axis == Axis.Horizontal)
                {
                    weight=(item.localPosition.x - maskRectRelativeContent.xMin) / maskRectRelativeContent.width;
                }
                else
                {
                    weight = (item.localPosition.y - maskRectRelativeContent.yMax) / maskRectRelativeContent.height;
                }
            }

            return weight;
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
        public enum Axis
        {
            Horizontal,
            Vertical
        }
    }
}