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
        [Range(0.01f,1f)]
        [SerializeField] protected float m_Speed;
        [SerializeField] protected Image m_Bar;
        [SerializeField] protected Text m_DetailText;
        protected ProgressStyle m_ProgressStyle;
        protected string unitStr;

        protected override void Awake()
        {
            base.Awake();

            if (m_Bar == null)
                throw new Exception("进度条不能为null.");
            m_Bar.type = Image.Type.Filled;
        }

        protected virtual void Update()
        {
            switch (m_ProgressStyle)
            {
                case ProgressStyle.Step:

                    break;

                case ProgressStyle.Percent:

                    break;
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
        }

        /// <summary>
        /// 按步骤更新
        /// </summary>
        /// <param name="current"></param>
        /// <param name="total"></param>
        /// <param name="unit">单位字符</param>
        public virtual void UpdateInfo(int current,int total,string unit="")
        {
            m_ProgressStyle = ProgressStyle.Step;
            unitStr = unit;

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