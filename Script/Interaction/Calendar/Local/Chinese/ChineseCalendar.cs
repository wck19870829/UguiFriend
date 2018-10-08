using UnityEngine;
using System.Collections;
using System;
using System.Globalization;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 中国日历
    /// </summary>
    public class ChineseCalendar : Calendar<ChineseCalendarConfig>
    {
        static ChineseLunisolarCalendar chineseLunisolarCalendar; 

        static ChineseCalendar()
        {
            chineseLunisolarCalendar = new ChineseLunisolarCalendar();
        }

        /// <summary>
        /// 公历转农历
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime Solar2Lunisolar(int solarYear, int solarMonth, int solarDay)
        {
            var lunisolar = new DateTime(solarYear, solarMonth, solarDay, chineseLunisolarCalendar);
            return lunisolar;
        }

        /// <summary>
        /// 农历转公历
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime Lunisolar2Solar(int lunisolarYear, int lunisolarMonth, int lunisolarDay)
        {
            DateTime dateTime=new DateTime();
            try
            {
                Debug.LogFormat("{0}  {1}  {2}", lunisolarYear, lunisolarMonth, lunisolarDay);
                dateTime =chineseLunisolarCalendar.ToDateTime(lunisolarYear, lunisolarMonth, lunisolarDay, 0, 0, 0, 0);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("{0}",e);
            }

            return dateTime;
        }
    }
}