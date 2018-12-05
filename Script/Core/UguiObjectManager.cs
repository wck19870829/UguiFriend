using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 管理器
    /// </summary>
    public sealed class UguiObjectManager : UguiSingleton<UguiObjectManager>,
        IUguiSingletonCreate<UguiObjectManager>
    {
        Dictionary<string, IUguiObject> m_ObjectDict;

        public void OnSingletonCreate(UguiObjectManager instance)
        {

        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="obj"></param>
        public void Register(IUguiObject obj)
        {
            if (obj == null) return;

            if (!m_ObjectDict.ContainsKey(obj.Guid))
            {
                m_ObjectDict.Add(obj.Guid, obj);
            }
            else
            {
                m_ObjectDict[obj.Guid] = obj;
            }
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="obj"></param>
        public void Unregister(IUguiObject obj)
        {
            if (obj == null) return;

            m_ObjectDict.Remove(obj.Guid);
        }

        public Dictionary<string, IUguiObject> ObjectDict
        {
            get
            {
                return m_ObjectDict;
            }
        }
    }
}