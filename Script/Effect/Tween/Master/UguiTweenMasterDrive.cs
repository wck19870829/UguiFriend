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
    public class UguiTweenMasterDrive
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

    [Serializable]
    public class UguiTweenMasterDriveVector2 : UguiTweenMasterDrive<Vector2>
    {

    }

    [Serializable]
    public class UguiTweenMasterDriveVector3 : UguiTweenMasterDrive<Vector3>
    {

    }

    [Serializable]
    public class UguiTweenMasterDriveVector4 : UguiTweenMasterDrive<Vector4>
    {

    }

    [Serializable]
    public class UguiTweenMasterDriveColor : UguiTweenMasterDrive<Color>
    {

    }

    [Serializable]
    public class UguiTweenMasterDriveColor32 : UguiTweenMasterDrive<Color32>
    {

    }

    [Serializable]
    public class UguiTweenMasterDriveBounds : UguiTweenMasterDrive<Bounds>
    {

    }

    [Serializable]
    public class UguiTweenMasterDriveRect : UguiTweenMasterDrive<Rect>
    {

    }

    [Serializable]
    public class UguiTweenMasterDriveFloat : UguiTweenMasterDrive<float>
    {

    }

    [Serializable]
    public class UguiTweenMasterDriveInt : UguiTweenMasterDrive<int>
    {

    }

    [Serializable]
    public class UguiTweenMasterDriveUint : UguiTweenMasterDrive<uint>
    {

    }

    [Serializable]
    public class UguiTweenMasterDriveLong : UguiTweenMasterDrive<long>
    {

    }

    [Serializable]
    public class UguiTweenMasterDriveChar : UguiTweenMasterDrive<char>
    {

    }

    [Serializable]
    public class UguiTweenMasterDriveString : UguiTweenMasterDrive<string>
    {

    }
}