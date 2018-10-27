using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(ScrollRect))]
    /// <summary>
    /// 子元素居中
    /// </summary>
    public class UguiCenterOnChild : MonoBehaviour, IEndDragHandler, IBeginDragHandler,IScrollHandler
    {
        public float velocityThreshold = 300;             //速度临界值，小于此值才会开始居中操作
        protected ScrollRect m_ScrollRect;
        protected Mask mask;
        protected Vector3[] maskCorners = new Vector3[4];
        protected ActionPeriod actionPeriod;
        protected Vector3 contentPos;
        ScrollRect.MovementType cacheMovementType;
        Transform centerTarget;

        public Action<Transform> OnCenter;

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void OnEnable()
        {
            centerTarget = null;
            CenterOn(true);
        }

        void Update()
        {
            if (m_ScrollRect == null || m_ScrollRect.content == null) return;

            if (actionPeriod == ActionPeriod.Checking)
            {
                if (m_ScrollRect.velocity.magnitude < velocityThreshold)
                {
                    CenterOn(false);
                }
            }
            else if (actionPeriod == ActionPeriod.PositionAlignment)
            {
                var decelerationRate = (m_ScrollRect.decelerationRate == 0 || !m_ScrollRect.inertia)?
                                        1f:
                                        m_ScrollRect.decelerationRate;
                var pos = Vector3.Lerp(m_ScrollRect.content.position, contentPos, decelerationRate);
                m_ScrollRect.content.position = pos;
            }

            var closest = GetClosest();
            if (centerTarget != closest)
            {
                centerTarget = closest;
                if (centerTarget != null)
                {
                    if (OnCenter != null)
                    {
                        OnCenter.Invoke(centerTarget);
                    }
                }
            }
        }

        protected virtual void OnDestroy()
        {
            m_ScrollRect.movementType = cacheMovementType;
        }

        void Init()
        {
            if (m_ScrollRect == null)
                m_ScrollRect = GetComponent<ScrollRect>();
            if (m_ScrollRect == null)
                throw new Exception("Scroll rect is null.");
            if (m_ScrollRect.content == null)
                throw new Exception("Scroll rect content is null!");
            mask = m_ScrollRect.GetComponentInChildren<Mask>();
            if (mask == null)
                throw new Exception("Mask is null.");

            cacheMovementType = m_ScrollRect.movementType;
            m_ScrollRect.movementType = ScrollRect.MovementType.Unrestricted;
        }

        /// <summary>
        /// 居中
        /// </summary>
        /// <param name="target"></param>
        /// <param name="isImmediate">立即居中</param>
        public virtual void CenterOn(Transform target,bool isImmediate)
        {
            if (target == null) return;

            Init();

            actionPeriod = ActionPeriod.PositionAlignment;
            if (!enabled) enabled = true;
            m_ScrollRect.StopMovement();
            mask.rectTransform.GetWorldCorners(maskCorners);
            var center = Vector3.Lerp(maskCorners[0], maskCorners[2], 0.5f);
            var offset = center - target.position;
            contentPos = m_ScrollRect.content.position;
            contentPos += offset;

            if (isImmediate)
            {
                m_ScrollRect.content.position = contentPos;
            }
        }

        /// <summary>
        /// 居中
        /// </summary>
        /// <param name="isImmediate"></param>
        public virtual void CenterOn(bool isImmediate)
        {
            var closest = GetClosest();
            CenterOn(closest,isImmediate);
        }

        /// <summary>
        /// 获取距离中心点最近的元素
        /// </summary>
        /// <returns></returns>
        public Transform GetClosest()
        {
            Init();

            mask.rectTransform.GetWorldCorners(maskCorners);
            var center = Vector3.Lerp(maskCorners[0], maskCorners[2], 0.5f);
            Transform closest = null;
            var closestDist = float.MaxValue;
            for (var i = 0; i < m_ScrollRect.content.childCount; i++)
            {
                var dist = Vector3.Distance(m_ScrollRect.content.GetChild(i).position, center);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = m_ScrollRect.content.GetChild(i);
                }
            }

            return closest;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            actionPeriod = ActionPeriod.Checking;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            actionPeriod = ActionPeriod.None;
        }

        public void OnScroll(PointerEventData eventData)
        {

        }

        /// <summary>
        /// 滑动区域
        /// </summary>
        public ScrollRect ScrollRect
        {
            get
            {
                if (m_ScrollRect == null)
                    m_ScrollRect = GetComponent<ScrollRect>();

                return m_ScrollRect;
            }
        }

        /// <summary>
        /// 动作周期
        /// </summary>
        protected enum ActionPeriod
        {
            None,
            Checking,
            PositionAlignment
        }
    }
}