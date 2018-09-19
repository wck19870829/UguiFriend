using UnityEngine;
using System.Collections;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 日历配置文件
    /// </summary>
    public class CalendarConfig:MonoBehaviour
    {
        public DayOfWeek weekStart;         //一周开始第一天

        [Header("Week Name")]
        public string monday;
        public string tuesday;
        public string wednesday;
        public string thursday;
        public string friday;
        public string saturday;
        public string sunday;

        [Header("Date Mark")]
        public DateMarkOnce[] once;


        public virtual string GetMarks(DateTime date)
        {
            return string.Empty;
        }

        /// <summary>
        /// 日期标记基类
        /// </summary>
        public class DateMarkBase
        {
            public string mark;
        }

        /// <summary>
        /// 只重复一次
        /// </summary>
        public class DateMarkOnce: DateMarkBase
        {
            public int year;
            public int month;
            public int day;
        }

        /// <summary>
        /// 每周某天重复
        /// </summary>
        public class DateMarkLoopWeek : DateMarkBase
        {
            public DayOfWeek day;
        }

        /// <summary>
        /// 每年重复
        /// </summary>
        public class DateMarkLoopYear : DateMarkBase
        {

        }
    }
}