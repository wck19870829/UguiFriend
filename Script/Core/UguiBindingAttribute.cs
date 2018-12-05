using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    /// <summary>
    /// 数据绑定到实体
    /// </summary>
    public class UguiBindingAttribute : Attribute
    {
        public string prefabPath;
        public Type entityType;

        public UguiBindingAttribute(Type entityType, string prefabPath = "")
        {
            this.entityType = entityType;
            this.prefabPath = prefabPath;
        }
    }
}