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
    public class UguiCenterOnChild : MonoBehaviour, IEndDragHandler, IBeginDragHandler
    {
        public Vector2 v2;
        public float velocityThreshold = 300;             //速度临界值，小于此值才会开始居中操作
        protected ScrollRect scrollRect;
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
            actionPeriod = ActionPeriod.None;
        }

        void Update()
        {
            if (scrollRect == null || scrollRect.content == null) return;

            if (actionPeriod == ActionPeriod.Checking)
            {
                if (scrollRect.velocity.magnitude < velocityThreshold)
                {
                    CenterOn();
                }
            }
            else if (actionPeriod == ActionPeriod.PositionAlignment)
            {
                var decelerationRate = (scrollRect.decelerationRate == 0 || !scrollRect.inertia)
                    ? 1f
                    : scrollRect.decelerationRate;
                var pos = Vector3.Lerp(scrollRect.content.position, contentPos, decelerationRate);
                scrollRect.content.position = pos;
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
            scrollRect.movementType = cacheMovementType;
        }

        void Init()
        {
            if (scrollRect == null)
                scrollRect = GetComponent<ScrollRect>();
            if (scrollRect == null)
                throw new Exception("Scroll rect is null.");
            if (scrollRect.content == null)
                throw new Exception("Scroll rect content is null!");
            mask = scrollRect.GetComponentInChildren<Mask>();
            if (mask == null)
                throw new Exception("Mask is null.");

            cacheMovementType = scrollRect.movementType;
            scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
        }

        /// <summary>
        /// 居中
        /// </summary>
        /// <param name="target"></param>
        public virtual void CenterOn(Transform target)
        {
            if (target == null) return;

            Init();

            actionPeriod = ActionPeriod.PositionAlignment;
            if (!enabled) enabled = true;
            scrollRect.StopMovement();
            mask.rectTransform.GetWorldCorners(maskCorners);
            var center = Vector3.Lerp(maskCorners[0], maskCorners[2], 0.5f);
            var offset = center - target.position;
            contentPos = scrollRect.content.position;
            contentPos += offset;

            if (!gameObject.activeSelf)
            {
                scrollRect.content.position = contentPos;
            }
        }

        /// <summary>
        /// 居中
        /// </summary>
        public virtual void CenterOn()
        {
            var closest = GetClosest();
            CenterOn(closest);
        }

        /// <summary>
        /// 获取里中心点最近的元素
        /// </summary>
        /// <returns></returns>
        public Transform GetClosest()
        {
            Init();

            mask.rectTransform.GetWorldCorners(maskCorners);
            var center = Vector3.Lerp(maskCorners[0], maskCorners[2], 0.5f);
            Transform closest = null;
            var closestDist = float.MaxValue;
            for (var i = 0; i < scrollRect.content.childCount; i++)
            {
                var dist = Vector3.Distance(scrollRect.content.GetChild(i).position, center);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = scrollRect.content.GetChild(i);
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

        public enum ActionPeriod
        {
            None,
            Checking,
            PositionAlignment
        }
    }
}