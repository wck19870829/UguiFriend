using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RedScarf.UguiFriend
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =false)]
    /// <summary>
    /// DisplayObjectData绑定DisplayObject
    /// </summary>
    public class UguiBindingAttribute : Attribute
    {
        public string prefabPath;
        public Type uiType;

        public UguiBindingAttribute(Type uiType,string prefabPath="")
        {
            this.uiType = uiType;
            this.prefabPath = prefabPath;
        }
    }
}