using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(Button))]
    /// <summary>
    /// 预设颜色
    /// </summary>
    public class UguiColorPresetSelectorItem : MonoBehaviour
    {
        [SerializeField] protected GameObject selectIcon;
        [SerializeField] protected RawImage colorImage;
        protected Color m_Color;
        protected Button button;
        protected bool m_IsSelect;

        public Action<UguiColorPresetSelectorItem> OnClick;

        protected virtual void Awake()
        {
            button = GetComponent<Button>();
            if (button == null)
                button = gameObject.AddComponent<Button>();
            button.onClick.AddListener(OnButtonClick);

            Color = m_Color;
            IsSelect = false;
        }

        protected virtual void OnButtonClick()
        {
            if (OnClick!=null)
            {
                OnClick.Invoke(this);
            }
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
                if (colorImage!=null)
                {
                    colorImage.color = value;
                }
            }
        }

        public virtual bool IsSelect
        {
            get
            {
                return m_IsSelect;
            }
            set
            {
                m_IsSelect = value;
                if (selectIcon!=null)
                {
                    selectIcon.SetActive(value);
                }
            }
        }
    }
}