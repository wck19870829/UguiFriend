using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// Ugui对象基类
    /// </summary>
    public abstract class UguiObject : UIBehaviour
    {
        protected UguiObjectData m_Data;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UguiObjectManager.Unregister(this);
        }

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
                    throw new Exception("不能为null!");
                }
                if (!UguiObjectManager.CheckMatch(value, this))
                {
                    throw new Exception("数据类型不匹配!");
                }

                UguiObjectManager.Unregister(this);
                m_Data = value;
                UguiObjectManager.Register(this);
            }
        }

        public string Guid
        {
            get
            {
                if (m_Data != null)
                {
                    return m_Data.guid;
                }

                return "";
            }
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
    }
}