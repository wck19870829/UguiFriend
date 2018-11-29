using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 层级排序
    /// </summary>
    public class UguiLayer : MonoBehaviour
    {
        static bool s_Dirty;

        [SerializeField] protected int m_Layer;                     //层类型,范围[0...31]
        [SerializeField] protected int m_Order;                     //相对排序值
        [SerializeField] protected int m_GlobalOrder;               //绝对排序值
        [SerializeField] protected bool m_AutoSort=true;            //是否自动排序

        protected virtual void OnEnabled()
        {
            SetDirty();
        }

        protected virtual void LateUpdate()
        {
            OnLayerChange();
        }

        static void OnLayerChange()
        {
            if (s_Dirty)
            {
                s_Dirty = false;
            }
        }

        /// <summary>
        /// 重新排序
        /// </summary>
        public static void SetDirty()
        {
            s_Dirty = true;
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
    }
}