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
        static readonly Dictionary<Type, DriveLink> driveLinkDict;

        [SerializeField] protected Component m_Component;
        [SerializeField] public List<UguiTweenMasterDrive> driveList;

        protected class DriveLink
        {
            public Func<object, object, float, object> lerpFunc;
            public Type driveType;

            public DriveLink(Func<object, object, float, object> lerpFunc,Type driveType)
            {
                this.lerpFunc = lerpFunc;
                this.driveType = driveType;
            }
        }

        static UguiTweenMaster()
        {
            driveLinkDict = new Dictionary<Type,DriveLink>()
            {
                {typeof(Color),new DriveLink(ColorLerp,typeof(UguiTweenMasterDriveColor))},
                {typeof(Color32),new DriveLink(Color32Lerp,typeof(UguiTweenMasterDriveColor32))},
                {typeof(Bounds),new DriveLink(BoundsLerp,typeof(UguiTweenMasterDriveBounds))},
                {typeof(Vector2),new DriveLink(Vector2Lerp,typeof(UguiTweenMasterDriveVector2))},
                {typeof(Vector3),new DriveLink(Vector3Lerp,typeof(UguiTweenMasterDriveVector3)) },
                {typeof(Vector4),new DriveLink(Vector4Lerp,typeof(UguiTweenMasterDriveVector4)) },
                {typeof(Quaternion),new DriveLink(QuaternionLerp,typeof(UguiTweenMasterDriveQuaternion))},
                {typeof(Rect),new DriveLink(RectLerp,typeof(UguiTweenMasterDriveRect)) },

                {typeof(float),new DriveLink(FloatLerp,typeof(UguiTweenMasterDriveFloat)) },
                {typeof(int),new DriveLink(IntLerp,typeof(UguiTweenMasterDriveInt)) },
                {typeof(uint),new DriveLink(UintLerp,typeof(UguiTweenMasterDriveUint)) },
                {typeof(long),new DriveLink(LongLerp,typeof(UguiTweenMasterDriveLong)) },
                {typeof(char),new DriveLink(CharLerp,typeof(UguiTweenMasterDriveChar)) },
                {typeof(string),new DriveLink(StringLerp,typeof(UguiTweenMasterDriveString)) }
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

        public static Type GetDriveType(Type valueType)
        {
            if (driveLinkDict.ContainsKey(valueType))
            {
                return driveLinkDict[valueType].driveType;
            }

            return null;
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
    public abstract class UguiTweenMasterDrive:ScriptableObject
    {
        public bool active;
        public string propName;
        public AnimationCurve animationCurve;
        public object from;
        public object to;
    }

    [Serializable]
    public class UguiTweenMasterDrive<T>: UguiTweenMasterDrive,ISerializationCallbackReceiver
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
            fromValue = (from==null)?default(T):(T)from;
            toValue = (to==null)?default(T):(T)to;
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
    public class UguiTweenMasterDriveQuaternion : UguiTweenMasterDrive<Quaternion>
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