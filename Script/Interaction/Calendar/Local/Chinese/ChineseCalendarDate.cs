using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 中国日历日期
    /// </summary>
    public class ChineseCalendarDate : CalendarDate
    {
        public override void Init(CalendarDateInfo info, CalendarConfig calendarConfig)
        {
            base.Init(info, calendarConfig);

            if (info!=null)
            {
                if (string.IsNullOrEmpty(mark.text))
                {
                    var lunisolar = ChineseCalendar.Solar2Lunisolar(info.date);
                    mark.text = ChineseCalendar.FormatDay(lunisolar.day);
                    mark.color = workdayColor;
                }
                else
                {
                    mark.color = weekendColor;
                }
            }
        }
    }
}