using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 多点触控输入基类
    /// </summary>
    public abstract class UguiMultPointInputBase : UIBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IDragHandler
    {
        protected Canvas m_canvas;
        protected Transform m_Target;
        protected List<PointerEventData> pointerList;
        protected bool isDirty;

        protected UguiMultPointInputBase()
        {
            pointerList = new List<PointerEventData>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (m_Target == null)
                m_Target = transform;
            m_canvas = GetComponentInParent<Canvas>();
        }

        protected virtual void Update()
        {
            //UguiMathf.SetPivot(m_Target as RectTransform, pivot);
            if (isDirty)
            {
                if (pointerList.Count>0)
                {
                    //UguiMathf.SetPivot(m_Target as RectTransform, pointerList);
                    DoChange();

                    isDirty = false;
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            isDirty = true;
        }

        protected abstract void DoChange();

        public void OnPointerDown(PointerEventData eventData)
        {
            if (pointerList.IndexOf(eventData) < 0)
                pointerList.Add(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pointerList.Remove(eventData);
        }

        public Transform Target
        {
            get
            {
                return m_Target;
            }
            set
            {
                m_Target = value;
            }
        }
    }
}