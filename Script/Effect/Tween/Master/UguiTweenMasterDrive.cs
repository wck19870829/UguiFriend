using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

namespace RedScarf.UguiFriend
{
    [Serializable]
    /// <summary>
    /// 驱动类,用于运行时修改字段或属性
    /// </summary>
    public class UguiTweenMasterDrive:MonoBehaviour
    {
        public string driveName;
        public AnimationCurve animationCurve;
        public object from;
        public object to;
        public DriveType driveType;
        public FieldInfo fInfo;
        public PropertyInfo pInfo;

        public enum DriveType
        {
            Property = 0,
            Field = 1
        }

        protected UguiTweenMasterDrive()
        {
            animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }
    }

    [Serializable]
    public class UguiTweenMasterDrive<T> : UguiTweenMasterDrive, ISerializationCallbackReceiver
    {
        public T fromValue;
        public T toValue;

        public void OnAfterDeserialize()
        {
            from = fromValue;
            to = toValue;
        }

        public void OnBeforeSerialize()
        {
            fromValue = (from == null) ? default(T) : (T)from;
            toValue = (to == null) ? default(T) : (T)to;
        }
    }
}