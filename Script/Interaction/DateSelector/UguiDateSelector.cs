using UnityEngine;
using System.Collections;
using System;

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
        int startYear;
        int startMonth;
        int startDay;

        public Action<int,int,int> OnSelectChangeEvent;

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
        }

        protected virtual void OnEnable()
        {
            if (gotoToday)
            {
                Goto(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
            }
        }

        public virtual void Goto(int year,int month,int day)
        {
            startYear = year;
            startMonth = month;
            startDay = day;

            yearContent.Realign();
            monthContent.Realign();
            dayContent.Realign();

            if (OnSelectChangeEvent != null)
            {
                OnSelectChangeEvent.Invoke(year,month,day);
            }
        }

        protected virtual void OnYearInitItem(RectTransform item,int index,int realIndex)
        {
            var dateItem = item.GetComponent<UguiDateSelectorDate>();
            if (dateItem != null)
            {
                dateItem.Set(startYear+realIndex);
            }
        }

        protected virtual void OnMonthInitItem(RectTransform item, int index, int realIndex)
        {
            var dateItem = item.GetComponent<UguiDateSelectorDate>();
            if (dateItem != null)
            {
                dateItem.Set(startMonth + realIndex);
            }
        }

        protected virtual void OnDayInitItem(RectTransform item, int index, int realIndex)
        {
            var dateItem = item.GetComponent<UguiDateSelectorDate>();
            if (dateItem != null)
            {
                dateItem.Set(startDay + realIndex);
            }
        }
    }
}