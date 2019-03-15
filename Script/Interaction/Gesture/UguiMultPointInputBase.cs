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
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        protected Canvas m_canvas;
        protected Transform m_Target;
        protected List<PointerEventData> pointerList;
        protected bool isDirty;
        protected Vector2 screenPos;

        public Action OnBeginDragEvent;
        public Action OnEndDragEvent;

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
            if (isDirty)
            {
                if (pointerList.Count>0)
                {
                    screenPos = pointerList[0].position;
                    for (var i=1;i<pointerList.Count;i++)
                    {
                        screenPos = Vector2.Lerp(screenPos, pointerList[i].position, 0.5f);
                    }
                    DoChange();

                    isDirty = false;
                }
            }
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

        public virtual void OnDrag(PointerEventData eventData)
        {
            isDirty = true;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (OnBeginDragEvent != null)
            {
                OnBeginDragEvent.Invoke();
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (OnEndDragEvent != null)
            {
                OnEndDragEvent.Invoke();
            }
        }

        /// <summary>
        /// 控制目标
        /// </summary>
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