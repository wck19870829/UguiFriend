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
        protected UguiObjectData m_Data;
        protected Canvas m_Canvas;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UguiObjectManager.Unregister(this);
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            m_Canvas = GetComponentInParent<Canvas>();
            Debug.Log("OnTransformParentChanged");
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
        }

        /// <summary>
        /// 数据
        /// </summary>
        public UguiObjectData Data
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

                    SetDirty();
                }
            }
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
            StopCoroutine("RefreshViewDelay");
            StartCoroutine("RefreshViewDelay");
        }

        /// <summary>
        /// 立即刷新界面
        /// </summary>
        public void RefreshViewImmediate()
        {
            RefreshView();
        }

        IEnumerator RefreshViewDelay()
        {
            yield return new WaitForEndOfFrame();
            RefreshView();
        }

        /// <summary>
        /// 子类重写此方法刷新视图
        /// </summary>
        protected abstract void RefreshView();

        /// <summary>
        /// 快照
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

        /// <summary>
        /// 是否在屏幕中
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool ScreenContains(UguiObject obj)
        {
            if (obj.Canvas != null)
            {
                var bounds = UguiMathf.GetGlobalBoundsIncludeChildren(obj.transform as RectTransform, false);
                var rect=UguiMathf.GetScreenRect(bounds, obj.Canvas.rootCanvas.worldCamera);
                var screenRect = new Rect(0,0,Screen.width,Screen.height);

                return UguiMathf.RectOverlap(rect, screenRect)==null
                        ?false
                        :true;
            }

            return false;
        }
    }
}