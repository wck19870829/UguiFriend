using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 日期拾取器-日期元素（年，月，日）
    /// </summary>
    public class UguiDateSelectorDate : MonoBehaviour
    {
        [SerializeField]protected Text dateText;
        int m_Num;

        protected virtual void Awake()
        {
            Num = 0;
        }

        public virtual int Num
        {
            get
            {
                return m_Num;
            }
            set
            {
                m_Num = value;
                if (dateText != null)
                {
                    dateText.text = m_Num.ToString();
                }
            }
        }
    }
}