using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 路径约束
    /// </summary>
    public class UguiPathConstraint : UIBehaviour
    {
        [SerializeField]protected List<Vector3> m_Points;
        protected UguiMathf.Bezier m_Bezier;
        protected bool m_Dirty;

        protected UguiPathConstraint()
        {
            m_Bezier = new UguiMathf.Bezier();
        }

        private void Update()
        {
            if (m_Dirty)
            {
                m_Dirty = false;
                m_Bezier.Set(m_Points);
            }
        }

        public void Set(List<Vector3>points)
        {
            m_Points = points;
            m_Dirty = true;
        }
    }
}