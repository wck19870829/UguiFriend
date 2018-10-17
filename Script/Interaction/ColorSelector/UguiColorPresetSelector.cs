using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 颜色预设拾取器
    /// </summary>
    public class UguiColorPresetSelector : MonoBehaviour
    {
        [SerializeField] protected Color[] presetColors;
        [SerializeField] protected GameObject content;

        protected virtual void Start()
        {
            Init(presetColors);
        }

        public virtual void Init(Color[] colors)
        {
            if (colors == null) return;

            presetColors = colors;
            foreach (var color in presetColors)
            {

            }
        }
    }
}