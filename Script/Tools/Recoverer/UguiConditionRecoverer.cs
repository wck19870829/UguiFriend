using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 条件回收器
    /// </summary>
    public abstract class UguiConditionRecoverer : MonoBehaviour
    {
        [SerializeField] protected UpdateMode m_UpdateMode;

        protected UguiConditionRecoverer()
        {
            m_UpdateMode = UpdateMode.Update;
        }

        protected virtual void Start()
        {
            if (m_UpdateMode == UpdateMode.StartOnly)
            {
                FindChildren();
            }
        }

        protected virtual void Update()
        {
            if(m_UpdateMode==UpdateMode.Update)
                FindChildren();

            Check();
        }

        protected abstract void Check();
        protected abstract void FindChildren();

        public abstract bool Recycle<Element>(Element child);

        /// <summary>
        /// 获取及更新模式
        /// </summary>
        public enum UpdateMode
        {
            StartOnly = 0,          //Start获取
            Update = 1,             //Update中更新获取
            Code = 2                //代码设置获取
        }
    }

    public abstract class UguiConditionRecoverer<T>: UguiConditionRecoverer
        where T:Component
    {
        protected HashSet<T> m_ChildSet;
        protected List<T> m_RemoveList;

        public Action<T> OnRecycle;

        protected UguiConditionRecoverer()
        {
            m_ChildSet = new HashSet<T>();
            m_RemoveList = new List<T>();
        }

        protected override void FindChildren()
        {
            m_ChildSet.Clear();
            var len = transform.childCount;
            var cacheTransform = transform;
            for (var i=0;i< len;i++)
            {
                var child= cacheTransform.GetChild(i).GetComponent<T>();
                if(child)
                    m_ChildSet.Add(child);
            }
        }

        /// <summary>
        /// 回收之前处理
        /// </summary>
        /// <param name="child"></param>
        protected virtual void ProcessItemBeforeRecycle(T child)
        {

        }

        /// <summary>
        /// 回收之后处理
        /// </summary>
        /// <param name="child"></param>
        protected virtual void ProcessItemAfterRecycle(T child)
        {

        }

        public override bool Recycle<Element>(Element child)
        {
            var obj = child as T;
            if (obj)
            {
                m_ChildSet.Remove(obj);

                return true;
            }

            return false;
        }

        public HashSet<T> ChildSet
        {
            get
            {
                return m_ChildSet;
            }
            set
            {
                m_ChildSet = value;
                m_UpdateMode = UpdateMode.Code;
            }
        }
    }
}