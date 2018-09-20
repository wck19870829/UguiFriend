using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 时间拾取器
    /// </summary>
    public class TimePicker
    {
        public bool isGotoNowWhenEnabled = true;
        [SerializeField] protected DisplayObjectContainer hourItem;
        [SerializeField] protected DisplayObjectContainer minuteItem;
        int m_CurrentHour;
        int m_CurrentMinute;

        protected virtual void OnEnabled()
        {
            if (isGotoNowWhenEnabled)
            {

            }
        }

        /// <summary>
        /// 跳转到某时某分
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        public virtual void Goto(int hour,int minute)
        {

        }

        /// <summary>
        /// 当前时
        /// </summary>
        public int CurrentHour
        {
            get
            {
                return m_CurrentHour;
            }
        }

        /// <summary>
        /// 当前分
        /// </summary>
        public int CurrentMinute
        {
            get
            {
                return m_CurrentMinute;
            }
        }
    }
}
