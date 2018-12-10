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
        /// 放入池
        /// </summary>
        /// <param name="objList"></param>
        public virtual void Push(List<UguiObject>objList)
        {
            if (objList == null)
                throw new Exception("List is null.");

            foreach (var obj in objList)
            {
                Push(obj);
            }
            objList.Clear();
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
        /// 由数据获取实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual UguiObject Get(UguiObjectData data)
        {
            if (data == null)
                throw new Exception("Data is null.");

            var objType=UguiObjectManager.GetObjectType(data.GetType());

            return Get(objType);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public virtual List<UguiObject> Get(List<UguiObjectData>dataList)
        {
            var list = new List<UguiObject>();
            foreach (var data in dataList)
            {
                list.Add(Get(data));
            }

            return list;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public virtual void Clear()
        {
            foreach (var value in poolDict.Values)
            {
                foreach (var obj in value)
                {
                    DestroyObject(obj);
                }
            }
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