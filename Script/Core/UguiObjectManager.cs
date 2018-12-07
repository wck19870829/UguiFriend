using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 管理器
    /// </summary>
    public sealed class UguiObjectManager
    {
        public static Func<UguiObjectData, UguiObject> onCreateByData;
        public static Func<Type, UguiObject> onCreateByType;

        static Dictionary<Type, Type> s_DataMappingDict;                    //Key:数据类型. Value:对象类型
        static Dictionary<Type, UguiBindingAttribute> s_BindingDict;        //Key:对象类型. Value:绑定信息
        static Dictionary<string, UguiObject> s_ObjectDict;

        static UguiObjectManager()
        {
            s_ObjectDict = new Dictionary<string, UguiObject>();
            s_DataMappingDict = new Dictionary<Type, Type>();
            s_BindingDict = new Dictionary<Type, UguiBindingAttribute>();
            var types = UguiTools.FindSubClass(typeof(UguiObject));
            foreach (var type in types)
            {
                if (type.IsAbstract) continue;

                var customAtts = type.GetCustomAttributes(typeof(UguiBindingAttribute), false);
                if (customAtts != null && customAtts.Length == 1)
                {
                    var customAtt = (UguiBindingAttribute)customAtts[0];
                    if (!customAtt.dataType.IsAbstract)
                    {
                        if (typeof(UguiObjectData).IsAssignableFrom(customAtt.dataType))
                        {
                            s_BindingDict.Add(type, customAtt);
                            s_DataMappingDict.Add(customAtt.dataType,type);
                            continue;
                        }
                    }
                }

                Debug.LogErrorFormat("数据绑定实体错误:{0}", type);
            }
        }

        /// <summary>
        /// 重载实体的预设路径
        /// </summary>
        /// <param name="config"></param>
        public static void OverloadPrefabPath(UguiObjectPrefabConfig config)
        {
            if (config == null) return;
            foreach (var info in config.infos)
            {
                try
                {
                    var type = Type.GetType(info.objType, true, true);
                    if (s_BindingDict.ContainsKey(type))
                    {
                        s_BindingDict[type].prefabPath = info.prefabPath;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="obj"></param>
        public static void Register(UguiObject obj)
        {
            if (obj == null) return;

            if (!s_ObjectDict.ContainsKey(obj.Guid))
            {
                s_ObjectDict.Add(obj.Guid, obj);
            }
            else
            {
                s_ObjectDict[obj.Guid] = obj;
            }
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="obj"></param>
        public static void Unregister(UguiObject obj)
        {
            if (obj == null) return;

            s_ObjectDict.Remove(obj.Guid);
        }

        /// <summary>
        /// 实体列表
        /// </summary>
        public static Dictionary<string, UguiObject> ObjectDict
        {
            get
            {
                return s_ObjectDict;
            }
        }

        /// <summary>
        /// 检测数据是否匹配实体
        /// </summary>
        /// <param name="objDataType"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        public static bool CheckMatch(UguiObjectData data,UguiObject obj)
        {
            if (data != null || obj != null)
            {
                if (s_DataMappingDict.ContainsKey(data.GetType()))
                {
                    if (s_DataMappingDict[data.GetType()] == obj.GetType())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 由类型创建新的实体
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        public static UguiObject CreateNew(Type objType)
        {
            if (onCreateByType != null)
            {
                return onCreateByType.Invoke(objType);
            }

            return null;
        }

        /// <summary>
        /// 由数据获取实体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static UguiObject CreateNew(UguiObjectData data)
        {
            if (onCreateByData != null)
            {
                return onCreateByData.Invoke(data);
            }

            return null;
        }
    }
}