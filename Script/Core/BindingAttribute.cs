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
    public class BindingAttribute : Attribute
    {
        public string prefabPath;
        public Type uiType;

        public BindingAttribute(Type uiType,string prefabPath="")
        {
            this.uiType = uiType;
            this.prefabPath = prefabPath;
        }
    }
}