using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// Renderer排序
    /// 适用于ParticleSystem,SpriteRenderer,LineRenderer,MeshRenderer等
    /// </summary>
    public class UguiSortingLayer4Renderer : UguiSortingLayer<Renderer>
    {
        protected override void Sort(List<Renderer> children)
        {
            children.Sort((a,b)=> 
            {
                return a.sortingOrder - b.sortingOrder;
            });
            foreach (var child in children)
            {
                child.sortingOrder = s_GlobalSortngLayer;
                child.sortingLayerID = m_SortingLayerID;
                s_GlobalSortngLayer++;
            }
        }
    }
}