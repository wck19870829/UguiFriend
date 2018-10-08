using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 通用日历
    /// </summary>
    public class Calendar: Calendar<CalendarConfig>
    {

    }

    /// <summary>
    /// 日历基类
    /// </summary>
    /// <typeparam name="TConfig">配置文件类型</typeparam>
    public class Calendar<TConfig>: MonoBehaviour
        where TConfig:CalendarConfig
    {
        const int daysOfWeek = 7;
        const int dayLine = 5;
        const int daysDisplayCount = daysOfWeek * dayLine;
        const int daysDisplayCountTotal = daysDisplayCount * 3;

        [Tooltip("Go to today when enable")]
        public bool gotoToday = true;

        [SerializeField] protected GridLayoutGroup dayGrid;
        [SerializeField] protected GridLayoutGroup dayOfWeekGrid;
        [SerializeField] protected Button nextMonthButton;
        [SerializeField] protected Button prevMonthButton;
        [SerializeField] protected Text yearTitleText;
        [SerializeField] protected Text monthTitleText;

        [Header("Skin")]
        [SerializeField] protected CalendarDayOfWeek dayOfWeekPrefab;
        [SerializeField] protected CalendarDate datePrefab;

        [Header("Config")]
        [SerializeField] protected TConfig m_Config;
        int m_ViewYear;
        int m_ViewMonth;

        //日期选中状态改变事件
        public Action<List<CalendarDate>> OnDateSelectChangeEvent;

        protected virtual void Awake()
        {
            if (nextMonthButton != null)
            {
                nextMonthButton.onClick.AddListener(() =>
                {
                    NextMonth();
                });
            }
            if (prevMonthButton != null)
            {
                prevMonthButton.onClick.AddListener(() =>
                {
                    PrevMonth();
                });
            }

            if (m_Config != null)
            {
                Init(m_Config);
            }
        }

        protected virtual void OnEnable()
        {
            if (m_Config != null)
            {
                if (gotoToday)
                    Goto(DateTime.Now.Year, DateTime.Now.Month);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config"></param>
        public virtual void Init(TConfig config)
        {
            if (config == null)
            {
                Debug.LogErrorFormat("config is null.");
                return;
            }

            m_Config = config;

            UguiTools.DestroyChildren(dayOfWeekGrid.gameObject);
            if (dayOfWeekPrefab != null)
            {
                for (var i = 0; i < daysOfWeek; i++)
                {
                    var clone = GameObject.Instantiate<CalendarDayOfWeek>(dayOfWeekPrefab);
                    clone.transform.SetParent(dayOfWeekGrid.transform);
                    var dayValue = (int)m_Config.weekBegins + i;
                    var dayOfWeek = (dayValue < daysOfWeek) ? (DayOfWeek)dayValue : (DayOfWeek)Mathf.Abs(dayValue - daysOfWeek);
                    clone.Set(dayOfWeek, config);
                    clone.gameObject.name = i.ToString();
                }
            }
            else
            {
                Debug.LogError("DayOfWeekPrefab is null!");
            }
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
        public virtual void Goto(int year, int month)
        {
            m_ViewMonth = month;
            m_ViewYear = year;
            Rebuild();
        }

        /// <summary>
        /// 重建视图
        /// </summary>
        protected virtual void Rebuild()
        {
            if (m_Config == null) return;
            if (dayGrid == null) return;

            if (monthTitleText != null) monthTitleText.text = m_ViewMonth.ToString();
            if (yearTitleText != null) yearTitleText.text = m_ViewYear.ToString();

            dayGrid.startAxis = GridLayoutGroup.Axis.Horizontal;
            dayGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            dayGrid.constraintCount = daysOfWeek;
            var start = new DateTime(m_ViewYear, m_ViewMonth, 1);
            var end = start.AddMonths(1).AddDays(-1);
            var startBlank = Mathf.Abs(start.DayOfWeek - m_Config.weekBegins);

            var dateItemList = new List<CalendarDateInfo>();
            for (var i = 0; i < daysDisplayCount; i++)
            {
                var date = new DateTime();
                var info = new CalendarDateInfo(date);
            }
        }

        /// <summary>
        /// 当前年
        /// </summary>
        public int ViewYear
        {
            get
            {
                return m_ViewYear;
            }
        }

        /// <summary>
        /// 当前月
        /// </summary>
        public int ViewMonth
        {
            get
            {
                return m_ViewMonth;
            }
        }
    }
}