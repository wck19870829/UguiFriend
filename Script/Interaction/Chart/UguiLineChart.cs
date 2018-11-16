using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 折线图
    /// </summary>
    public class UguiLineChart : UguiChart
    {
        public GameObject keyPrefab;
        float m_Scale;
        UguiLine[] m_Lines;

        public override void Rebuild()
        {
            var rectTransform = this.transform as RectTransform;
            var line = new UguiLine();
        }

        /// <summary>
        /// 缩放
        /// </summary>
        public float Scale
        {
            get
            {
                return m_Scale;
            }
            set
            {
                m_Scale = value;
                Rebuild();
            }
        }
    }

    public class UguiLineChartData : UguiChartData
    {

    }
}