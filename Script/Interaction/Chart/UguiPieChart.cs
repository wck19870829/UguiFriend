using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 饼图
    /// </summary>
    public class UguiPieChart : UguiChart
    {
        [SerializeField] protected internal int fillOrigin;
        [SerializeField] protected float innerDiameter=0;               //内径
        [SerializeField] protected float externalDiameter=10;           //外径

        public override void Rebuild()
        {
            var data = m_Data as UguiPieChartData;


            isDirty = false;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            innerDiameter= Mathf.Max(innerDiameter, 0);
            externalDiameter = Mathf.Max(innerDiameter, externalDiameter);
        }
#endif
    }

    [System.Serializable]
    public class UguiPieChartData : UguiChartData
    {
        public List<UguiPieChartItemData> itemDataList;

        public UguiPieChartData()
        {
            itemDataList = new List<UguiPieChartItemData>();
        }
    }
}