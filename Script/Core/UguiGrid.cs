﻿using UnityEngine;
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
        [SerializeField]protected UguiObject m_PrefabSourec;
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
            UguiTools.DestroyChildren(gameObject);
            UguiObjectPool.Instance.Get(m_Children,dataList, transform);
        }

        public void Set(List<UguiObjectData> dataList, UguiObject prefabSource)
        {
            if (prefabSource == null)
                throw new Exception("Prefab is null.");

            var prefabType = prefabSource.GetType();
            UguiObjectPool.Instance.Push(m_Children);
            UguiTools.DestroyChildren(gameObject);
            UguiObjectPool.Instance.Get(m_Children, dataList, prefabSource, transform);
        }

        public virtual void Reposition()
        {

        }
    }
}