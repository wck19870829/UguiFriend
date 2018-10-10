using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 中国日历日期
    /// </summary>
    public class UguiChineseCalendarDate : UguiCalendarDate
    {
        public override void Init(UguiCalendarDateInfo info, UguiCalendarConfig calendarConfig)
        {
            base.Init(info, calendarConfig);

            if (info!=null)
            {
                if (string.IsNullOrEmpty(mark.text))
                {
                    var lunisolar = UguiChineseCalendar.Solar2Lunisolar(info.date);
                    mark.text = UguiChineseCalendar.FormatDay(lunisolar.day);
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