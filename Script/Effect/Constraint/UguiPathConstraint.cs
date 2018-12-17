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
        [SerializeField]protected List<Vector3> m_PointList;
        protected UguiMathf.Bezier m_Bezier;
    }
}