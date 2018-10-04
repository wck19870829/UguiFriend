using UnityEngine;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 日历配置-中国
    /// </summary>
    public class ChineseCalendarConfig : CalendarConfig
    {
        static ChineseLunisolarCalendar chineseLunisolarCalendar;

        public List<ChineseDateMarkYearAfterYear> lunisolarYearLoop;
        protected Dictionary<DateTime, ChineseDateMarkYearAfterYear> lunisolarYearLoopDict;

        static ChineseCalendarConfig()
        {
            chineseLunisolarCalendar = new ChineseLunisolarCalendar();
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            lunisolarYearLoopDict = new Dictionary<DateTime, ChineseDateMarkYearAfterYear>(lunisolarYearLoop.Count);
            foreach (var mark in lunisolarYearLoop)
            {
                try
                {
                    
                }
                catch (Exception e)
                {

                }
            }
        }

        /// <summary>
        /// 阳历转农历
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetLunisolarDate(DateTime date)
        {
            var lunisolarDate = new DateTime(chineseLunisolarCalendar.GetYear(date),
                                            chineseLunisolarCalendar.GetMonth(date),
                                            chineseLunisolarCalendar.GetDayOfMonth(date)
                                            );
            return lunisolarDate;
        }

        public static DateTime GetLunisolarDate(int year, int month, int day)
        {
            var date = new DateTime(year, month, day);
            return GetLunisolarDate(date);
        }

        /// <summary>
        /// 阴历转阳历
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime GetSolarDate(int year,int month,int day)
        {
            var solar= chineseLunisolarCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            return solar;
        }
    }

    [Serializable]
    public class ChineseDateMarkYearAfterYear : DateMarkBase
    {
        public int lunisolarMonth;
        public int lunisolarDay;
    }
}