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
    public class TimeSelector:MonoBehaviour
    {
        [SerializeField]protected bool gotoNow = true;
        [SerializeField] protected UguiWidgetContent hourItem;
        [SerializeField] protected UguiWidgetContent minuteItem;
        int m_CurrentHour;
        int m_CurrentMinute;

        protected virtual void OnEnable()
        {
            var now = DateTime.Now;
            for (var i=0;i<1000;i++)
            {
                var game = new GameObject(i+"_");
                game.transform.SetParent(transform);
            }

            if (gotoNow)
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
