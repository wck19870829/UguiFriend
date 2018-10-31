using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 图表基类
    /// </summary>
    public abstract class UguiChart : UIBehaviour
    {
        [SerializeField] protected UguiChartData m_Data;
        protected bool isDirty;
        protected RectTransform chartContent;
        protected RectTransform valueTextContent;

        protected override void Awake()
        {
            base.Awake();

            chartContent = new RectTransform();
            chartContent.name = "Chart";
            chartContent.SetParent(transform);
            valueTextContent = new RectTransform();
            valueTextContent.name = "ValueText";
            valueTextContent.SetParent(transform);

            isDirty = true;
        }

        protected virtual void Update()
        {
            if (isDirty)
            {
                Rebuild();
                isDirty = false;
            }
        }

        public abstract void Rebuild();
    }

    /// <summary>
    /// 图标数据基类
    /// </summary>
    public abstract class UguiChartData
    {

    }
}