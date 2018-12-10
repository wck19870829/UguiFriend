using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 网格排序
    /// </summary>
    public class UguiGrid : GridLayoutGroup, IUguiContent
    {
        protected List<UguiObject> m_Children;
        protected Action m_OnReposition;

        protected UguiGrid()
        {
            m_Children = new List<UguiObject>();
        }

        public Action OnReposition
        {
            get
            {
                return m_OnReposition;
            }
            set
            {
                m_OnReposition = value;
            }
        }

        public virtual List<UguiObject> Children
        {
            get
            {
                return m_Children;
            }
        }

        public virtual void Set(List<UguiObjectData> dataList)
        {
            UguiObjectPool.Instance.Push(m_Children);
            m_Children = UguiObjectPool.Instance.Get(dataList);
        }

        public virtual void Reposition()
        {

        }
    }
}