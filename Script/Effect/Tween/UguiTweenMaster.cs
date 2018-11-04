using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Reflection;
using System.Text;

namespace RedScarf.UguiFriend
{
    [Serializable]
    public class UguiTweenMaster : UguiTween
    {
        static readonly Dictionary<Type, Func<object, object, float, object>> internalTypeLerpDict;

        [SerializeField] protected Component m_Component;
        [SerializeField] public List<UguiTweenMasterDrive> driveList;

        static UguiTweenMaster()
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

        public override void UpdateProgress(float progress)
        {
            foreach (var drive in driveList)
            {

            }
        }

        protected override void UpdateValue(float t)
        {

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
                Vector3.Lerp(bFrom.center, bTo.center, t),
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
            for (var i = 0; i < len; i++)
            {
                var chFrom = (i < strFrom.Length) ? strFrom[i] : char.MinValue;
                var chTo = (i < strTo.Length) ? strTo[i] : char.MinValue;
                strBuilder.Append(CharLerp(chFrom, chTo, t));
            }

            return strBuilder.ToString();
        }

        #endregion
    }

    [Serializable]
    public class UguiTweenMasterDrive//,ISerializationCallbackReceiver
    {
        public bool active;
        public string propName;
        public Type valueType;
        public object from;
        public object to;
        public AnimationCurve animationCurve;

        public string rawDataValueType;
        public string rawDataFrom;
        public string rawDataTo;

        public UguiTweenMasterDrive()
        {
            
        }

        public UguiTweenMasterDrive(object from, object to, Type valueType, string propName, AnimationCurve animationCurve)
        {
            this.from = from;
            this.to = to;
            this.propName = propName;
            this.valueType = valueType;
            this.animationCurve = animationCurve;
        }

        public void OnAfterDeserialize()
        {
            valueType = Type.GetType(rawDataValueType);
            from = null;
            to = null;
            if (valueType != null)
            {
                from = UguiTools.DeserializeObject(UguiTools.String2Bytes(rawDataFrom));
                to = UguiTools.DeserializeObject(UguiTools.String2Bytes(rawDataTo));
            }
        }

        public void OnBeforeSerialize()
        {
            rawDataFrom = UguiTools.Bytes2String(UguiTools.SerializeObject(from));
            rawDataTo = UguiTools.Bytes2String(UguiTools.SerializeObject(to));
            rawDataValueType = (valueType == null) ?
                                string.Empty :
                                valueType.FullName;
        }
    }
}