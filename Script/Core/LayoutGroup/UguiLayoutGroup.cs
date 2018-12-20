using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 布局容器基类
    /// </summary>
    public abstract class UguiLayoutGroup : LayoutGroup
    {
        [SerializeField] protected UguiObject m_PrefabSource;
        [SerializeField] protected bool m_Optimize;                     //是否使用优化模式,如果使用优化模式,会复用容器内元素,同时一些功能会受到限制
        [SerializeField] protected Rect m_SafeViewPortRect;             //当使用优化创建时安全框,安全框内的物体才会被创建显示
        protected List<UguiObject> m_Children;

        public Action OnReposition;

        protected UguiLayoutGroup()
        {
            m_Children = new List<UguiObject>();
            m_SafeViewPortRect = Rect.MinMaxRect(-0.2f, -0.2f, 1.2f, 1.2f);
        }

        public virtual List<UguiObject> Children
        {
            get
            {
                if (m_Optimize)
                {
                    throw new Exception("Please do not get children when optimize is ture.");
                }

                return m_Children;
            }
        }

        public virtual void Set(List<UguiObjectData> dataList)
        {
            if (!m_Optimize)
            {
                UguiTools.SetChildrenDatas(m_Children, transform, dataList, m_PrefabSource);
            }
        }

        public virtual void Reposition()
        {

        }
    }
}