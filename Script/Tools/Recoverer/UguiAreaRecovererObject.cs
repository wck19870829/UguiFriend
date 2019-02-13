using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// Object屏幕越界回收器
    /// </summary>
    public class UguiAreaRecovererObject : UguiAreaRecoverer<UguiObject>
    {
        protected Dictionary<string, UguiObject> m_InAreaDict;    //视图中可见的子物体

        protected UguiAreaRecovererObject()
        {
            m_InAreaDict = new Dictionary<string, UguiObject>();
        }

        protected override void FindChildren()
        {
            base.FindChildren();

            m_InAreaDict.Clear();
            foreach (var child in m_ChildSet)
            {
                if(!m_InAreaDict.ContainsKey(child.Guid))
                    m_InAreaDict.Add(child.Guid, child);
            }
        }

        public override bool Recycle<Element>(Element child)
        {
            var isRemove= base.Recycle(child);
            if (isRemove)
            {
                var obj = child as UguiObject;
                m_InAreaDict.Remove(obj.Guid);
                UguiObjectPool.Instance.Push(obj);
            }

            return isRemove;
        }

        public Dictionary<string, UguiObject> InAreaDict
        {
            get
            {
                return m_InAreaDict;
            }
        }
    }
}