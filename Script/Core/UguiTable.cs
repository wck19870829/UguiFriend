using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    [ExecuteInEditMode]
    /// <summary>
    /// 可变长宽表格
    /// </summary>
    public class UguiTable : UIBehaviour,IUguiContent
    {
        [SerializeField]protected Axis m_StartAxis;
        [SerializeField]protected Vector2 m_Spacing;
        protected List<UguiObject> m_Children;
        protected Action m_OnReposition;

        [Range(1,100)]
        [SerializeField]protected int m_ConstraintCount;
        protected bool m_Reposition;

        protected UguiTable()
        {
            m_Children = new List<UguiObject>();
        }

        protected override void Start()
        {
            base.Start();

            m_Reposition = true;
        }

        protected virtual void Update()
        {
            if (m_Reposition)
            {

            }
            Reposition();
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

        }

        public virtual void Reposition()
        {

        }

        public void Set(List<UguiObjectData> dataList, UguiObject prefab)
        {

        }

        public enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }
    }
}