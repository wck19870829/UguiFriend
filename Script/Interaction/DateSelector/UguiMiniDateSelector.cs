using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 微型日期拾取器
    /// </summary>
    public class UguiMiniDateSelector : MonoBehaviour
    {
        [SerializeField] protected Button prevYearButton;
        [SerializeField] protected Button nextYearButton;
        [SerializeField] protected Button prevMonthButton;
        [SerializeField] protected Button nextMonthButton;
        [SerializeField] protected Button prevDayButton;
        [SerializeField] protected Button nextDayButton;

        protected virtual void Awake()
        {
            if (prevYearButton != null)
                prevYearButton.onClick.AddListener(OnPrevYearClick);
            if (nextYearButton != null)
                nextYearButton.onClick.AddListener(OnNextYearClick);
            if (prevMonthButton != null)
                prevMonthButton.onClick.AddListener(OnPrevMonthClick);
            if (nextMonthButton != null)
                nextMonthButton.onClick.AddListener(OnNextMonthClick);
            if (prevDayButton != null)
                prevDayButton.onClick.AddListener(OnPrevDayClick);
            if (nextDayButton != null)
                nextDayButton.onClick.AddListener(OnNextDayClick);
        }

        protected virtual void OnPrevYearClick()
        {

        }

        protected virtual void OnNextYearClick()
        {

        }

        protected virtual void OnPrevMonthClick()
        {

        }

        protected virtual void OnNextMonthClick()
        {

        }

        protected virtual void OnPrevDayClick()
        {

        }

        protected virtual void OnNextDayClick()
        {

        }
    }
}