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
        static readonly int defaultRow = 5;
        static readonly int defaultColumn = 5;

        [SerializeField] protected UguiColorPresetSelectorItem selectColorItem;
        [SerializeField] protected UguiColorPresetSelectorItem itemPrefab;
        [SerializeField] protected GridLayoutGroup content;
        [SerializeField] protected GameObject presetPanel;

        [Header("- Select color")]
        [SerializeField] protected Color m_SelectColor = Color.white;

        [Header("- Preset colors")]
        [SerializeField] protected Color[] presetColors = GetBeautifulPresetColors(defaultRow * defaultColumn);

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
            SetPresetColors(presetColors);
            SelectColor = m_SelectColor;
        }

        /// <summary>
        /// 设置预设颜色
        /// </summary>
        /// <param name="colors"></param>
        public virtual void SetPresetColors(Color[] colors)
        {
            if (colors == null|| itemPrefab == null) return;

            UguiTools.DestroyChildren(content.gameObject);
            presetColors = colors;
            foreach (var color in presetColors)
            {
                var item = UguiTools.AddChild<UguiColorPresetSelectorItem>(itemPrefab, content.transform);
                item.Color = color;
                item.OnClick += OnItemClick;
            }
        }

        /// <summary>
        /// 当前选择的颜色
        /// </summary>
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
            SelectColor = item.Color;
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

        #region 静态方法

        /// <summary>
        /// 获得色彩协调的预设颜色
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Color[] GetBeautifulPresetColors(int count)
        {
            var colors = new Color[count];

            return colors;
        }

        #endregion
    }
}