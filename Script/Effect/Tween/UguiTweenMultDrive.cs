using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Text;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 缓动驱动多属性
    /// </summary>
    public class UguiTweenMultDrive : UguiTweenMultDrive<Component>
    {

    }

    /// <summary>
    /// 缓动驱动多属性
    /// </summary>
    public abstract class UguiTweenMultDrive<TComponent> : UguiTween
        where TComponent : Component
    {
        static readonly Dictionary<Type, Func<object, object, float, object>> internalTypeLerpDict;

        [SerializeField] protected TComponent m_Component;
        [SerializeField] protected UguiTweenDriveInfo[] driveList;

        static UguiTweenMultDrive()
        {
            internalTypeLerpDict = new Dictionary<Type, Func<object, object, float, object>>()
            {
                {typeof(Color),ColorLerp},
                {typeof(Color32),Color32Lerp},
                {typeof(Bounds),BoundsLerp},
                {typeof(Vector2),Vector2Lerp},
                {typeof(Vector3),Vector3Lerp },
                {typeof(Vector4),Vector4Lerp },
                {typeof(Quaternion),QuaternionLerp },
                {typeof(Rect),RectLerp },

                {typeof(float),FloatLerp },
                {typeof(int),IntLerp },
                {typeof(uint),UintLerp },
                {typeof(long),LongLerp },
                {typeof(char),CharLerp },
                {typeof(string),StringLerp }
            };
        }

        protected override void Awake()
        {
            if (m_Component == null)
            {
                m_Component = GetComponentInChildren<TComponent>();
                if (m_Component == null)
                    throw new Exception("Component is null.");
            }

            base.Awake();
        }

        protected override void UpdateValue(float t)
        {
            foreach (var item in driveList)
            {
                var propInfo=m_Component.GetType().GetProperty(item.driveName);
                if (propInfo != null)
                {
                    object value=null;
                    if (internalTypeLerpDict.ContainsKey(propInfo.PropertyType))
                    {
                        value = internalTypeLerpDict[propInfo.PropertyType].Invoke(item.from, item.to, t);
                    }
                    else if (typeof(ILerp).IsAssignableFrom(propInfo.PropertyType))
                    {
                        value = ((ILerp)propInfo.PropertyType).Lerp(item.from, item.to, t);
                    }
                    else
                    {
                        Debug.LogErrorFormat("类需要实现ILerp接口:{0}", propInfo.PropertyType);
                    }

                    if (value != null)
                    {
                        propInfo.SetValue(m_Component, value,new object[0]);
                    }
                }
            }
        }

        #region 常用类型插值计算

        static object ColorLerp(object from, object to, float t)
        {
            return Color.Lerp((Color)from, (Color)to, t);
        }

        static object Color32Lerp(object from, object to, float t)
        {
            return Color32.Lerp((Color32)from, (Color32)to, t);
        }

        static object BoundsLerp(object from, object to, float t)
        {
            var bFrom = (Bounds)from;
            var bTo = (Bounds)to;
            var bounds = new Bounds(
                Vector3.Lerp(bFrom.center,bTo.center,t),
                Vector3.Lerp(bFrom.size, bTo.size, t)
                );

            return bounds;
        }

        static object Vector2Lerp(object from, object to, float t)
        {
            return Vector2.Lerp((Vector2)from, (Vector2)to, t);
        }

        static object Vector3Lerp(object from, object to, float t)
        {
            return Vector3.Lerp((Vector3)from, (Vector3)to, t);
        }

        static object Vector4Lerp(object from, object to, float t)
        {
            return Vector4.Lerp((Vector4)from, (Vector4)to, t);
        }

        static object FloatLerp(object from, object to, float t)
        {
            return Mathf.Lerp((float)from, (float)to, t);
        }

        static object IntLerp(object from, object to, float t)
        {
            return (int)Mathf.Lerp((int)from, (float)to, t);
        }

        static object UintLerp(object from, object to, float t)
        {
            return (uint)Mathf.Lerp((uint)from, (uint)to, t);
        }

        static object LongLerp(object from, object to, float t)
        {
            return (long)Mathf.Lerp((long)from, (long)to, t);
        }

        static object QuaternionLerp(object from, object to, float t)
        {
            return Quaternion.Lerp((Quaternion)from, (Quaternion)to, t);
        }

        static object RectLerp(object from, object to, float t)
        {
            var rectFrom = (Rect)from;
            var rectTo = (Rect)to;
            var rect = new Rect();
            rect.center = Vector2.Lerp(rectFrom.center, rectTo.center, t);
            rect.size = Vector2.Lerp(rectFrom.size, rectTo.size, t);

            return rect;
        }

        static object CharLerp(object from, object to, float t)
        {
            var chFrom = (int)from;
            var chTo = (int)to;
            var value = (int)Mathf.Lerp(chFrom, chTo, t);

            return Convert.ToChar(value);
        }

        static object StringLerp(object from, object to, float t)
        {
            var strFrom = from.ToString();
            var strTo = to.ToString();
            var len = (int)Mathf.Lerp(strFrom.Length, strTo.Length, t);
            var strBuilder = new StringBuilder(len);
            for (var i=0;i<len;i++)
            {
                var chFrom = (i < strFrom.Length) ? strFrom[i] : char.MinValue;
                var chTo = (i < strTo.Length) ? strTo[i] : char.MinValue;
                strBuilder.Append(CharLerp(chFrom,chTo,t));
            }

            return strBuilder.ToString();
        }

        #endregion

        /// <summary>
        /// 是否为可驱动的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanDrive(Type type)
        {
            if (internalTypeLerpDict.ContainsKey(type))
                return true;

            if (typeof(ILerp).IsAssignableFrom(type))
                return true;

            return false;
        }
    }

    /// <summary>
    /// 插值接口
    /// </summary>
    public interface ILerp
    {
        object Lerp(object from, object to, float t);
    }

    [Serializable]
    /// <summary>
    /// 驱动的字段
    /// </summary>
    public class UguiTweenDriveInfo
    {
        public string driveName;
        public object from;
        public object to;

        public UguiTweenDriveInfo(string driveName, object from,object to)
        {
            this.driveName = driveName;
            this.from = from;
            this.to = to;
        }
    }
}