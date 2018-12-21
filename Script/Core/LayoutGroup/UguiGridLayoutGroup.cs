using UnityEngine;
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
        [SerializeField] protected int m_ConstraintCount;

        public override void CalculateLayoutInputVertical()
        {

        }

        public override void SetLayoutHorizontal()
        {

        }

        public override void SetLayoutVertical()
        {

        }

        protected override void UpdateChildrenLocalPosition()
        {
            switch (m_StartAxis)
            {
                case Axis.Horizontal:

                    break;

                case Axis.Vertical:

                    break;
            }

            switch (m_Constraint)
            {
                case Constraint.FixedColumnCount:

                    break;

                case Constraint.FixedRowCount:

                    break;

                case Constraint.Flexible:

                    break;
            }

            var fixedCount = int.MaxValue;
            m_ChildrenLocalPositionList.Clear();
            for (var i=0;i<m_childrenDataList.Count;i++)
            {

            }
        }

        public enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }

        public enum Corner
        {
            UpperLeft = 0,
            UpperRight = 1,
            LowerLeft = 2,
            LowerRight = 3
        }

        public enum Constraint
        {
            Flexible = 0,
            FixedColumnCount = 1,
            FixedRowCount = 2
        }
    }
}