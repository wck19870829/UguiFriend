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
    /// 层级排序
    /// </summary>
    public abstract class UguiLayer : UIBehaviour
    {
        public const int layerInterval = 10000;
        public const int orderMax = layerInterval - 1;
        static readonly List<UguiLayer> s_LayerList;
        static readonly Dictionary<int, int> s_OrderDict;
        static readonly Dictionary<int, HashSet<int>> customOrderSet;
        static bool s_Dirty;

        [SerializeField] protected int m_Layer;                     //层类型,范围[0...31]
        [SerializeField] protected int m_Order;                     //相对排序值
        [SerializeField] protected int m_GlobalOrder;               //绝对排序值
        [SerializeField] protected bool m_AutoSort=true;            //是否自动排序

        static UguiLayer()
        {
            s_LayerList = new List<UguiLayer>();
            s_OrderDict = new Dictionary<int, int>();
            customOrderSet = new Dictionary<int, HashSet<int>>();
            for (var i=0;i<32;i++)
            {
                customOrderSet.Add(i,new HashSet<int>());
                s_OrderDict.Add(i,0);
            }

            Canvas.willRenderCanvases += OnWillRenderCanvases;
        }

        static void OnWillRenderCanvases()
        {
            if (s_Dirty)
            {
                s_Dirty = false;
                SortingImmediate();
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
        /// 渲染层
        /// </summary>
        public int Layer
        {
            get
            {
                return m_Layer;
            }
            set
            {
                if (value < 0 || value >= 32)
                {
                    throw new Exception("层范围为[0...31]");
                }

                m_Layer = value;
                SetDirty();
            }
        }

        /// <summary>
        /// 同一层级下排序值
        /// </summary>
        public int Order
        {
            get
            {
                return m_Order;
            }
            set
            {
                if(value<0||value> orderMax)
                {
                    throw new Exception("取值范围为[0..."+ orderMax+"]");
                }

                m_Order = value;
                SetDirty();
            }
        }

        /// <summary>
        /// 全局排序值
        /// </summary>
        public int GlobalOrder
        {
            get
            {
                return m_GlobalOrder;
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

        protected abstract int SetRenderOrder();

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
            for (var i = 0; i < 32; i++)
            {
                s_OrderDict[i] = 0;
            }
            foreach (var item in customOrderSet.Values)
            {
                item.Clear();
            }
            foreach (var item in s_LayerList)
            {
                if (!item.m_AutoSort)
                {
                    if (!customOrderSet[item.m_Layer].Contains(item.m_Order))
                    {
                        customOrderSet[item.m_Layer].Add(item.m_Order);
                    }
                }
            }

            foreach (var item in s_LayerList)
            {
                var layer = item.m_Layer;
                var order = s_OrderDict[layer];
                var orderSet = customOrderSet[layer];
                while (orderSet.Contains(order))
                {
                    order++;
                }
                if (item.m_AutoSort)
                {
                    item.m_Order = order;
                    s_OrderDict[layer] = order + 1;
                }
                item.m_GlobalOrder = layer * layerInterval + item.m_Order;
            }

            //排序
            s_LayerList.Sort((a, b) => {
                if (a.m_GlobalOrder == b.m_GlobalOrder) return 0;

                return a.m_GlobalOrder > b.m_GlobalOrder ? 1 : -1;
            });

            var renderOrder=0;
            foreach (var item in s_LayerList)
            {
                var num=item.SetRenderOrder();
                renderOrder += num;
            }
        }
    }

    public abstract class UguiLayer<T> : UguiLayer
        where T:Component
    {
        protected List<T> m_Children;

        protected UguiLayer()
        {
            m_Children = new List<T>();
        }

        protected override int SetRenderOrder()
        {
            m_Children.Clear();
            var children = GetComponentsInChildren<T>(true);
            foreach (var child in children)
            {
                if (child.GetComponentInParent<UguiLayer>() == this)
                {
                    m_Children.Add(child);
                }
            }

            return m_Children.Count;
        }
    }
}