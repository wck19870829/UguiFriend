using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [Serializable]
    /// <summary>
    /// 配置文件
    /// </summary>
    public class CalendarConfig: CalendarConfig<DateMarkOnce, DateMarkWeekAfterWeek, DateMarkYearAfterYear>
    {

    }

    [Serializable]
    /// <summary>
    /// 配置文件
    /// </summary>
    public class CalendarConfig<Once,WeekLoop,YearLoop>: MonoBehaviour, ISerializationCallbackReceiver
        where Once:DateMarkOnce
        where WeekLoop:DateMarkWeekAfterWeek
        where YearLoop:DateMarkYearAfterYear
    {
        protected const int yearLoopDefaultYear=2000;

        [Header("Convention")]
        [Tooltip("A week begins on the day of the week")]
        public DayOfWeek weekBegins;
        public DayOfWeek[] weekend;

        [Header("Day of week")]
        public string monday;
        public string tuesday;
        public string wednesday;
        public string thursday;
        public string friday;
        public string saturday;
        public string sunday;
        public Dictionary<DayOfWeek, string> dayOfWeekNameDict;

        [Header("Date mark")]
        public List<Once> once;
        public List<WeekLoop> weekLoop;
        public List<YearLoop> yearLoop;
        protected Dictionary<YearMonthDay, Once> onceDict;
        protected Dictionary<DayOfWeek, WeekLoop> weekLoopDict;
        protected Dictionary<MonthDay, YearLoop> yearLoopDict;

        /// <summary>
        /// 获取标记信息
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public virtual DateMarkBase GetMark(DateTime dateTime)
        {
            var yearMonthDay = new YearMonthDay(dateTime.Year, dateTime.Month, dateTime.Day);
            if (onceDict.ContainsKey(yearMonthDay))
            {
                return onceDict[yearMonthDay];
            }
            if (weekLoopDict.ContainsKey(dateTime.DayOfWeek))
            {
                return weekLoopDict[dateTime.DayOfWeek];
            }
            var monthDay = new MonthDay(dateTime.Month, dateTime.Day);
            if (yearLoopDict.ContainsKey(monthDay))
            {
                return yearLoopDict[monthDay];
            }

            return null;
        }

        public virtual void OnAfterDeserialize()
        {
            dayOfWeekNameDict = new Dictionary<DayOfWeek, string>() {
                { DayOfWeek.Sunday, sunday},
                { DayOfWeek.Monday, monday},
                { DayOfWeek.Tuesday, tuesday},
                { DayOfWeek.Wednesday, wednesday},
                { DayOfWeek.Thursday, thursday},
                { DayOfWeek.Friday, friday},
                { DayOfWeek.Saturday, saturday}
            };
            onceDict = new Dictionary<YearMonthDay, Once>(once.Count);
            foreach (var mark in once)
            {
                try
                {
                    onceDict.Add(mark.date, mark);
                }
                catch (Exception e)
                {
                    Debug.LogWarningFormat("{0}", e);
                }
            }
            weekLoopDict = new Dictionary<DayOfWeek, WeekLoop>(weekLoop.Count);
            foreach (var mark in weekLoop)
            {
                try
                {
                    weekLoopDict.Add(mark.dayOfWeek, mark);
                }
                catch (Exception e)
                {
                    Debug.LogWarningFormat("{0}", e);
                }
            }
            yearLoopDict = new Dictionary<MonthDay, YearLoop>(yearLoop.Count);
            foreach (var mark in yearLoop)
            {
                try
                {
                    yearLoopDict.Add(mark.date, mark);
                }
                catch (Exception e)
                {
                    Debug.LogWarningFormat("{0}", e);
                }
            }
        }

        public virtual void OnBeforeSerialize()
        {

        }
    }

    [Serializable]
    /// <summary>
    /// 日期
    /// </summary>
    public struct YearMonthDay
    {
        public int year;
        public int month;
        public int day;

        public YearMonthDay(int year,int month,int day)
        {
            this.year = year;
            this.month = month;
            this.day = day;
        }
    }

    [Serializable]
    /// <summary>
    /// 日历日期(无年)
    /// </summary>
    public struct MonthDay
    {
        public int month;
        public int day;

        public MonthDay(int month,int day)
        {
            this.month = month;
            this.day = day;
        }
    }

    [Serializable]
    /// <summary>
    /// 日期标记基类
    /// </summary>
    public abstract class DateMarkBase
    {
        public string mark;                         //标记，可以为节日，纪念日...
        public bool isHolidays;                     //是否节假日
    }

    [Serializable]
    /// <summary>
    /// 只重复一次
    /// </summary>
    public class DateMarkOnce : DateMarkBase
    {
        public YearMonthDay date;
    }

    [Serializable]
    /// <summary>
    /// 每周重复
    /// </summary>
    public class DateMarkWeekAfterWeek : DateMarkBase
    {
        public DayOfWeek dayOfWeek;
    }

    [Serializable]
    /// <summary>
    /// 每年重复
    /// </summary>
    public class DateMarkYearAfterYear : DateMarkBase
    {
        public MonthDay date;
    }
}