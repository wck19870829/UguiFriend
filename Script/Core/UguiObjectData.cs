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
        public string name;
        public string guid;
        public string parent;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public string tag;
        public int layer;

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
            var bindInfo = UguiObjectManager.Instance.GetBindingAtt(this);

            return Resources.Load<UguiObject>(bindInfo.prefabPath);
        }
    }
}