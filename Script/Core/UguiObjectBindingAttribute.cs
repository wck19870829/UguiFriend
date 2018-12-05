using UnityEngine;
using System.Collections;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 数据类型绑定实体类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =false)]
    public class UguiObjectBindingAttribute : Attribute
    {
        public Type type;

        public UguiObjectBindingAttribute(Type type)
        {
            this.type = type;
        }
    }
}