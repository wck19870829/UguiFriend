using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(CanvasGroup))]
    /// <summary>
    /// 日历日期
    /// </summary>
    public class CalendarDate : MonoBehaviour
    {
        [SerializeField] protected Text dateText;
        [SerializeField] protected Text mark;
        [SerializeField] protected GameObject selectIcon;
        [SerializeField] protected GameObject todayIcon;
        [SerializeField] protected GameObject mattersIcon;
        [SerializeField] protected Text mattersText;
        [SerializeField] protected Color workdayColor = Color.black;
        [SerializeField] protected Color weekendColor = Color.black;
        protected CalendarDateInfo m_Info;
        protected CalendarConfig m_CalendarConfig;
        protected CanvasGroup m_CanvasGroup;
        protected int m_MattersCount;
        protected bool m_IsSelect;
        protected bool m_IsToday;

        public event Action<CalendarDate> OnClickEvent;

        protected virtual void Awake()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
            if (m_CanvasGroup == null)
                m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();
            var hotArea = GetComponent<RawImage>();
            if (hotArea == null)
                hotArea = gameObject.AddComponent<RawImage>();
            hotArea.color = Color.clear;
            MattersCount = 0;
            IsSelect = false;
            IsToday = false;
        }

        protected virtual void Start()
        {
            var button = GetComponent<Button>();
            if (button == null)
            {
                button = gameObject.AddComponent<Button>();
            }
            button.onClick.AddListener(() =>
            {
                if (OnClickEvent != null)
                {
                    OnClickEvent.Invoke(this);
                }
            });
        }

        /// <summary>
        /// 设置激活
        /// </summary>
        public virtual void SetActiveState(bool isActive)
        {
            m_CanvasGroup.alpha = isActive?1:0.3f;
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        public virtual bool IsSelect
        {
            get
            {
                return m_IsSelect;
            }
            set
            {
                m_IsSelect = value;
                if (selectIcon != null)
                {
                    selectIcon.SetActive(value);
                }
            }
        }

        /// <summary>
        /// 是否今日
        /// </summary>
        public virtual bool IsToday
        {
            get
            {
                return m_IsToday;
            }
            set
            {
                m_IsToday = value;
                if (todayIcon!=null)
                {
                    todayIcon.SetActive(value);
                }
            }
        }

        /// <summary>
        /// 当日事宜数
        /// 无事宜隐藏事宜标志
        /// </summary>
        public virtual int MattersCount
        {
            get
            {
                return m_MattersCount;
            }
            set
            {
                m_MattersCount = value;
                if (mattersText != null)
                {
                    mattersText.text = value.ToString();
                }
                if (mattersIcon!=null)
                {
                    mattersIcon.SetActive(value>0?true:false);
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="info"></param>
        public virtual void Init(CalendarDateInfo info, CalendarConfig calendarConfig)
        {
            if (info == null) return;
            if (calendarConfig == null) return;

            m_Info = info;
            m_CalendarConfig = calendarConfig;

            dateText.text = info.date.Day.ToString();
            dateText.color = workdayColor;
            mark.text = string.Empty;

            if (info.mark != null)
            {
                mark.text = info.mark.mark;
            }
            if (calendarConfig.weekend!=null)
            {
                foreach (var weekend in calendarConfig.weekend)
                {
                    if (info.date.DayOfWeek == weekend)
                    {
                        dateText.color = weekendColor;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 数据信息
        /// </summary>
        public CalendarDateInfo Info
        {
            get
            {
                return m_Info;
            }
        }
    }

    [Serializable]
    /// <summary>
    /// 日历日期
    /// </summary>
    public class CalendarDateInfo
    {
        public DateTime date;
        public DateMark mark;

        public CalendarDateInfo(DateTime date, DateMark mark)
        {
            this.date = date;
            this.mark = mark;
        }
    }
}