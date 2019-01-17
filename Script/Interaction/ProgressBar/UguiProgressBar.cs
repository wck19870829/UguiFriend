using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 进度条
    /// </summary>
    public class UguiProgressBar : UIBehaviour
    {
        [SerializeField] [Range(0.01f, 1f)] protected float m_Speed=0.1f;
        [SerializeField] protected Image m_Bar;
        [SerializeField] protected Text m_DetailText;
        protected ProgressStyle m_ProgressStyle;
        protected float m_CurrentValue;
        protected float m_TotalValue;
        protected float m_AnimCurrentValue;
        protected string unitStr;

        public Action OnComplete;

        protected override void Awake()
        {
            base.Awake();

            if (m_Bar == null)
                throw new Exception("进度条不能为null.");
            m_Bar.type = Image.Type.Filled;
        }

        protected virtual void Update()
        {
            var cacheValue = m_AnimCurrentValue;
            m_AnimCurrentValue = Mathf.Lerp(m_AnimCurrentValue, m_CurrentValue, m_Speed);
            if (m_CurrentValue - m_AnimCurrentValue < 0.1f) m_AnimCurrentValue = m_CurrentValue;

            if (cacheValue!= m_AnimCurrentValue)
            {
                switch (m_ProgressStyle)
                {
                    case ProgressStyle.Step:
                        UpdateBar(m_AnimCurrentValue / m_TotalValue);
                        UpdateText(m_AnimCurrentValue.ToString("0") + unitStr + "/" + m_TotalValue + unitStr);
                        if (m_AnimCurrentValue == m_TotalValue)
                        {
                            if (OnComplete != null)
                            {
                                OnComplete.Invoke();
                            }
                        }
                        break;

                    case ProgressStyle.Percent:
                        UpdateBar(m_AnimCurrentValue / 100);
                        UpdateText(m_AnimCurrentValue.ToString("0.0"));
                        if (m_AnimCurrentValue == 100)
                        {
                            if (OnComplete != null)
                            {
                                OnComplete.Invoke();
                            }
                        }
                        break;
                }
            }
        }

        protected virtual void UpdateBar(float percent)
        {
            if (m_Bar != null)
            {
                m_Bar.fillAmount = percent;
            }
        }

        protected virtual void UpdateText(string text)
        {
            if (m_DetailText != null)
            {
                m_DetailText.text = text;
            }
        }

        /// <summary>
        /// 百分比形式更新
        /// </summary>
        /// <param name="percent">当前进度百分比,范围(0-100)</param>
        public virtual void UpdateInfo(float percent)
        {
            m_ProgressStyle = ProgressStyle.Percent;
            unitStr = "%";
            m_CurrentValue = percent;
        }

        /// <summary>
        /// 按步骤更新
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="totalValue"></param>
        /// <param name="unit">单位字符</param>
        public virtual void UpdateInfo(int currentValue,int totalValue,string unit="")
        {
            m_ProgressStyle = ProgressStyle.Step;
            unitStr = unit;
            m_CurrentValue = currentValue;
            m_TotalValue = totalValue;
        }

        /// <summary>
        /// 进度显示类型
        /// </summary>
        protected enum ProgressStyle
        {
            None,
            Percent,
            Step
        }
    }
}