﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 网格排序
    /// </summary>
    public class UguiGridLayoutGroup : UguiLayoutGroup
    {
        [SerializeField] protected Corner m_StartCorner;
        [SerializeField] protected Axis m_StartAxis;
        [SerializeField] protected Vector2 m_CellSize;
        [SerializeField] protected Vector2 m_Spacing;
        [SerializeField] protected Constraint m_Constraint;
        [SerializeField] protected int m_FlexibleCount;

        protected UguiGridLayoutGroup()
        {
            m_CellSize = new Vector2(100,100);
            m_Spacing = new Vector2();
        }

        protected override void ProcessItemAfterCreated(UguiObject obj)
        {
            obj.RectTransform.sizeDelta = m_CellSize;
        }

        public override void UpdateChildrenLocalPosition()
        {
            var flexibleCount = int.MaxValue;
            if (m_Constraint == Constraint.FixedColumnCount || m_Constraint == Constraint.FixedRowCount)
                flexibleCount = m_FlexibleCount;
            if(flexibleCount<=0)
                flexibleCount = int.MaxValue;

            var len = m_ChildDataList.Count;
            var origin = new Vector2();

            if (m_StartAxis==Axis.Horizontal)
            {
                //横向排序
                for (var i=0;i< len; i++)
                {
                    var hIndex = i % flexibleCount;
                    var vIndex = i / flexibleCount;
                    var localPos = new Vector2(
                                    origin.x+hIndex*m_CellSize.x+hIndex*m_Spacing.x,
                                    origin.y + vIndex * m_CellSize.y + vIndex * m_Spacing.y);
                    m_ChildrenLocalPositionList.Add(localPos);
                }
            }
            else
            {
                //纵向排序
                for (var i = 0; i < len; i++)
                {
                    var hIndex = i / flexibleCount;
                    var vIndex = i % flexibleCount;
                    var localPos = new Vector2(
                                    origin.x + hIndex * m_CellSize.x + hIndex * m_Spacing.x,
                                    origin.y + vIndex * m_CellSize.y + vIndex * m_Spacing.y);
                    m_ChildrenLocalPositionList.Add(localPos);
                }
            }
        }

        public Vector2 Spacing { get { return m_Spacing; } }

        public Vector2 CellSize { get { return m_CellSize; } }
    }
}