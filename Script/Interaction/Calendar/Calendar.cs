using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 日历
    /// </summary>
    public class Calendar: MonoBehaviour
    {
        const int daysOfWeek = 7;
        const int dayLine=5;
        const int daysDisplayCount = daysOfWeek * dayLine;

        public bool isGotoNowWhenEnable=true;
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

        protected virtual void OnEnable()
        {
            if (m_Config != null)
            {
                Init(m_Config);
                if(isGotoNowWhenEnable)
                    Goto(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config"></param>
        public virtual void Init(CalendarConfig config)
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
        public virtual void NextMonth()
        {

        }

        /// <summary>
        /// 上一月
        /// </summary>
        public virtual void PrevMonth()
        {

        }

        /// <summary>
        /// 跳转到某年某月
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        public virtual void Goto(int year,int month)
        {
            m_CurrentMonth = month;
            m_CurrentYear = year;
            Rebuild();
        }

        /// <summary>
        /// 重建视图
        /// </summary>
        protected virtual void Rebuild()
        {
            if (m_Config == null) return;
            if (dayGrid == null) return;

            dayGrid.startAxis = GridLayoutGroup.Axis.Horizontal;
            dayGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            dayGrid.constraintCount = daysOfWeek;
            var start = new DateTime(m_CurrentYear, m_CurrentMonth, 1);
            var end = start.AddMonths(1).AddDays(-1);
            var startBlank = Mathf.Abs(start.DayOfWeek - m_Config.weekStart);

            var dateItemList = new List<CalendarDateInfo>();
            for (var i=0;i< daysDisplayCount;i++)
            {
                var date = new DateTime();
                var info = new CalendarDateInfo(date);
            }

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
