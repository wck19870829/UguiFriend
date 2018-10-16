using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 日期拾取器
    /// </summary>
    public class UguiDateSelector : MonoBehaviour
    {
        [Tooltip("Go to today when enable.")]
        public bool gotoToday=true;
        [SerializeField] protected UguiWrapContent yearContent;
        [SerializeField] protected UguiWrapContent monthContent;
        [SerializeField] protected UguiWrapContent dayContent;
        [SerializeField] protected Text dateText;
        int startYear;
        int startMonth;
        int startDay;
        int currentYear;
        int currentMonth;
        int currentDay;
        bool isDateChange;

        public Action<DateTime> OnDateChange;

        protected virtual void Awake()
        {
            if (yearContent == null)
                throw new Exception("Year is null.");
            if (monthContent == null)
                throw new Exception("Month is null.");
            if (dayContent == null)
                throw new Exception("Day is null.");

            yearContent.OnInitItem += OnYearInitItem;
            monthContent.OnInitItem += OnMonthInitItem;
            dayContent.OnInitItem += OnDayInitItem;
            yearContent.GetComponent<UguiCenterOnChild>().OnCenter += OnYearCenter;
            monthContent.GetComponent<UguiCenterOnChild>().OnCenter += OnMonthCenter;
            dayContent.GetComponent<UguiCenterOnChild>().OnCenter += OnDayCenter;
        }

        protected virtual void OnEnable()
        {
            if (gotoToday)
            {
                Goto(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
            }
        }

        protected virtual void Update()
        {
            if (isDateChange)
            {
                isDateChange = false;

                try
                {
                    var selectDay = new DateTime(currentYear, currentMonth, currentDay);
                    DateChange(selectDay);

                    if (OnDateChange != null)
                    {
                        OnDateChange.Invoke(selectDay);
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        protected virtual void DateChange(DateTime date)
        {
            if (dateText != null)
                dateText.text = currentYear + "年" + currentMonth + "月" + currentDay + "日";
        }

        public virtual void Goto(int year,int month,int day)
        {
            startYear = year;
            startMonth = month;
            startDay = day;

            yearContent.Realign();
            monthContent.Realign();
            dayContent.Realign();
        }

        protected virtual void OnYearCenter(Transform center)
        {
            isDateChange = true;
            currentYear = center.GetComponent<UguiDateSelectorDate>().Num;
        }

        protected virtual void OnMonthCenter(Transform center)
        {
            isDateChange = true;
            currentMonth = center.GetComponent<UguiDateSelectorDate>().Num;
        }

        protected virtual void OnDayCenter(Transform center)
        {
            isDateChange = true;
            currentDay = center.GetComponent<UguiDateSelectorDate>().Num;
        }

        protected virtual void OnYearInitItem(RectTransform item,int index,int realIndex)
        {
            var dateItem = item.GetComponent<UguiDateSelectorDate>();
            if (dateItem != null)
            {
                dateItem.Num = startYear + realIndex;
            }
        }

        protected virtual void OnMonthInitItem(RectTransform item, int index, int realIndex)
        {
            var dateItem = item.GetComponent<UguiDateSelectorDate>();
            if (dateItem != null)
            {
                dateItem.Num=realIndex;
            }
        }

        protected virtual void OnDayInitItem(RectTransform item, int index, int realIndex)
        {
            var dateItem = item.GetComponent<UguiDateSelectorDate>();
            if (dateItem != null)
            {
                dateItem.Num = realIndex;
            }
        }
    }
}