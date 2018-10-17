using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 颜色预设拾取器
    /// </summary>
    public class UguiColorPresetSelector : MonoBehaviour
    {
        [SerializeField] protected UguiColorPresetSelectorItem selectColorItem;
        [SerializeField] protected UguiColorPresetSelectorItem itemPrefab;
        [SerializeField] protected GridLayoutGroup content;
        [SerializeField] protected GameObject presetPanel;

        [Header("- Preset colors")]
        [SerializeField] protected Color[] presetColors;
        protected Color m_SelectColor;

        public Action<Color> OnSelectColor;

        protected virtual void OnEnable()
        {
            if (presetPanel != null)
            {
                presetPanel.SetActive(false);
            }
        }

        protected virtual void Start()
        {
            if (itemPrefab == null)
                throw new Exception("Item prefab is null.");
            if (selectColorItem == null)
                throw new Exception("Select color is null.");
            if (content == null)
                throw new Exception("Content is null.");

            selectColorItem.OnClick += OnSelectColorClick;
            Init(presetColors);
        }

        public virtual void Init(Color[] colors)
        {
            if (colors == null) return;

            UguiTools.DestroyChildren(content.gameObject);
            presetColors = colors;
            if (itemPrefab != null)
            {
                foreach (var color in presetColors)
                {
                    var item=UguiTools.AddChild<UguiColorPresetSelectorItem>(itemPrefab,content.transform);
                    item.Color = color;
                    item.OnClick += OnItemClick;
                }
            }
        }

        public virtual Color SelectColor
        {
            get
            {
                return m_SelectColor;
            }
            set
            {
                m_SelectColor = value;
                var items = content.GetComponentsInChildren<UguiColorPresetSelectorItem>();
                foreach (var item in items)
                {
                    item.IsSelect = (item.Color == value) ? true : false;
                }
                if (selectColorItem != null)
                {
                    selectColorItem.Color = value;
                }
                if (OnSelectColor != null)
                {
                    OnSelectColor.Invoke(value);
                }
            }
        }

        private void OnItemClick(UguiColorPresetSelectorItem item)
        {
            ClosePresetPanel();
        }

        private void OnSelectColorClick(UguiColorPresetSelectorItem item)
        {
            OpenPresetPanel();
        }

        protected virtual void OpenPresetPanel()
        {
            if (presetPanel != null)
            {
                presetPanel.SetActive(true);
            }
        }

        protected virtual void ClosePresetPanel()
        {
            if (presetPanel != null)
            {
                presetPanel.SetActive(false);
            }
        }
    }
}