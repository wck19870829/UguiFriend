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
        protected Dictionary<DateTime, Once> onceDict;
        protected Dictionary<DayOfWeek, WeekLoop> weekLoopDict;
        protected Dictionary<DateTime, YearLoop> yearLoopDict;

        /// <summary>
        /// 获取标记信息
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public virtual DateMarkBase GetMark(DateTime dateTime)
        {
            var date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            if (onceDict.ContainsKey(date))
            {
                return onceDict[date];
            }
            if (weekLoopDict.ContainsKey(date.DayOfWeek))
            {
                return weekLoopDict[date.DayOfWeek];
            }
            var yearLoopDate = new DateTime(yearLoopDefaultYear, date.Month,date.Day);
            if (yearLoopDict.ContainsKey(yearLoopDate))
            {
                return yearLoopDict[yearLoopDate];
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
            onceDict = new Dictionary<DateTime, Once>(once.Count);
            foreach (var mark in once)
            {
                try
                {
                    var dateTime = new DateTime(mark.year, mark.month, mark.day);
                    onceDict.Add(dateTime, mark);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("{0}", e);
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
                    Debug.LogErrorFormat("{0}", e);
                }
            }
            yearLoopDict = new Dictionary<DateTime, YearLoop>(yearLoop.Count);
            foreach (var mark in yearLoop)
            {
                try
                {
                    var dateTime = new DateTime(yearLoopDefaultYear, mark.month, mark.day);
                    yearLoopDict.Add(dateTime, mark);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("{0}", e);
                }
            }
        }

        public virtual void OnBeforeSerialize()
        {

        }
    }

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
        public int year;
        public int month;
        public int day;
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
        public int month;
        public int day;
    }
}