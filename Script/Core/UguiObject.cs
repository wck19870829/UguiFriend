using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace RedScarf.UguiFriend
{
    public abstract class UguiObject : UIBehaviour, IUguiObject
    {
        protected UguiObjectData m_Data;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UguiObjectManager.Instance.Unregister(this);
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
                if (value.GetBindingInfo().entityType != GetType())
                {
                    throw new Exception("此实体的数据绑定类型应为:"+ value.GetBindingInfo().entityType);
                }

                UguiObjectManager.Instance.Unregister(this);
                m_Data = value;
                UguiObjectManager.Instance.Register(this);
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