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
        public Type dataType;

        public UguiBindingAttribute(Type dataType, string prefabPath = "")
        {
            this.dataType = dataType;
            this.prefabPath = prefabPath;
        }
    }
}