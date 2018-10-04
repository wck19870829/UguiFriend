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
        [SerializeField] protected CalendarDateInfo m_Info;
        [SerializeField] protected Color workdayColor = Color.black;
        [SerializeField] protected Color weekendColor = Color.black;
        CanvasGroup m_CanvasGroup;

        public Action<CalendarDate> OnClickEvent;

        protected virtual void Awake()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
            if (m_CanvasGroup == null)
                m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();
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