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
                throw new Exception("Object is null.");

            var objType = obj.GetType();
            if (!poolDict.ContainsKey(objType))
            {
                poolDict.Add(objType, new Stack<UguiObject>());
            }
            poolDict[objType].Push(obj);

            obj.transform.SetParent(content);
            obj.transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// 放入池
        /// </summary>
        /// <param name="objList"></param>
        public virtual void Push(List<UguiObject>objList)
        {
            if (objList == null)
                throw new Exception("Object list is null.");

            foreach (var obj in objList)
            {
                Push(obj);
            }
        }

        /// <summary>
        /// 由对象类型获取对象
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public virtual UguiObject Get(Type objType, Transform parent)
        {
            if (!typeof(UguiObject).IsAssignableFrom(objType))
            {
                throw new Exception("类型应继承自UguiObject:"+objType);
            }

            UguiObject obj = null;
            //优先池中获取
            if (poolDict.ContainsKey(objType))
            {
                if (poolDict[objType].Count > 0)
                {
                    obj=poolDict[objType].Pop();
                }
            }

            if (obj==null)
            {
                //创建新的
                obj = UguiObjectManager.CreateNew(objType);
            }

            obj.transform.SetParent(parent);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;

            return obj;
        }

        /// <summary>
        /// 由数据获取对象
        /// </summary>
        /// <param name="data"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public virtual UguiObject Get(UguiObjectData data,Transform parent)
        {
            if (data == null)
                throw new Exception("Data is null.");

            var objType=UguiObjectManager.GetObjectType(data.GetType());
            var obj = Get(objType,parent);
            obj.Data = data;

            return obj;
        }

        /// <summary>
        /// 由对象预设和数据获取对象
        /// </summary>
        /// <param name="data"></param>
        /// <param name="prefabSource"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public virtual UguiObject Get(UguiObjectData data, UguiObject prefabSource, Transform parent)
        {
            if (prefabSource == null)
                throw new Exception("Prefab source is null.");

            var prefabType = prefabSource.GetType();
            var obj = Get(prefabType, parent);
            obj.Data = data;

            return obj;
        }

        /// <summary>
        /// 由数据集合获取对象列表
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public virtual void Get(List<UguiObject> objList, List<UguiObjectData>dataList,Transform parent)
        {
            if (dataList == null)
                throw new Exception("Data list is null.");

            if (objList == null)
                objList = new List<UguiObject>(dataList.Count);

            objList.Clear();
            foreach (var data in dataList)
            {
                var obj = Get(data, parent);
                objList.Add(obj);
            }
        }

        /// <summary>
        /// 由数据集合及预设获取对象列表
        /// </summary>
        /// <param name="objList"></param>
        /// <param name="dataList"></param>
        /// <param name="prefabSource"></param>
        /// <param name="parent"></param>
        public virtual void Get(List<UguiObject>objList,List<UguiObjectData> dataList, UguiObject prefabSource, Transform parent)
        {
            if (dataList == null)
                throw new Exception("Data list is null.");
            if (prefabSource == null)
                throw new Exception("Prefab source is null.");

            if (objList == null)
                objList = new List<UguiObject>(dataList.Count);

            objList.Clear();
            foreach (var data in dataList)
            {
                var obj = Get(data, prefabSource, parent);
                objList.Add(obj);
            }
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
            transform.position = new Vector3(999999,999999,999999);
        }
    }
}