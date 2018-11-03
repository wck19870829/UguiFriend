using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Reflection;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 缓动基类
    /// </summary>
    public abstract class UguiTween: UIBehaviour
    {
        [SerializeField] protected bool playOnEnable=true;
        [SerializeField] protected PlayStyle m_PlayStyle;
        [SerializeField] protected AnimationCurve m_AnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] protected float m_Duration=0.2f;
        [SerializeField] protected bool m_IsPlaying;
        [SerializeField] protected Direction m_Direction=Direction.Forward;
        [SerializeField] protected float m_Progress;

        public Action<UguiTween> OnStepFinished;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (playOnEnable)
                Play(Direction.Forward, 0, m_Duration);
        }

        protected virtual void Update()
        {
            if (!m_IsPlaying) return;

            m_Duration = (m_Duration <= 0) ? 0.001f : m_Duration;
            m_Progress +=Time.deltaTime/m_Duration* Mathf.Sign((int)m_Direction);
            var newProgress = Mathf.Clamp(m_Progress, 0, 1);
            UpdateProgress(newProgress);
            var t = m_AnimationCurve.Evaluate(newProgress);
            UpdateValue(t);

            switch (m_PlayStyle)
            {
                case PlayStyle.Loop:
                    if (m_Progress <= 0 || m_Progress >= 1)
                    {
                        CallStepFinished();
                    }
                    if (m_Progress <= 0) m_Progress = 1;
                    else if (m_Progress >= 1) m_Progress = 0;
                    break;

                case PlayStyle.Once:
                    if (m_Progress < 0 || m_Progress > 1)
                    {
                        CallStepFinished();
                        m_IsPlaying = false;
                    }
                    break;

                case PlayStyle.PingPong:
                    if (m_Progress <= 0 || m_Progress >= 1)
                    {
                        CallStepFinished();
                        m_Direction = (m_Direction == Direction.Forward) ?
                                        Direction.Reverse :
                                        Direction.Forward;
                    }
                    break;
            }
            m_Progress = newProgress;
        }

        protected abstract void UpdateValue(float t);

        public virtual void UpdateProgress(float progress)
        {

        }

        public virtual void Play()
        {
            Play(m_Direction, m_Progress, m_Duration);
        }

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="direction">播放方向</param>
        /// <param name="progress">进度值范围:(0f-1f),其他值不改变当前值</param>
        /// <param name="duration">持续时间,小于0不改变当前值</param>
        public virtual void Play(Direction direction,float progress=-1,float duration=-1)
        {
            if (progress >= 0 && progress <= 1)
            {
                m_Progress = progress;
            }
            if (duration >= 0)
            {
                m_Duration = duration;
            }
            m_Direction = direction;
            m_IsPlaying = true;
        }

        public virtual void Pause()
        {
            m_IsPlaying = false;
        }

        protected void CallStepFinished()
        {
            if (OnStepFinished != null)
            {
                try
                {
                    OnStepFinished.Invoke(this);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        /// <summary>
        /// 持续时间
        /// </summary>
        public float Duration
        {
            get
            {
                return m_Duration;
            }
        }

        /// <summary>
        /// 当前进度
        /// </summary>
        public float Progress
        {
            get
            {
                return m_Progress;
            }
        }

        /// <summary>
        /// 当前播放方向
        /// </summary>
        public Direction CurrentDirection
        {
            get
            {
                return m_Direction;
            }
        }

        /// <summary>
        /// 播放循环类型
        /// </summary>
        public PlayStyle CurrentPlayStyle
        {
            get
            {
                return m_PlayStyle;
            }
            set
            {
                m_PlayStyle = value;
            }
        }

        /// <summary>
        /// 播放方向
        /// </summary>
        public enum Direction
        {
            Forward=1,
            Reverse=-1
        }

        /// <summary>
        /// 播放循环类型
        /// </summary>
        public enum PlayStyle
        {
            Once,
            Loop,
            PingPong
        }

        public abstract class PropDrive
        {
            public AnimationCurve animationCurve;
        }
    }

    /// <summary>
    /// 缓动泛型基类
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class UguiTween<TValue,TComponent> : UguiTween
        where TComponent:Component
    {
        [SerializeField] protected TValue m_From;
        [SerializeField] protected TValue m_To;
        [SerializeField] protected TComponent m_Component;
        [SerializeField] protected TValue m_Value;

        protected override void Awake()
        {
            if (m_Component == null)
            {
                m_Component = GetComponentInChildren<TComponent>();
                if (m_Component == null)
                    throw new Exception("Component is null.");
            }

            base.Awake();
        }

        protected override void UpdateValue(float t)
        {
            m_Value = RefrashView(m_From, m_To, t);
        }

        protected abstract TValue RefrashView(TValue from, TValue to, float t);

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="direction"></param>
        /// <param name="progress"></param>
        public virtual void Play(TValue from,TValue to,Direction direction, float progress = -1,float duration=-1)
        {
            m_From = from;
            m_To = to;
            Play(direction, progress,duration);
        }

        /// <summary>
        /// 开始值
        /// </summary>
        public TValue From
        {
            get
            {
                return m_From;
            }
        }

        /// <summary>
        /// 结束值
        /// </summary>
        public TValue To
        {
            get
            {
                return m_To;
            }
        }

        /// <summary>
        /// 当前值
        /// </summary>
        public TValue Value
        {
            get
            {
                return m_Value;
            }
        }
    }
}