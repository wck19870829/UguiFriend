using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(UguiCenterOnChild))]
    /// <summary>
    /// 图片轮播
    /// </summary>
    public class UguiSliderShow : UIBehaviour,IDragHandler
    {
        [Range(1f, 60f)]
        [SerializeField] protected float m_Interval = 10;
        [SerializeField] protected FindDirection m_StartFindDirection;
        [SerializeField] protected WrapMode m_StartWrapMode;
        [SerializeField] protected bool m_Invert;
        protected int speed=1;
        protected RectTransform m_Content;
        protected UguiCenterOnChild centerOnChild;
        protected float m_CountDown;
        protected Transform current;
        protected List<Transform> m_Children;
        protected Dictionary<Transform, float> m_ChildrenScoreDict;
        protected bool m_Init;

        protected UguiSliderShow()
        {
            m_Children = new List<Transform>();
            m_ChildrenScoreDict = new Dictionary<Transform, float>();


            var points = new Vector3[] {
                new Vector3(100,100),
                new Vector3(-100,-100)
            };
            Debug.Log(UguiTools.GetRectContainsPoints(points));
        }

        protected override void Awake()
        {
            base.Awake();

            Init();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Init();
            m_CountDown = m_Interval;
            if (current == null&&m_Content.childCount>0)
            {
                current = m_Content.GetChild(0);
                centerOnChild.CenterOn(current, true);
            }
        }

        protected virtual void Update()
        {
            m_CountDown -= Time.deltaTime;
            if (m_CountDown <= 0)
            {
                GotoNext();
            }
        }

        protected virtual void Init()
        {
            if (!m_Init)
            {
                centerOnChild = GetComponent<UguiCenterOnChild>();
                try
                {
                    m_Content = centerOnChild.ScrollRect.content;
                }
                catch (Exception e)
                {
                    throw e;
                }

                m_Init = true;
            }
        }

        /// <summary>
        /// 每个子物体根据原点点积排列顺序
        /// </summary>
        /// <param name="findDirection">方向</param>
        protected virtual void SortChildrenByScore(FindDirection findDirection)
        {
            if (m_Content.childCount == 0) return;

            m_Children.Clear();
            m_ChildrenScoreDict.Clear();
            var rect=UguiTools.GetLocalRectIncludeChildren(m_Content,m_Content,false);
            if (rect == null) return;

            var origin = ((Rect)rect).min;
            var findVector = (findDirection == FindDirection.Horizontal)?
                            Vector2.right:
                            Vector2.up;
            foreach (Transform child in m_Content)
            {
                var localPos = m_Content.InverseTransformPoint(child.position);
                var dir = (Vector2)localPos - origin;
                var dot = Mathf.Max(Vector2.Dot(dir, findVector),0.01f);
                var score = dot / dir.sqrMagnitude;

                m_ChildrenScoreDict.Add(child, score);
                m_Children.Add(child);
            }

            m_Children.Sort((a,b)=> {
                var scoreA = m_ChildrenScoreDict[a];
                var scoreB = m_ChildrenScoreDict[b];

                if (scoreA == scoreB) return 0;
                return scoreA > scoreB ? 1 : -1;
            });
        }

        /// <summary>
        /// 查询下一个目标
        /// </summary>
        /// <param name="wrapMode"></param>
        /// <param name="findDirection"></param>
        /// <param name="invert"></param>
        /// <returns></returns>
        protected virtual Transform FindNext(Transform origin, WrapMode wrapMode, FindDirection findDirection,bool invert)
        {
            if (origin == null)
                return null;
            if (origin.parent != m_Content)
            {
                Debug.LogErrorFormat("{0}'s parent is not this content.", origin);
                return null;
            }
            if (m_Content.childCount == 1)
                return null;

            Transform bestPick = null;
            SortChildrenByScore(findDirection);
            var index = m_Children.IndexOf(origin);
            var moveTo = invert ? index - 1 : index + 1;
            var maxIndex = m_Children.Count-1;
            switch (wrapMode)
            {
                case WrapMode.Loop:
                    if (!invert)
                    {
                        if (moveTo > maxIndex) bestPick = m_Children[0];
                        else bestPick = m_Children[moveTo];
                    }
                    else
                    {
                        if(moveTo<0) bestPick = m_Children[maxIndex];
                        else bestPick = m_Children[moveTo];
                    }
                    break;

                case WrapMode.Once:
                    if (moveTo >= 0 && moveTo <= maxIndex)
                    {
                        bestPick = m_Children[moveTo];
                    }
                    break;

                case WrapMode.PingPong:
                    if (index >= maxIndex || index <= 0) speed=-speed;
                    moveTo = index + speed;
                    if (moveTo <= maxIndex && moveTo>=0)
                    {
                        bestPick = m_Children[moveTo];
                    }
                    break;
            }

            return bestPick;
        }

        /// <summary>
        ///  尝试下一个居中
        /// </summary>
        protected virtual void GotoNext()
        {
            m_CountDown = m_Interval;
            Transform next = FindNext(current,m_StartWrapMode,m_StartFindDirection,m_Invert);
            if (next != null)
            {
                current = next;
                centerOnChild.CenterOn(current, false);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_CountDown = m_Interval;
        }

        /// <summary>
        /// 序列方向
        /// </summary>
        public FindDirection StartFindDirection
        {
            get
            {
                return m_StartFindDirection;
            }
            set
            {
                m_StartFindDirection = value;
            }
        }

        /// <summary>
        /// 时间间隔
        /// </summary>
        public float Interval
        {
            get
            {
                return m_Interval;
            }
            set
            {
                m_Interval = value;
            }
        }

        /// <summary>
        /// 循环模式
        /// </summary>
        public WrapMode StartWrapMode
        {
            get
            {
                return m_StartWrapMode;
            }
            set
            {
                m_StartWrapMode = value;
            }
        }

        /// <summary>
        /// 查找方向
        /// </summary>
        public enum FindDirection
        {
            Horizontal,
            Vertical
        }

        /// <summary>
        /// 循环模式
        /// </summary>
        public enum WrapMode
        {
            Loop,
            PingPong,
            Once,
        }
    }
}