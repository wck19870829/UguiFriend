using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 数据
    /// </summary>
    public abstract class UguiObjectData
    {
        static Dictionary<Type, UguiBindingAttribute> s_DataBindingDict;

        public string name;
        public string guid;
        public string parent;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public string tag;
        public int layer;

        static UguiObjectData()
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

        /// <summary>
        /// 深度复制
        /// </summary>
        /// <returns></returns>
        public virtual UguiObjectData DeepClone()
        {
            return MemberwiseClone() as UguiObjectData;
        }

        /// <summary>
        /// 获取实体预设
        /// 默认使用Resources加载，如使用其他加载方式重写此方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual UguiObject GetPrefabSource()
        {
            var bindInfo = s_DataBindingDict[GetType()];

            return Resources.Load<UguiObject>(bindInfo.prefabPath);
        }

        public UguiBindingAttribute GetBindingInfo()
        {
            return s_DataBindingDict[GetType()];
        }
    }
}