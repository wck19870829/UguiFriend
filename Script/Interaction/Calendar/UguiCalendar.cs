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
    public class UguiCalendar : UguiCalendar<UguiCalendarConfig>
    {

    }

    /// <summary>
    /// 日历基类
    /// </summary>
    /// <typeparam name="TConfig">配置文件类型</typeparam>
    public class UguiCalendar<TConfig> : MonoBehaviour
        where TConfig : UguiCalendarConfig, new()
    {
        const int daysOfWeek = 7;
        const int dayLine = 6;
        const int daysDisplayCount = daysOfWeek * dayLine;
        const int daysDisplayCountTotal = daysDisplayCount * 3;

        [Tooltip("Go to today when enable")]
        [SerializeField] protected bool gotoToday = true;
        [SerializeField] protected GridLayoutGroup dayGrid;
        [SerializeField] protected GridLayoutGroup dayOfWeekGrid;
        [SerializeField] protected Button nextMonthButton;
        [SerializeField] protected Button prevMonthButton;
        [SerializeField] protected Text yearTitleText;
        [SerializeField] protected Text monthTitleText;
        [Range(0, 10)]
        [SerializeField] protected int dateSelectLimit = 1;             //日期选择数量上限，0为不能选择日期，1为单选，大于1为多选

        [Header("-Skin")]
        [SerializeField] protected UguiCalendarDayOfWeek dayOfWeekPrefab;
        [SerializeField] protected UguiCalendarDate datePrefab;

        [Header("-DateSelector")]
        [SerializeField] protected Button dateSelectorButton;
        [SerializeField] protected UguiDateSelector dateSelector;

        protected TConfig m_Config;
        List<DayOfWeek> dayOfWeekList;
        HashSet<DateTime> dateSelectSet;
        List<DateTime> dateSelectList;
        int m_ViewYear;
        int m_ViewMonth;

        //日期选中状态改变事件
        public event Action<List<DateTime>> OnDateSelectChangeEvent;
        public event Action<UguiCalendarDate[]> OnDateRebuildEvent;

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
            if (dateSelectorButton != null)
            {
                dateSelectorButton.onClick.AddListener(() =>
                {
                    OpenDateSelector();
                });
            }

            Init();
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
        protected virtual void Init()
        {
            m_Config = new TConfig();

            //创建星期图标
            UguiTools.DestroyChildren(dayOfWeekGrid.gameObject);
            dayOfWeekList = new List<DayOfWeek>(daysOfWeek);
            if (dayOfWeekPrefab != null)
            {
                for (var i = 0; i < daysOfWeek; i++)
                {
                    var clone = GameObject.Instantiate<UguiCalendarDayOfWeek>(dayOfWeekPrefab);
                    clone.transform.SetParent(dayOfWeekGrid.transform);
                    var dayValue = (int)m_Config.weekBegins + i;
                    var dayOfWeek = (dayValue < daysOfWeek) ? (DayOfWeek)dayValue : (DayOfWeek)Mathf.Abs(dayValue - daysOfWeek);
                    dayOfWeekList.Add(dayOfWeek);
                    clone.Set(dayOfWeek, m_Config);
                    clone.gameObject.name = i.ToString();
                }
            }
            else Debug.LogError("DayOfWeek prefab is null!");

            //创建日期
            UguiTools.DestroyChildren(dayGrid.gameObject);
            dayGrid.startAxis = GridLayoutGroup.Axis.Horizontal;
            dayGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            dayGrid.constraintCount = daysOfWeek;
            if (datePrefab != null)
            {
                for (var i = 0; i < daysDisplayCount; i++)
                {
                    var clone = GameObject.Instantiate<UguiCalendarDate>(datePrefab);
                    clone.transform.SetParent(dayGrid.transform);
                    clone.OnClickEvent -= OnDateClick;
                    clone.OnClickEvent += OnDateClick;
                }
            }
            else Debug.LogErrorFormat("Date prefab is null!");
        }

        /// <summary>
        /// 下一月
        /// </summary>
        public virtual void NextMonth()
        {
            var date = new DateTime(m_ViewYear, m_ViewMonth, 1).AddMonths(1);
            Goto(date.Year, date.Month);
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

        protected virtual void OpenDateSelector()
        {
            if (dateSelector != null)
            {

            }
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

            var start = new DateTime(m_ViewYear, m_ViewMonth, 1);
            var end = start.AddMonths(1).AddDays(-1);
            var startBlank = Mathf.Abs(dayOfWeekList.IndexOf(m_Config.weekBegins) - dayOfWeekList.IndexOf(start.DayOfWeek));
            var startDate = start.AddDays(-startBlank);

            //更新日期
            var dateItemList = new List<UguiCalendarDateInfo>();
            for (var i = 0; i < daysDisplayCount; i++)
            {
                var date = startDate.AddDays(i);
                var mark = m_Config.GetMark(date);
                var info = new UguiCalendarDateInfo(date, mark);
                dateItemList.Add(info);
            }
            var dateItems = dayGrid.GetComponentsInChildren<UguiCalendarDate>();
            for (var i = 0; i < daysDisplayCount; i++)
            {
                var dateItem = dateItems[i];
                var info = dateItemList[i];
                dateItem.Init(info, m_Config);
                dateItem.name = GetDateStr(info.date.Year, info.date.Month, info.date.Day);
                dateItem.IsSelect = dateSelectSet.Contains(info.date) ? true : false;
                dateItem.IsToday = info.date == DateTime.Today ? true : false;
                var isActive = (info.date >= start && info.date <= end) ? true : false;
                dateItem.SetActiveState(isActive);
            }

            if (OnDateRebuildEvent != null)
            {
                OnDateRebuildEvent.Invoke(dateItems);
            }
        }

        /// <summary>
        /// 日期点击
        /// </summary>
        /// <param name="calendarDate"></param>
        protected virtual void OnDateClick(UguiCalendarDate calendarDate)
        {
            if (calendarDate.Info.date.Year == m_ViewYear && calendarDate.Info.date.Month == m_ViewMonth)
            {
                //点击当月日期为选择日期
                if (dateSelectSet.Contains(calendarDate.Info.date))
                {
                    //取消选择
                    dateSelectSet.Remove(calendarDate.Info.date);
                    dateSelectList.RemoveAll((x) =>
                    {
                        return x == calendarDate.Info.date ? true : false;
                    }
                    );
                    calendarDate.IsSelect = false;
                }
                else
                {
                    if (dateSelectList.Count > 0)
                    {
                        if (dateSelectList.Count >= dateSelectLimit)
                        {
                            var dateItems = dayGrid.GetComponentsInChildren<UguiCalendarDate>();
                            var removeCount = Math.Abs(dateSelectLimit - dateSelectList.Count) + 1;
                            for (var i = 0; i < removeCount; i++)
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
            else
            {
                //点击其他月日期跳转到对应月份
                Goto(calendarDate.Info.date.Year, calendarDate.Info.date.Month);
            }
        }

        /// <summary>
        /// 清除所有选择
        /// </summary>
        public void ClearAllSelectDate()
        {
            dateSelectList.Clear();
            dateSelectSet.Clear();
            var dateItems = dayGrid.GetComponentsInChildren<UguiCalendarDate>();
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
        public UguiCalendarConfig Config
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