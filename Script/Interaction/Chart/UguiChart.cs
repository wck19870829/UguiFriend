using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 图表基类
    /// </summary>
    public abstract class UguiChart : UIBehaviour
    {
        public Text valueTextPrefab;
        public Text nameTextPrefab;
        [SerializeField] protected UguiChartData m_Data;
        protected bool isDirty;
        protected RectTransform chartContent;
        protected RectTransform valueTextContent;

        protected override void Awake()
        {
            base.Awake();

            chartContent = UguiTools.AddChild<RectTransform>("Chart",transform);
            valueTextContent = UguiTools.AddChild<RectTransform>("ValueText", transform);

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
    public class UguiChartData
    {
        
    }
}