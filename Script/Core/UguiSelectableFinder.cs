using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 查找交互物体
    /// </summary>
    public class UguiSelectableFinder : UIBehaviour
    {
        [SerializeField] protected RectTransform m_Children;
        protected RectTransform m_CurrentTarget;

        /// <summary>
        /// 查找下一个
        /// </summary>
        /// <param name="direction">查找方向</param>
        /// <returns></returns>
        public RectTransform FindNext(FindDirection direction)
        {


            return null;
        }

        /// <summary>
        /// 设置当前目标
        /// </summary>
        public RectTransform CurrentTarget
        {
            get
            {
                return m_CurrentTarget;
            }
            set
            {
                m_CurrentTarget = value;
            }
        }

        public enum FindDirection
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}