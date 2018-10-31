using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
    public abstract class UguiChartItem : UIBehaviour
    {
        protected UguiChartItemData m_Data;
        [SerializeField] protected Image graph;

        protected override void Awake()
        {
            base.Awake();
            if (graph == null)
                throw new Exception("Graph is null.");
        }
    }

    public abstract class UguiChartItemData
    {

    }
}