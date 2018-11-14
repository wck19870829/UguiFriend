using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 查找交互物体
    /// </summary>
    public class UguiSelectableFinder : UIBehaviour
    {
        protected static readonly Dictionary<FindDirection, Vector2> dirValueDict;

        [Range(0,180)]
        [SerializeField] protected float m_SearchAngle = 120f;
        [SerializeField] protected float m_SearchDist = -1f;
        [SerializeField] protected List<RectTransform> m_Children;
        protected RectTransform m_CurrentTarget;
        protected Dictionary<RectTransform, float> m_ChildrenScoreDict;

        static UguiSelectableFinder()
        {
            dirValueDict = new Dictionary<FindDirection, Vector2>()
            {
                {FindDirection.Up,Vector2.up },
                {FindDirection.Down,Vector2.down },
                {FindDirection.Left,Vector2.left },
                {FindDirection.Right,Vector2.right }
            };
        }

        protected UguiSelectableFinder()
        {
            m_Children = new List<RectTransform>();
            m_ChildrenScoreDict = new Dictionary<RectTransform, float>();
        }

        /// <summary>
        /// 查找下一个
        /// </summary>
        /// <param name="direction">查找方向</param>
        /// <returns></returns>
        public RectTransform FindNext(FindDirection direction)
        {
            if (m_Children.Count > 0)
            {
                var rectTransform = transform as RectTransform;
                var origin = m_CurrentTarget.position;
                var findDirValue = dirValueDict[direction];
                var searchDotLimit = Vector2.Dot(Vector2.up,UguiMathf.Rotation(Vector2.up, m_SearchAngle*0.5f));
                var searchDistLimit = m_SearchDist < 0 
                                    ? float.MaxValue
                                    : m_SearchDist;
                m_ChildrenScoreDict.Clear();
                foreach (RectTransform child in m_Children)
                {
                    var dir = child.position - origin;
                    var dot = Vector2.Dot(dir, findDirValue);
                    var score = (dot >= searchDotLimit)&&(dir.magnitude<= searchDistLimit)
                                ? dot / dir.sqrMagnitude
                                : float.MinValue;

                    m_ChildrenScoreDict.Add(child, score);
                }

                m_Children.Sort((a, b) =>
                {
                    var scoreA = m_ChildrenScoreDict[a];
                    var scoreB = m_ChildrenScoreDict[b];

                    if (scoreA == scoreB) return 0;
                    return scoreA > scoreB ? -1 : 1;
                });

                var first = m_Children[0];
                if (m_CurrentTarget != first)
                {
                    var firstScore = m_ChildrenScoreDict[first];
                    if (firstScore >= searchDotLimit && firstScore<= searchDistLimit)
                    {
                        return first;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 添加物体
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(RectTransform child)
        {
            if (!m_Children.Contains(child))
            {
                m_Children.Add(child);
            }
        }

        /// <summary>
        /// 移除物体
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool RemoveChild(RectTransform child)
        {
            m_ChildrenScoreDict.Remove(child);

            return m_Children.Remove(child);
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

        /// <summary>
        /// 方向
        /// </summary>
        public enum FindDirection
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}