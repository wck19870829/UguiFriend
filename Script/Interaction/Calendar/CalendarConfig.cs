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
    public class CalendarConfig
    {
        public DayOfWeek weekBegins;
        public DayOfWeek[] weekend;
        public Dictionary<DayOfWeek, string> dayOfWeekNameDict;

        public CalendarConfig()
        {
            weekBegins = DayOfWeek.Sunday;
            weekend = new DayOfWeek[]
            {
                DayOfWeek.Saturday,
                DayOfWeek.Sunday
            };
            dayOfWeekNameDict = new Dictionary<DayOfWeek, string>()
            {
                {DayOfWeek.Sunday,"Sun"},
                {DayOfWeek.Monday,"Mon"},
                {DayOfWeek.Tuesday,"Tue"},
                {DayOfWeek.Wednesday,"Wed"},
                {DayOfWeek.Thursday,"Thu"},
                {DayOfWeek.Friday,"Fri"},
                {DayOfWeek.Saturday,"Sat"}
            };
        }

        /// <summary>
        /// 获取标记信息
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public virtual DateMark GetMark(DateTime dateTime)
        {
            return null;
        }
    }

    [Serializable]
    /// <summary>
    /// 日历日期
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
    public class DateMark
    {
        public string mark;                         //标记，可以为节日，纪念日...

        public DateMark(string mark)
        {
            this.mark = mark;
        }
    }
}