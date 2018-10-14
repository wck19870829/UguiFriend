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

        public virtual void Set(int num)
        {
            if (dateText != null)
            {
                dateText.text = num.ToString();
            }
        }
    }
}