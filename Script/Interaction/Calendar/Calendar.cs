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
        where TConfig:CalendarConfig,new()
    {
        const int daysOfWeek = 7;
        const int dayLine = 6;
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
        [SerializeField] protected int dateSelectLimit = 1;             //日期选择数量上限，0为不能选择日期，1为单选，大于1为多选

        [Header("Skin")]
        [SerializeField] protected CalendarDayOfWeek dayOfWeekPrefab;
        [SerializeField] protected CalendarDate datePrefab;

        protected TConfig m_Config;
        List<DayOfWeek> dayOfWeekList;
        HashSet<DateTime> dateSelectSet;
        List<DateTime> dateSelectList;
        int m_ViewYear;
        int m_ViewMonth;

        //日期选中状态改变事件
        public Action<List<DateTime>> OnDateSelectChangeEvent;

        protected virtual void Awake()
        {
            dateSelectSet = new HashSet<DateTime>();
            dateSelectList = new List<DateTime>();
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

            m_Config = new TConfig();
            Init(m_Config);
        }

        protected virtual void OnEnable()
        {
            if (m_Config != null)
            {
                if (gotoToday)
                    Goto(DateTime.Today.Year, DateTime.Today.Month);
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
            dayOfWeekList = new List<DayOfWeek>(daysOfWeek);
            if (dayOfWeekPrefab != null)
            {
                for (var i = 0; i < daysOfWeek; i++)
                {
                    var clone = GameObject.Instantiate<CalendarDayOfWeek>(dayOfWeekPrefab);
                    clone.transform.SetParent(dayOfWeekGrid.transform);
                    var dayValue = (int)m_Config.weekBegins + i;
                    var dayOfWeek = (dayValue < daysOfWeek) ? (DayOfWeek)dayValue : (DayOfWeek)Mathf.Abs(dayValue - daysOfWeek);
                    dayOfWeekList.Add(dayOfWeek);
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
            var date = new DateTime(m_ViewYear, m_ViewMonth, 1).AddMonths(1);
            Goto(date.Year,date.Month);
        }

        /// <summary>
        /// 上一月
        /// </summary>
        public virtual void PrevMonth()
        {
            var date = new DateTime(m_ViewYear, m_ViewMonth, 1).AddMonths(-1);
            Goto(date.Year, date.Month);
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
            var startBlank = Mathf.Abs(dayOfWeekList.IndexOf(m_Config.weekBegins)-dayOfWeekList.IndexOf(start.DayOfWeek));
            var startDate = start.AddDays(-startBlank);

            var dateItemList = new List<CalendarDateInfo>();
            for (var i = 0; i < daysDisplayCount; i++)
            {
                var date = startDate.AddDays(i);
                var mark = m_Config.GetMark(date);
                var info = new CalendarDateInfo(date, mark);
                dateItemList.Add(info);
            }
            UguiTools.DestroyChildren(dayGrid.gameObject);
            if (datePrefab!=null)
            {
                foreach (var info in dateItemList)
                {
                    var clone = GameObject.Instantiate<CalendarDate>(datePrefab);
                    clone.transform.SetParent(dayGrid.transform);
                    clone.Init(info,m_Config);
                    clone.name = info.date.Year.ToString()
                                +info.date.Month.ToString("00")
                                +info.date.Day.ToString("00");
                    clone.OnClickEvent -= OnDateClick;
                    clone.OnClickEvent += OnDateClick;
                    if (dateSelectSet.Contains(info.date))
                    {
                        clone.IsSelect = true;
                    }
                    if (info.date>=start&&info.date<=end)
                    {
                        clone.SetActiveState();
                    }
                    else
                    {
                        clone.SetInactiveState();
                    }
                }
            }
            else
            {
                Debug.LogErrorFormat("Date prefab is null!");
            }
        }

        protected virtual void OnDateClick(CalendarDate calendarDate)
        {
            if (dateSelectSet.Contains(calendarDate.Info.date))
            {
                //取消选择
                dateSelectSet.Remove(calendarDate.Info.date);
                dateSelectList.RemoveAll((x)=> 
                    {
                        return x == calendarDate.Info.date ? true : false;
                    }
                );
                calendarDate.IsSelect = false;
            }
            else
            {
                if (dateSelectList.Count>0)
                {
                    if (dateSelectList.Count >= dateSelectLimit)
                    {
                        var dateItems = dayGrid.GetComponentsInChildren<CalendarDate>();
                        var removeCount = Math.Abs(dateSelectLimit-dateSelectList.Count)+1;
                        for (var i=0;i<removeCount;i++)
                        {
                            var removeDate = dateSelectList[0];
                            dateSelectSet.Remove(removeDate);
                            dateSelectList.RemoveAt(0);
                            foreach (var dateItem in dateItems)
                            {
                                if (dateItem.Info.date == removeDate)
                                {
                                    dateItem.IsSelect = false;
                                }
                            }
                        }
                    }
                }
                dateSelectSet.Add(calendarDate.Info.date);
                dateSelectList.Add(calendarDate.Info.date);
                calendarDate.IsSelect = true;
            }

            if (OnDateSelectChangeEvent != null)
            {
                OnDateSelectChangeEvent.Invoke(dateSelectList);
            }
        }

        /// <summary>
        /// 清除所有选择
        /// </summary>
        public void ClearAllSelectDate()
        {
            dateSelectList.Clear();
            dateSelectSet.Clear();
            var dateItems = dayGrid.GetComponentsInChildren<CalendarDate>();
            foreach (var dateItem in dateItems)
            {
                dateItem.IsSelect = false;
            }

            if (OnDateSelectChangeEvent != null)
            {
                OnDateSelectChangeEvent.Invoke(dateSelectList);
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

        /// <summary>
        /// 配置文件
        /// </summary>
        public CalendarConfig Config
        {
            get
            {
                return m_Config;
            }
        }

        #region 静态方法

        ///// <summary>
        ///// 获取本月第几周
        ///// </summary>
        ///// <param name="date"></param>
        ///// <param name="weekStart"></param>
        ///// <returns></returns>
        //public static int GetWeekOfMonth(DateTime date,DayOfWeek weekStart)
        //{
        //    var firstDay = new DateTime(date.Year,date.Month,1);
        //    firstDay.DayOfWeek
        //}

        /// <summary>
        /// 获取日期字符串
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns>返回如20180125格式</returns>
        public static string GetDateStr(int year, int month, int day)
        {
            var dateStr = year.ToString("0000")
                        + month.ToString("00")
                        + day.ToString("00");

            return dateStr;
        }

        #endregion
    }
}