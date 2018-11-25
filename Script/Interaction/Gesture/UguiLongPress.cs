using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 长按
    /// </summary>
    public class UguiLongPress : UIBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] protected AnimationCurve m_RateCurve;
        protected float startTime = 0.2f;
        protected float tickInterval = 0.1f;
        protected float m_Time;
        protected int m_TickTotalCount;

        public Action OnClick;                  //点击事件

        public Action OnLongPressBegin;         //长按开始
        public Action<int> OnLongPressTick;     //长按中
        public Action OnLongPressEnd;           //长按结束

        protected UguiLongPress()
        {
            m_RateCurve = AnimationCurve.Linear(0, 0, 3, 30);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            CancelInvoke();
            Invoke("OnCheckStart", startTime);
            m_Time = 0;
            m_TickTotalCount = 0;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            CancelInvoke();

            if (m_TickTotalCount == 0)
            {
                if (OnClick != null)
                {
                    OnClick.Invoke();
                }
            }
            else
            {
                if (OnLongPressEnd != null)
                {
                    OnLongPressEnd.Invoke();
                }
            }
            m_Time = 0;
        }

        protected virtual void OnCheckStart()
        {
            CancelInvoke();
            InvokeRepeating("OnTickHandler", 0, tickInterval);

            if (OnLongPressBegin != null)
            {
                OnLongPressBegin.Invoke();
            }
        }

        protected virtual void OnTickHandler()
        {
            m_Time += tickInterval;
            var tickCount = (int)m_RateCurve.Evaluate(m_Time);
            m_TickTotalCount += tickCount;

            if (OnLongPressTick != null)
            {
                OnLongPressTick.Invoke(tickCount);
            }
        }

        /// <summary>
        /// 按下的时间
        /// </summary>
        public float Time
        {
            get
            {
                return m_Time;
            }
        }
    }
}