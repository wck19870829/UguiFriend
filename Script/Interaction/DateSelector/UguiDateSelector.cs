﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(AudioSource))]
    /// <summary>
    /// 日期拾取器
    /// </summary>
    public class UguiDateSelector : MonoBehaviour
    {
        [Tooltip("Go to today when enable.")]
        public bool gotoToday = true;
        [SerializeField] protected UguiWrapContent yearContent;
        [SerializeField] protected UguiWrapContent monthContent;
        [SerializeField] protected UguiWrapContent dayContent;
        [SerializeField] protected Text dateText;
        [SerializeField] protected Button cancelButton;
        [SerializeField] protected Button confirmButton;

        [Header("Sound")]
        [SerializeField] protected AudioClip turnSound;

        protected AudioSource audioSource;
        public int startYear;
        public int startMonth;
        public int startDay;
        public int currentYear;
        public int currentMonth;
        public int currentDay;
        public int daysInMonth;
        bool isDateChange;

        public Action<DateTime> OnDateChange;
        public Action OnCancel;
        public Action OnConfirm;

        protected virtual void Awake()
        {
            if (yearContent == null)
                throw new Exception("Year is null.");
            if (monthContent == null)
                throw new Exception("Month is null.");
            if (dayContent == null)
                throw new Exception("Day is null.");

            audioSource = GetComponent<AudioSource>();

            yearContent.OnInitItem += OnYearInitItem;
            monthContent.OnInitItem += OnMonthInitItem;
            dayContent.OnInitItem += OnDayInitItem;
            yearContent.GetComponent<UguiCenterOnChild>().OnCenter += OnYearCenter;
            monthContent.GetComponent<UguiCenterOnChild>().OnCenter += OnMonthCenter;
            dayContent.GetComponent<UguiCenterOnChild>().OnCenter += OnDayCenter;

            confirmButton.onClick.AddListener(null);
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
                    //限定天数
                    var days = DateTime.DaysInMonth(currentYear, currentMonth);
                    if (days > daysInMonth)
                    {
                        daysInMonth = days;

                    }

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
                dateText.text = currentYear + "/" + currentMonth + "/" + currentDay+" "+
                                date.DayOfWeek;

            //播放声音
            if (audioSource != null)
            {
                if (turnSound != null)
                {
                    audioSource.PlayOneShot(turnSound);
                }
            }
        }

        public virtual void Goto(int year, int month, int day)
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

        protected virtual void OnYearInitItem(RectTransform item, int index, int realIndex)
        {
            var dateItem = item.GetComponent<UguiDateSelectorDate>();
            if (dateItem != null)
            {
                dateItem.Num = realIndex + startYear;
            }
        }

        protected virtual void OnMonthInitItem(RectTransform item, int index, int realIndex)
        {
            var dateItem = item.GetComponent<UguiDateSelectorDate>();
            if (dateItem != null)
            {
                dateItem.name = realIndex + "";
                var value = realIndex >= 0 ?
                            realIndex % 12 + 1 :
                            (realIndex + 1) % 12 + 12;
                dateItem.Num = value;
            }
        }

        protected virtual void OnDayInitItem(RectTransform item, int index, int realIndex)
        {
            if (daysInMonth <= 0) return;

            var dateItem = item.GetComponent<UguiDateSelectorDate>();
            if (dateItem != null)
            {
                dateItem.name = realIndex + "";
                var value = realIndex >= 0 ?
                            realIndex % daysInMonth + 1 :
                            (realIndex + 1) % daysInMonth + daysInMonth;
                dateItem.Num = value;
            }
        }
    }
}