using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(UguiCenterOnChild))]
    /// <summary>
    /// 图片轮播
    /// </summary>
    public class UguiSliderShow : UIBehaviour
    {
        [Range(1f, 30f)]
        [SerializeField] protected float m_Interval = 10;
        [SerializeField] protected SequenceType m_StartSequenceType;
        [SerializeField] protected WrapMode m_StartWrapMode;
        protected UguiCenterOnChild centerOnChild;
        protected float m_CountDown;
        protected int currentIndex;

        protected override void Awake()
        {
            base.Awake();

            centerOnChild = GetComponent<UguiCenterOnChild>();
            if (centerOnChild == null)
                centerOnChild = gameObject.AddComponent<UguiCenterOnChild>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            m_CountDown = m_Interval;
        }

        protected virtual void Update()
        {
            m_CountDown -= Time.time;
            if (m_CountDown <= 0)
            {
                Play();
            }
        }

        protected virtual void Play()
        {
            if (centerOnChild == null||
                centerOnChild.ScrollRect==null||
                centerOnChild.ScrollRect.content==null)
                return;

            m_CountDown = m_Interval;
            currentIndex += (int)m_StartSequenceType;
            switch (m_StartWrapMode)
            {
                case WrapMode.Loop:

                    break;

                case WrapMode.Once:

                    break;

                case WrapMode.PingPong:

                    break;
            }
            currentIndex = Mathf.Clamp(currentIndex, 0, centerOnChild.ScrollRect.content.childCount-1);
        }

        /// <summary>
        /// 序列方向
        /// </summary>
        public SequenceType StartSequenceType
        {
            get
            {
                return m_StartSequenceType;
            }
            set
            {
                m_StartSequenceType = value;
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
        /// 序列
        /// </summary>
        public enum SequenceType
        {
            Backwards=-1,
            Forward=1
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