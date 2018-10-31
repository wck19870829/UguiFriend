using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 饼图子元素
    /// </summary>
    public class UguiPieChartItem : UguiChartItem
    {
        protected override void Awake()
        {
            base.Awake();

            var pie = GetComponentInParent<UguiPieChart>();
            if (pie == null)
                throw new Exception("Pie chart is null");

            graph.type = Image.Type.Filled;
            graph.fillMethod = Image.FillMethod.Radial360;
            graph.fillOrigin = pie.fillOrigin;
        }

        public virtual void Set(float fillAmountFrom,float fillAmountTo,Color color)
        {

        }
    }

    [System.Serializable]
    /// <summary>
    /// 饼图子元素数据
    /// </summary>
    public class UguiPieChartItemData: UguiChartItemData
    {

    }
}