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
        public List<ChineseDateMarkYearAfterYear> lunisolarYearLoop;
        protected Dictionary<DateTime, ChineseDateMarkYearAfterYear> lunisolarYearLoopDict;

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            lunisolarYearLoopDict = new Dictionary<DateTime, ChineseDateMarkYearAfterYear>(lunisolarYearLoop.Count);
            foreach (var mark in lunisolarYearLoop)
            {
                try
                {
                    var solar = ChineseCalendar.Lunisolar2Solar(yearLoopDefaultYear,mark.lunisolarMonth,mark.lunisolarDay);
                    lunisolarYearLoopDict.Add(solar, mark);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("{0}",e);
                }
            }
        }
    }

    [Serializable]
    public class ChineseDateMarkYearAfterYear : DateMarkBase
    {
        public int lunisolarMonth;
        public int lunisolarDay;
    }
}