using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    [DisallowMultipleComponent]
    /// <summary>
    /// 层级排序基类
    /// </summary>
    public abstract class UguiSortingLayer : UIBehaviour
    {
        public const int layerInterval = 1000;
        public const int sortingOrderMin = 0;
        public const int sortingOrderMax = layerInterval - 1;
        protected static readonly List<UguiSortingLayer> s_LayerList;
        protected static int s_GlobalSortngLayer;
        protected static bool s_Dirty;

        [SerializeField] protected int m_SortingLayerID;            //排序层id
        [SerializeField] protected int m_SortingOrder;              //相对排序值
        [SerializeField] protected bool m_AutoSort = true;          //是否自动排序

        static UguiSortingLayer()
        {
            s_LayerList = new List<UguiSortingLayer>();

            Canvas.willRenderCanvases += OnWillRenderCanvases;
        }

        static void OnWillRenderCanvases()
        {
            if (Application.isPlaying)
            {
                if (s_Dirty)
                {
                    s_Dirty = false;
                    SortingImmediate();
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            s_LayerList.Add(this);
            SetDirty();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            s_LayerList.Remove(this);
        }

        /// <summary>
        /// 排序层级
        /// </summary>
        public int SortingLayerID
        {
            get
            {
                return m_SortingLayerID;
            }
            set
            {
                if (!SortingLayer.IsValid(value))
                {
                    Debug.LogErrorFormat("无效的值:{0}", value);
                    return;
                }

                m_SortingLayerID = value;
                SetDirty();
            }
        }

        /// <summary>
        /// 排序层级名称
        /// </summary>
        public string SortingLayerName
        {
            get
            {
                return SortingLayer.IDToName(m_SortingLayerID);
            }
            set
            {
                var newID=SortingLayer.NameToID(value);
                if (!SortingLayer.IsValid(newID))
                {
                    Debug.LogErrorFormat("无效的名称:{0}",value);
                    return;
                }

                m_SortingLayerID = newID;
                SetDirty();
            }
        }

        /// <summary>
        /// 同一层级下排序值
        /// </summary>
        public int SortingOrder
        {
            get
            {
                return m_SortingOrder;
            }
            set
            {
                if(value< sortingOrderMin || value> sortingOrderMax)
                {
                    throw new Exception("取值范围为["+sortingOrderMin+"..." + sortingOrderMax+"]");
                }

                m_SortingOrder = value;
                m_AutoSort = false;
                SetDirty();
            }
        }

        /// <summary>
        /// 自动排序
        /// </summary>
        public bool AutoSort
        {
            get
            {
                return m_AutoSort;
            }
            set
            {
                m_AutoSort = value;
                SetDirty();
            }
        }

        protected abstract void SetRenderOrder();

        /// <summary>
        /// 标记改变延迟排序
        /// </summary>
        public static void SetDirty()
        {
            s_Dirty = true;
        }

        /// <summary>
        /// 立即排序
        /// </summary>
        public static void SortingImmediate()
        {
            var orderDict = new Dictionary<int, List<UguiSortingLayer>>();
            var customOrderSet = new Dictionary<int, HashSet<int>>();
            foreach (var layer in SortingLayer.layers)
            {
                customOrderSet.Add(layer.id,new HashSet<int>());
                orderDict.Add(layer.id,new List<UguiSortingLayer>());
            }
            foreach (var layer in s_LayerList)
            {
                var sortingLayerID = layer.m_SortingLayerID;
                if (!layer.m_AutoSort)
                {
                    if (!customOrderSet[sortingLayerID].Contains(layer.m_SortingOrder))
                    {
                        customOrderSet[sortingLayerID].Add(layer.m_SortingOrder);
                    }
                }

                orderDict[sortingLayerID].Add(layer);
            }

            foreach (var item in orderDict)
            {
                var order = 0;
                s_GlobalSortngLayer = 0;
                var sortingLayerID = item.Key;
                var layerList = item.Value;
                foreach (var layer in layerList)
                {
                    while (customOrderSet[sortingLayerID].Contains(order))
                    {
                        order++;
                    }
                    if (layer.m_AutoSort)
                    {
                        layer.m_SortingOrder = order;
                        order++;
                    }
                }
                layerList.Sort((a,b)=> {
                    return a.m_SortingOrder - b.m_SortingOrder;
                });
                foreach (var layer in layerList)
                {
                    layer.SetRenderOrder();
                }
            }
        }
    }

    /// <summary>
    /// 层级排序泛型基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UguiSortingLayer<T> : UguiSortingLayer
        where T:Component
    {
        protected List<T> m_Children;

        protected UguiSortingLayer()
        {
            m_Children = new List<T>();
        }

        protected override void SetRenderOrder()
        {
            m_Children.Clear();
            var children = GetComponentsInChildren<T>(true);
            foreach (var child in children)
            {
                if (child.GetComponentInParent<UguiSortingLayer>() == this)
                {
                    m_Children.Add(child);
                }
            }

            Sort(m_Children);
        }

        protected abstract void Sort(List<T> children);
    }
}