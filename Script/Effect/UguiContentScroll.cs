using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(ScrollRect))]
    /// <summary>
    /// 滚动
    /// </summary>
    public class UguiContentScroll : UIBehaviour,IBeginDragHandler,IEndDragHandler
    {
        public float speed = 1;
        public bool stopScrollWhenDrag=true;
        [SerializeField] protected float repeatInterval=5;
        [SerializeField] protected Direction m_Direction;
        [SerializeField] protected bool m_Invert;
        protected ScrollRect scrollRect;
        protected Mask m_Mask;
        protected bool isMoving;
        Vector3[] maskCorners;
        Vector3[] contentCorners;

        protected UguiContentScroll()
        {
            maskCorners = new Vector3[4];
            contentCorners = new Vector3[4];
        }

        protected override void Awake()
        {
            base.Awake();

            scrollRect = GetComponent<ScrollRect>();
            if (scrollRect == null)
                throw new Exception("Scroll rect is null.");
            m_Mask = scrollRect.viewport.GetComponent<Mask>();
            if(m_Mask==null)
                throw new Exception("Mask is null.");
            isMoving = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            isMoving = true;
        }

        protected virtual void Update()
        {
            Scroll();
        }

        protected virtual void Scroll()
        {
            if (scrollRect == null|| scrollRect.content==null) return;
            if (!isMoving) return;

            if (m_Mask != null)
            {
                scrollRect.content.GetWorldCorners(contentCorners);
                m_Mask.rectTransform.GetWorldCorners(maskCorners);
                var localContent=UguiTools.GlobalPoints2LocalPoints(contentCorners,transform);
                var localMask= UguiTools.GlobalPoints2LocalPoints(maskCorners, transform);
                var contentRect = UguiTools.GetRectContainsPoints(localContent);
                var maskRect = UguiTools.GetRectContainsPoints(localMask);
                if (!contentRect.Overlaps(maskRect))
                {
                    var restPosition = contentRect.position;
                    switch (m_Direction)
                    {
                        case Direction.Horizontal:
                            if (m_Invert)
                            {
                                restPosition.x = maskRect.xMax;
                                restPosition.y += contentRect.height;
                            }
                            else
                            {
                                restPosition.x = maskRect.xMin-contentRect.width;
                                restPosition.y += contentRect.height;
                            }
                            break;

                        case Direction.Vertical:
                            if (m_Invert)
                            {
                                restPosition.y = maskRect.yMax + contentRect.height;
                            }
                            else
                            {
                                restPosition.y = maskRect.yMin;
                            }
                            break;
                    }
                    scrollRect.content.position = transform.TransformPoint(restPosition);
                    isMoving = false;
                    Invoke("Play", repeatInterval);
                }
            }

            var direction = (m_Direction == Direction.Horizontal) ?
                Vector2.right :
                Vector2.up;
            if (m_Invert) direction *= -1;
            scrollRect.content.Translate(direction * speed, Space.Self);
        }

        public virtual void Play()
        {
            isMoving = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            CancelInvoke("Play");
            isMoving = false;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            CancelInvoke("Play");
            if (!stopScrollWhenDrag)
            {
                isMoving = true;
            }
        }

        /// <summary>
        /// 方向
        /// </summary>
        public enum Direction
        {
            Horizontal,
            Vertical
        }
    }
}