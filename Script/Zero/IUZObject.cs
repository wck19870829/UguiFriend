using UnityEngine;
using System.Collections;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// Ugui物体接口
    /// </summary>
    public interface IUZObject
    {
        object Data { get; set; }
    }

    public interface IUZObject<TData> : IUZObject
        where TData : IUZObjectData
    {
        new TData Data { get; set; }
    }

    /// <summary>
    /// 数据接口
    /// </summary>
    public interface IUZObjectData
    {
        object ID { get; }
    }

    public interface IUZObjectData<T> : IUZObjectData
    {
        new T ID { get; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    /// <summary>
    /// 数据绑定实体
    /// </summary>
    public class UZBindAttribute : Attribute
    {
        public string prefabPath;
        public Type uiType;

        public UZBindAttribute(Type uiType, string prefabPath = "")
        {
            this.uiType = uiType;
            this.prefabPath = prefabPath;
        }

        public void TestFunc()
        {
            Canvas c;

        }
    }
}