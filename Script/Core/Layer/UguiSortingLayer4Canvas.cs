using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 画布层级控制
    /// </summary>
    public class UguiSortingLayer4Canvas : UguiSortingLayer<Canvas>
    {
        protected override void Sort(List<Canvas> children)
        {
            children.Sort((a,b) =>
            {
                return a.sortingOrder-b.sortingOrder;
            });
            foreach (var child in children)
            {
                child.overrideSorting = true;
                child.sortingOrder = s_GlobalSortngLayer;
                child.sortingLayerID = m_SortingLayerID;
                s_GlobalSortngLayer++;
            }
        }
    }
}