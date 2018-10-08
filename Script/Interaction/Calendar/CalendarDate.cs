using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
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
        [SerializeField] protected CalendarDateInfo m_Info;
        [SerializeField] protected Color workdayColor = Color.black;
        [SerializeField] protected Color weekendColor = Color.black;
        protected CanvasGroup m_CanvasGroup;
        protected int m_MattersCount;
        protected bool m_IsSelect;
        protected bool m_IsToday;

        public Action<CalendarDate> OnClickEvent;

        protected virtual void Awake()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
            if (m_CanvasGroup == null)
                m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();
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
        public virtual void SetActiveState()
        {
            m_CanvasGroup.alpha = 1;
        }

        /// <summary>
        /// 设置禁用
        /// </summary>
        public virtual void SetInactiveState()
        {
            m_CanvasGroup.alpha = 0;
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
        public virtual void Init(CalendarDateInfo info)
        {
            if (info == null) return;

            m_Info = info;
            dateText.text = info.date.Day.ToString();
            if (mark && info.mark != null)
                mark.text = info.mark.mark;
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

    /// <summary>
    /// 日历日期
    /// </summary>
    public class CalendarDateInfo
    {
        public DateTime date;
        public DateMarkBase mark;

        public CalendarDateInfo(DateTime date, DateMarkBase mark = null)
        {
            this.date = date;
            this.mark = mark;
        }
    }
}