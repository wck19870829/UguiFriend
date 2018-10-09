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
        protected Dictionary<MonthDay, ChineseDateMarkYearAfterYear> lunisolarYearLoopDict;

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            lunisolarYearLoopDict = new Dictionary<MonthDay, ChineseDateMarkYearAfterYear>(lunisolarYearLoop.Count);
            foreach (var mark in lunisolarYearLoop)
            {
                try
                {
                    lunisolarYearLoopDict.Add(mark.lunisolar, mark);
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
        public MonthDay lunisolar;
    }
}