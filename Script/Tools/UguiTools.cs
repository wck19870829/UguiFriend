using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class UguiTools
    {
        public static List<UguiObject> objTempList;
        public static Dictionary<string, UguiObject> objTempDict;
        public static Dictionary<string,UguiObjectData> objDataTempSet;
        public static List<UguiObjectData> objDataTempList;

        static UguiTools()
        {
            objTempList = new List<UguiObject>();
            objTempDict = new Dictionary<string, UguiObject>();
            objDataTempSet = new Dictionary<string, UguiObjectData>();
            objDataTempList = new List<UguiObjectData>();
        }

        #region 图片

        /// <summary>
        /// 调整图片HSV
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="hOffset"></param>
        /// <param name="sOffset"></param>
        /// <param name="vOffset"></param>
        public static void AdjustTexture(Texture2D src,Texture2D dest,float hOffset,float sOffset,float vOffset)
        {
            if (!src) return;


        }

        /// <summary>
        /// 缩放图片
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="maxWdith"></param>
        /// <param name="maxHeight"></param>
        public static void AdjustTexture(Texture2D src,int maxWdith,int maxHeight)
        {
            if (!src) return;


        }

        /// <summary>
        /// Texture转Texture2D
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="t2d"></param>
        public static void Texture2Texture2D(Texture tex,Texture2D t2d)
        {

        }

        #endregion

        /// <summary>
        /// 获取置顶的ui元素
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <returns></returns>
        public static GameObject GetTopElement(Vector2 screenPosition)
        {
            var pointerInputModule = EventSystem.current.currentInputModule as PointerInputModule;
            if (pointerInputModule)
            {
                var pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = screenPosition;
                var raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerEventData, raycastResults);
                if (raycastResults.Count > 0)
                {
                    return raycastResults[0].gameObject;
                }
            }

            return null;
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="children"></param>
        /// <param name="content"></param>
        /// <param name="dataList"></param>
        /// <param name="prefabSource"></param>
        public static void SetChildrenDatas(List<UguiObject>children,Transform content,List<UguiObjectData>dataList,UguiObject prefabSource)
        {
            //继续使用的对象复用,未使用中的对象放入池中,最后补齐新增加的对象
            objDataTempSet.Clear();
            objDataTempList.Clear();
            objTempList.Clear();
            foreach (var data in dataList)
            {
                objDataTempSet.Add(data.guid, data);
            }
            foreach (var child in children)
            {
                if (!objDataTempSet.ContainsKey(child.Guid))
                {
                    objTempList.Add(child);
                }
                else
                {
                    objDataTempList.Add(objDataTempSet[child.Guid]);
                }
            }
            foreach (var removeChild in objTempList)
            {
                children.Remove(removeChild);
            }
            UguiObjectPool.Instance.Push(objTempList);
            DestroyChildren(content.gameObject);

            if (prefabSource == null)
            {
                UguiObjectPool.Instance.Get(objTempList, objDataTempList, content);
            }
            else
            {
                UguiObjectPool.Instance.Get(objTempList, objDataTempList, prefabSource, content);
            }
            children.AddRange(objTempList);
            objTempList.Clear();
            objDataTempList.Clear();
            objTempDict.Clear();
            foreach (var child in children)
            {
                objTempDict.Add(child.Guid, child);
            }
            children.Clear();
            foreach (var data in dataList)
            {
                children.Add(objTempDict[data.guid]);
            }
            objTempDict.Clear();
        }

        /// <summary>
        /// 销毁所有子元素
        /// </summary>
        /// <param name="go"></param>
        public static void DestroyChildren(GameObject go)
        {
            for (var i = go.transform.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(go.transform.GetChild(i).gameObject);
            }
        }

        

        /// <summary>
        /// 添加一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T AddChild<T>(string name,Transform parent=null)where T:Component
        {
            var go = new GameObject(name);
            go.AddComponent<RectTransform>();
            if (parent != null)
                go.transform.SetParent(parent);

            go.transform.localPosition = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            var component = go.GetComponent<T>();
            if(component==null)
                component= go.AddComponent<T>();

            return component;
        }

        /// <summary>
        /// 复制一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T AddChild<T>(T source, Transform parent = null) where T : Component
        {
            if (source == null)
                throw new Exception("Source is null.");

            var clone = GameObject.Instantiate<T>(source);
            if (parent != null)
                clone.transform.SetParent(parent);
            clone.transform.localScale = Vector3.one;
            clone.transform.localRotation = Quaternion.identity;
            clone.transform.localPosition = Vector3.zero;

            return clone;
        }

        /// <summary>
        /// 查询一个不使用的层
        /// </summary>
        /// <returns></returns>
        public static int FindUnusedLayer()
        {
            for (var i=0;i<32;i++)
            {
                if (string.IsNullOrEmpty(LayerMask.LayerToName(i)))
                {
                    return i;
                }
            }

            return -1;
        }

        public static void CopyProps(object source, object destination,bool onlyCopyValueType=true)
        {
            if (source == null || destination == null)
                return;

            var flag = BindingFlags.Public | BindingFlags.Instance;
            var props = source.GetType().GetProperties(flag);
            foreach (var prop in props)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    if (onlyCopyValueType)
                    {
                        if (!typeof(ValueType).IsAssignableFrom(prop.PropertyType))
                        {
                            continue;
                        }
                    }

                    try
                    {
                        var value = prop.GetValue(source, null);
                        prop.SetValue(destination, value, null);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);
                    }
                }
            }
        }

        /// <summary>
        /// 添加事件触发器
        /// </summary>
        /// <param name="target"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public static void AddTriger(GameObject target,EventTriggerType type,UnityAction<BaseEventData> callback)
        {
            var trigger = target.GetComponent<EventTrigger>();
            if (!trigger) trigger = target.AddComponent<EventTrigger>();

            foreach (var t in trigger.triggers)
            {
                if (t.eventID == type)
                {
                    t.callback.AddListener(callback);
                    return;
                }
            }

            var entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(callback);
            trigger.triggers.Add(entry);
        }

        #region 其他

        /// <summary>
        /// 查找子类
        /// </summary>
        /// <param name="baseType">父类</param>
        /// <returns></returns>
        public static List<Type> FindSubClass(Type baseType)
        {
            var subTypeList = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assemblie in assemblies)
            {
                var types = assemblie.GetTypes();
                foreach (var type in types)
                {
                    if (baseType.IsAssignableFrom(type))
                    {
                        subTypeList.Add(type);
                    }
                }
            }

            return subTypeList;
        }

        /// <summary>
        /// byte[]转string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Bytes2String(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// string转byte[]
        /// </summary>
        /// <param name="str"></param>
        public static byte[] String2Bytes(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// 二进制序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeObject(object obj)
        {
            if (obj == null)
                return null;

            using (var ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                byte[] bytes = ms.GetBuffer();
                ms.Close();

                return bytes;
            }
        }

        /// <summary>
        /// 二进制反序列化对象
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static object DeserializeObject(byte[] bytes)
        {
            if (bytes == null||bytes.Length==0)
                return null;

            using (var ms = new MemoryStream())
            {
                ms.Position = 0;
                var formatter = new BinaryFormatter();
                var obj = formatter.Deserialize(ms);
                ms.Close();

                return obj;
            }
        }

        #endregion
    }
}