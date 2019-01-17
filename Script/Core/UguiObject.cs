using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    [Serializable]
    /// <summary>
    /// Ugui对象基类
    /// </summary>
    public abstract class UguiObject : UIBehaviour
    {
        protected const string openStateName = "Open";
        protected const string closeStateName = "Close";

        protected Animator m_Animator;
        protected UguiObjectData m_Data;
        protected Canvas m_Canvas;
        protected RectTransform m_RectTransform;
        protected internal bool m_Dirty;

        public Action<UguiObject> OnCloseComplete;
        public Action<UguiObject> OnOpenComplete;

        protected UguiObject()
        {
            m_Dirty = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UguiObjectManager.Unregister(this);
        }

        /// <summary>
        /// 打开
        /// </summary>
        public virtual void Open()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            Play(openStateName);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void Close()
        {
            if (!Play(closeStateName))
            {
                if (gameObject.activeSelf)
                {
                    gameObject.SetActive(false);
                    InvokeCloseComplete();
                }
            }
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public virtual bool Play(string stateName)
        {
            if (m_Animator == null)
                m_Animator = GetComponent<Animator>();
            if (m_Animator == null) return false;

            m_Animator.Play(stateName,0,0);
            return true;
        }

        /// <summary>
        /// 关闭完成
        /// </summary>
        public void InvokeCloseComplete()
        {
            if (OnCloseComplete != null)
                OnCloseComplete.Invoke(this);
        }

        /// <summary>
        /// 矩形变换组件
        /// </summary>
        public RectTransform RectTransform
        {
            get
            {
                if (m_RectTransform == null)
                    m_RectTransform = GetComponent<RectTransform>();

                return m_RectTransform;
            }
        }

        /// <summary>
        /// 数据
        /// </summary>
        public virtual UguiObjectData Data
        {
            get
            {
                return m_Data;
            }
            set
            {
                if (value == null)
                {
                    //为null取消注册
                    UguiObjectManager.Unregister(this);
                    m_Data = null;
                }
                else
                {
                    if (!UguiObjectManager.CheckMatch(value, this))
                    {
                        throw new Exception("数据类型不匹配!");
                    }

                    UguiObjectManager.Unregister(this);
                    m_Data = value;
                    UguiObjectManager.Register(this);

                    OnSetData(m_Data);
                    SetDirty();
                }
            }
        }

        protected virtual void OnSetData(UguiObjectData newData)
        {

        }

        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string Guid
        {
            get
            {
                if (m_Data != null)
                {
                    return m_Data.guid;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// 画布
        /// </summary>
        public Canvas Canvas
        {
            get
            {
                return m_Canvas;
            }
        }

        /// <summary>
        /// 设置改变标记
        /// </summary>
        public void SetDirty()
        {
            m_Dirty = true;
        }

        /// <summary>
        /// 强制立即刷新界面
        /// </summary>
        public void RefreshViewImmediate()
        {
            if (m_Data != null)
            {
                RefreshView();
            }
            m_Dirty = false;
        }

        /// <summary>
        /// 子类重写此方法刷新视图
        /// </summary>
        protected abstract void RefreshView();

        /// <summary>
        /// 快照,实质为深度复制对象数据
        /// </summary>
        /// <returns></returns>
        public UguiObjectData Snapshoot()
        {
            if (m_Data != null)
            {
                return m_Data.DeepClone();
            }

            return null;
        }

        public virtual void GotoNextStep(UguiObjectData data)
        {

        }

        public virtual void GotoPrevStep(UguiObjectData data)
        {

        }

        public virtual void GotoStep(UguiObjectData data)
        {

        }
    }
}