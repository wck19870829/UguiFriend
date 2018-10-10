using UnityEngine;
using System.Collections;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 日期拾取器
    /// </summary>
    public class UguiDateSelector : MonoBehaviour
    {
        protected int m_CurrentYear;
        protected int m_CurrentMonth;
        protected int m_CurrentDay;

        public event Action OnSelectChangeEvent; 

        public virtual void Goto(int year,int month,int day)
        {


            if (OnSelectChangeEvent != null)
            {
                OnSelectChangeEvent.Invoke();
            }
        }

        public int CurrentYear
        {
            get
            {
                return m_CurrentYear;
            }
        }

        public int CurrentMonth
        {
            get
            {
                return m_CurrentMonth;
            }
        }

        public int CurrentDay
        {
            get
            {
                return m_CurrentDay;
            }
        }
    }
}