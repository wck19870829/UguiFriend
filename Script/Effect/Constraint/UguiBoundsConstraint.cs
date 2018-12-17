using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 边界约束
    /// </summary>
    public class UguiBoundsConstraint : UIBehaviour
    {
        [SerializeField]protected MoveEffect m_MoveEffect;

        public MoveEffect Effect
        {
            get
            {
                return m_MoveEffect;
            }
            set
            {
                m_MoveEffect = value;
            }
        }

        /// <summary>
        /// 效果
        /// </summary>
        public enum MoveEffect
        {
            None,
            Momentum,
            MomentumAndSpring,
        }
    }
}