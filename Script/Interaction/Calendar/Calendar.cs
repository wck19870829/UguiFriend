using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 日历
    /// </summary>
    public class Calendar: MonoBehaviour
    {
        const int weekCount = 7;

        [SerializeField]protected GridLayoutGroup dayGrid;
        [SerializeField]protected GridLayoutGroup weekGrid;
        [SerializeField]protected CalendarConfig m_Config;
        [SerializeField]protected Button nextMonthButton;
        [SerializeField]protected Button prevMonthButton;
        int m_CurrentYear;
        int m_CurrentMonth;

        public Action OnSelectEvent;

        protected virtual void Awake()
        {
            if (nextMonthButton!=null)
            {
                nextMonthButton.onClick.AddListener(() => {
                    NextMonth();
                });
            }
            if (prevMonthButton != null)
            {
                prevMonthButton.onClick.AddListener(() => {
                    PrevMonth();
                });
            }
        }

        protected virtual void OnEnabled()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config"></param>
        public void Init(CalendarConfig config)
        {
            if (config == null)
            {
                Debug.LogErrorFormat("config is null.");
                return;
            }

            m_Config = config;
            Rebuild();
        }

        /// <summary>
        /// 下一月
        /// </summary>
        public void NextMonth()
        {

        }

        /// <summary>
        /// 上一月
        /// </summary>
        public void PrevMonth()
        {

        }

        /// <summary>
        /// 跳转到某年某月
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        public void Goto(int year,int month)
        {


            Rebuild();
        }

        void Rebuild()
        {
            if (m_Config == null) return;

            var start = new DateTime(m_CurrentYear, m_CurrentMonth, 1);
            var end = start.AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 当前年
        /// </summary>
        public int CurrentYear
        {
            get
            {
                return m_CurrentYear;
            }
        }

        /// <summary>
        /// 当前月
        /// </summary>
        public int CurrentMonth
        {
            get
            {
                return m_CurrentMonth;
            }
        }
    }
}
