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
        static Dictionary<Type, UguiBindingAttribute> s_DataBindingDict;

        Dictionary<string, IUguiObject> m_ObjectDict;

        static UguiObjectManager()
        {
            s_DataBindingDict = new Dictionary<Type, UguiBindingAttribute>();
            var types = UguiTools.FindSubClass(typeof(UguiObjectData));
            foreach (var type in types)
            {
                var customAtts = type.GetCustomAttributes(typeof(UguiBindingAttribute), false);
                if (customAtts != null && customAtts.Length == 1)
                {
                    var customAtt = (UguiBindingAttribute)customAtts[0];
                    if (typeof(IUguiObject).IsAssignableFrom(customAtt.entityType))
                    {
                        s_DataBindingDict.Add(type, customAtt);
                        continue;
                    }
                }

                Debug.LogErrorFormat("数据绑定实体错误:{0}", type);
            }
        }


        public void OnSingletonCreate(UguiObjectManager instance)
        {
            DontDestroyOnLoad(gameObject);
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

        public UguiBindingAttribute GetBindingAtt(UguiObjectData data)
        {
            return s_DataBindingDict[data.GetType()];
        }
    }
}