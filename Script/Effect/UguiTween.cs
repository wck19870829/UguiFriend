using UnityEngine;
using System.Collections;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 缓动基类
    /// </summary>
    public abstract class UguiTween: MonoBehaviour
    {
        [SerializeField] protected PlayStyle m_PlayStyle;
        [SerializeField] protected AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] protected float duration=0.2f;
        float speed;

        public Action<UguiTween> OnStepFinished;

        protected virtual void Update()
        {

        }

        public virtual void Play(Direction direction,float duration=0.2f)
        {
            speed = (int)direction;
            this.duration = duration;
        }

        /// <summary>
        /// 播放方向
        /// </summary>
        public enum Direction
        {
            Forward=1,
            Reverse=-1
        }

        public enum PlayStyle
        {
            Once,
            Loop,
            PingPong
        }
    }
}