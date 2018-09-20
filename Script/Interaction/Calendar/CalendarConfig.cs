using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 配置文件
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
        public List<DateMarkOnce> once;
        public List<DateMarkLoopWeek> weekLoop;
        public List<DateMarkLoopYear> yearLoop;

        public virtual string GetMarks(DateTime date)
        {
            return string.Empty;
        }

        /// <summary>
        /// 日期标记基类
        /// </summary>
        public abstract class DateMarkBase
        {
            public string mark;         //标记，可以为节日，纪念日...
            public bool isDayOff;       //是否休息日
        }

        [Serializable]
        /// <summary>
        /// 只重复一次
        /// </summary>
        public class DateMarkOnce: DateMarkBase
        {
            public int year;
            public int month;
            public int day;
        }

        [Serializable]
        /// <summary>
        /// 每周某天重复
        /// </summary>
        public class DateMarkLoopWeek : DateMarkBase
        {
            public DayOfWeek day;
        }

        [Serializable]
        /// <summary>
        /// 每年重复
        /// </summary>
        public class DateMarkLoopYear : DateMarkBase
        {
            public int month;
            public int day;
        }
    }
}