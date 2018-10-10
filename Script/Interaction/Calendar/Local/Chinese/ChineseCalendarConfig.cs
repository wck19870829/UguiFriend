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
        public ChineseCalendarConfig()
        {
            weekBegins = DayOfWeek.Monday;
            dayOfWeekNameDict = new Dictionary<DayOfWeek, string>()
            {
                {DayOfWeek.Sunday,"日"},
                {DayOfWeek.Monday,"一"},
                {DayOfWeek.Tuesday,"二"},
                {DayOfWeek.Wednesday,"三"},
                {DayOfWeek.Thursday,"四"},
                {DayOfWeek.Friday,"五"},
                {DayOfWeek.Saturday,"六"}
            };
        }

        public override DateMark GetMark(DateTime dateTime)
        {
            //按星期
            //if (dateTime.Month == 5)
            //{
            //    if (dateTime.DayOfWeek == DayOfWeek.Sunday)
            //    {
            //        return new DateMark("母亲节");
            //    }
            //}

            //阳历
            if (dateTime.Month == 1&& dateTime.Day == 1)
            {
                return new DateMark("元旦");
            }
            if (dateTime.Month == 2 && dateTime.Day == 14)
            {
                return new DateMark("情人节");
            }
            if (dateTime.Month == 3 && dateTime.Day == 8)
            {
                return new DateMark("妇女节");
            }
            if (dateTime.Month == 3 && dateTime.Day == 12)
            {
                return new DateMark("植树节");
            }
            if (dateTime.Month == 3 && dateTime.Day == 15)
            {
                return new DateMark("消费者权益日");
            }
            if (dateTime.Month == 4 && dateTime.Day == 1)
            {
                return new DateMark("愚人节");
            }
            if (dateTime.Month == 5 && dateTime.Day == 1)
            {
                return new DateMark("劳动节");
            }
            if (dateTime.Month == 5 && dateTime.Day == 1)
            {
                return new DateMark("青年节");
            }
            if (dateTime.Month == 6 && dateTime.Day == 1)
            {
                return new DateMark("儿童节");
            }
            if (dateTime.Month == 7 && dateTime.Day == 1)
            {
                return new DateMark("建党节");
            }
            if (dateTime.Month == 8 && dateTime.Day == 1)
            {
                return new DateMark("建军节");
            }
            if (dateTime.Month == 9 && dateTime.Day == 10)
            {
                return new DateMark("教师节");
            }
            if (dateTime.Month == 11 && dateTime.Day == 1)
            {
                return new DateMark("万圣节");
            }
            if (dateTime.Month == 12 && dateTime.Day == 24)
            {
                return new DateMark("平安夜");
            }
            if (dateTime.Month == 12 && dateTime.Day == 25)
            {
                return new DateMark("圣诞节");
            }

            //阴历
            var lunisolar = ChineseCalendar.Solar2Lunisolar(dateTime);
            if (lunisolar.month == 12 && lunisolar.day == 8)
            {
                return new DateMark("腊八节");
            }
            if (lunisolar.month == 12 && lunisolar.day == 23)
            {
                return new DateMark("小年");
            }
            if (lunisolar.month == 12 && lunisolar.day == 30)
            {
                return new DateMark("除夕");
            }
            if (lunisolar.month == 12 && lunisolar.day == 29)
            {
                //腊月可能为29天
                return new DateMark("除夕");
            }
            if (lunisolar.month == 1 && lunisolar.day == 1)
            {
                return new DateMark("春节");
            }
            if (lunisolar.month == 1 && lunisolar.day == 15)
            {
                return new DateMark("元宵节");
            }
            if (lunisolar.month == 2 && lunisolar.day == 2)
            {
                return new DateMark("龙抬头");
            }
            if (lunisolar.month == 2 && lunisolar.day == 20)
            {
                return new DateMark("清明节");
            }
            if (lunisolar.month == 5 && lunisolar.day == 5)
            {
                return new DateMark("端午节");
            }
            if (lunisolar.month == 7 && lunisolar.day == 7)
            {
                return new DateMark("七夕");
            }
            if (lunisolar.month == 7 && lunisolar.day == 15)
            {
                return new DateMark("中元节");
            }
            if (lunisolar.month == 8 && lunisolar.day == 15)
            {
                return new DateMark("中秋节");
            }
            if (lunisolar.month == 9 && lunisolar.day == 9)
            {
                return new DateMark("重阳节");
            }

            return null;
        }
    }
}