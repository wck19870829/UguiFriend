using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    [ExecuteInEditMode]
    /// <summary>
    /// 可变长宽表格
    /// </summary>
    public class UguiTableLayoutGroup : UguiLayoutGroup
    {
        [SerializeField] protected Corner m_StartCorner;
        [SerializeField] protected Axis m_StartAxis;
        [SerializeField] protected Vector2 m_Spacing;
        [SerializeField] protected int m_FlexibleCount=5;

        public override void CalculateLayoutInputVertical()
        {

        }

        public override void SetLayoutHorizontal()
        {

        }

        public override void SetLayoutVertical()
        {

        }

        public override void UpdateChildrenLocalPosition()
        {
            var len = m_ChildrenDataList.Count;
            var origin = new Vector2();

            var flexibleCount = Math.Max(m_FlexibleCount, 1);
            var childCount = transform.childCount;
            if (m_StartAxis == Axis.Horizontal)
            {
                //横向排序
                var vCount = childCount / flexibleCount;
                for (var i = 0; i < flexibleCount; i++)
                {
                    var hIndex = i % flexibleCount;
                    var vIndex = i / flexibleCount;
                    var obj = transform.GetChild(i) as RectTransform;
                    var bounds = UguiMathf.GetGlobalBoundsIncludeChildren(obj);
                }
            }
            else
            {
                //纵向排序

            }

            //if (m_StartAxis == Axis.Horizontal)
            //{
            //    //横向排序
            //    for (var i = 0; i < len; i++)
            //    {
            //        var hIndex = i % flexibleCount;
            //        var vIndex = i / flexibleCount;
            //        var localPos = new Vector2(
            //                        origin.x + hIndex * m_CellSize.x + hIndex * m_Spacing.x,
            //                        origin.y + vIndex * m_CellSize.y + vIndex * m_Spacing.y);
            //        m_ChildrenLocalPositionList.Add(localPos);
            //    }
            //}
            //else
            //{
            //    //纵向排序
            //    for (var i = 0; i < len; i++)
            //    {
            //        var hIndex = i / flexibleCount;
            //        var vIndex = i % flexibleCount;
            //        var localPos = new Vector2(
            //                        origin.x + hIndex * m_CellSize.x + hIndex * m_Spacing.x,
            //                        origin.y + vIndex * m_CellSize.y + vIndex * m_Spacing.y);
            //        m_ChildrenLocalPositionList.Add(localPos);
            //    }
            //}
        }
    }
}