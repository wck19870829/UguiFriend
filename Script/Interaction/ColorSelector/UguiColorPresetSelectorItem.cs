using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(Button))]
    /// <summary>
    /// 预设颜色
    /// </summary>
    public class UguiColorPresetSelectorItem : MonoBehaviour
    {
        protected Color m_Color;
        protected Button button;

        protected virtual void Start()
        {
            button = GetComponent<Button>();
            if (button == null)
                button = gameObject.AddComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
        }

        protected virtual void OnButtonClick()
        {

        }

        public virtual Color Color
        {
            get
            {
                return m_Color;
            }
            set
            {
                m_Color = value;
            }
        }
    }
}