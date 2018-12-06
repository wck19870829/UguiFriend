using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 对象池
    /// </summary>
    public class UguiObjectPool:UguiSingleton<UguiObjectPool>,
        IUguiSingletonCreate<UguiObjectPool>
    {
        Transform content;
        Dictionary<Type, Stack<UguiObject>> poolDict;

        /// <summary>
        /// 放入池
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Push(UguiObject obj)
        {
            if (obj == null)
                throw new Exception("obj==null!");

            var objType = obj.GetType();
            if (!poolDict.ContainsKey(objType))
            {
                poolDict.Add(objType, new Stack<UguiObject>());
            }
            poolDict[objType].Push(obj);

            obj.transform.SetParent(content,true);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public virtual UguiObject Get(Type objType)
        {
            if (!typeof(UguiObject).IsAssignableFrom(objType))
            {
                throw new Exception("类型应继承自UguiObject:"+objType);
            }

            //优先池中获取
            if (poolDict.ContainsKey(objType))
            {
                if (poolDict[objType].Count > 0)
                {
                    return poolDict[objType].Pop();
                }
            }

            //创建新的
            return UguiObjectManager.CreateNew(objType);
        }

        /// <summary>
        /// 清除
        /// </summary>
        public virtual void Clear()
        {
            poolDict.Clear();
        }

        public void OnSingletonCreate(UguiObjectPool instance)
        {
            poolDict = new Dictionary<Type, Stack<UguiObject>>();
            content = UguiTools.AddChild<Transform>("Content",transform);
            content.gameObject.SetActive(false);
        }
    }
}