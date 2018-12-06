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
    }
}