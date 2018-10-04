using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 日历星期
    /// </summary>
    public class CalendarDayOfWeek : MonoBehaviour
    {
        [SerializeField] protected Color workdayColor=Color.black;
        [SerializeField] protected Color weekendColor = Color.black;
        [SerializeField] protected Text dayOfWeekText;

        public virtual void Set(DayOfWeek dayOfWeek, CalendarConfig config)
        {
            if (dayOfWeekText != null)
            {
                dayOfWeekText.text = config.dayOfWeekNameDict[dayOfWeek];
                var isWeekend = false;
                foreach (var weekend in config.weekend)
                {
                    if(weekend== dayOfWeek)
                    {
                        isWeekend = true;
                        break;
                    }
                }

                dayOfWeekText.color = isWeekend?weekendColor:workdayColor;
            }
        }
    }
}