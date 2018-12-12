using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Reflection;
using System.Text;

namespace RedScarf.UguiFriend
{
    [DisallowMultipleComponent]
    [Serializable]
    public class UguiTweenMaster : UguiTween
    {
        static readonly Dictionary<Type, DriveLink> driveLinkDict;

        [SerializeField] protected Component m_Component;
        [SerializeField] protected List<UguiTweenMasterDrive> m_DriveList;

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
                {typeof(Rect),new DriveLink(RectLerp,typeof(UguiTweenMasterDriveRect)) },

                {typeof(float),new DriveLink(FloatLerp,typeof(UguiTweenMasterDriveFloat)) },
                {typeof(int),new DriveLink(IntLerp,typeof(UguiTweenMasterDriveInt)) },
                {typeof(uint),new DriveLink(UintLerp,typeof(UguiTweenMasterDriveUint)) },
                {typeof(long),new DriveLink(LongLerp,typeof(UguiTweenMasterDriveLong)) },
                {typeof(char),new DriveLink(CharLerp,typeof(UguiTweenMasterDriveChar)) },
                {typeof(string),new DriveLink(StringLerp,typeof(UguiTweenMasterDriveString)) }
            };
        }

        /// <summary>
        /// 添加一个驱动
        /// </summary>
        /// <param name="driveName"></param>
        /// <returns></returns>
        public UguiTweenMasterDrive AddDrive(string driveName)
        {
            if (m_Component == null)
                throw new Exception("Component is null.");

            var drive = m_DriveList.Find((x) => {return x.driveName == driveName; });
            if (drive == null)
            {
                var flags = BindingFlags.Public | BindingFlags.Instance;
                var field = m_Component.GetType().GetField(driveName, flags);
                if (field != null)
                {
                    var driveType=GetDriveType(field.FieldType);
                    drive=gameObject.AddComponent(driveType) as UguiTweenMasterDrive;
                    drive.driveType = UguiTweenMasterDrive.DriveType.Field;
                }
                else
                {
                    var prop = m_Component.GetType().GetProperty(driveName, flags);
                    if (prop == null)
                    {
                        throw new Exception("Drive name is invalid.");
                    }
                    else
                    {
                        var driveType = GetDriveType(prop.PropertyType);
                        drive = gameObject.AddComponent(driveType) as UguiTweenMasterDrive;
                        drive.driveType = UguiTweenMasterDrive.DriveType.Property;
                    }
                }
            }
            drive.driveName = driveName;
            m_DriveList.Add(drive);

            return drive;
        }

        /// <summary>
        /// 移除一个驱动
        /// </summary>
        /// <param name="driveName"></param>
        public void RemoveDrive(string driveName)
        {
            var removeList = m_DriveList.FindAll((x) =>
            {
                return x.driveName == driveName;
            });
            foreach (var item in removeList)
            {
                m_DriveList.Remove(item);
                DestroyImmediate(item);
            }
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="driveName"></param>
        /// <returns></returns>
        public bool ContainDrive(string driveName)
        {
            var index=m_DriveList.FindIndex((x) =>
            {
                return x.driveName == driveName;
            });

            return index >= 0 ? true : false;
        }

        public override void UpdateProgress(float progress)
        {
            if (m_Component != null)
            {
                foreach (var drive in m_DriveList)
                {
                    if (drive == null) continue;

                    drive.UpdateValue();
                    var progressValue= drive.animationCurve.Evaluate(progress);
                    var flags = BindingFlags.Public | BindingFlags.Instance;
                    switch (drive.driveType)
                    {
                        case UguiTweenMasterDrive.DriveType.Field:
                            if (drive.fInfo == null)
                                drive.fInfo = m_Component.GetType().GetField(drive.driveName, flags);
                            if (drive.fInfo != null)
                            {
                                var value = driveLinkDict[drive.fInfo.FieldType].lerpFunc(drive.from, drive.to, progressValue);
                                drive.fInfo.SetValue(m_Component, value);
                            }
                            break;

                        case UguiTweenMasterDrive.DriveType.Property:
                            if (drive.pInfo == null)
                                drive.pInfo = m_Component.GetType().GetProperty(drive.driveName, flags);
                            if (drive.pInfo != null)
                            {
                                var value = driveLinkDict[drive.pInfo.PropertyType].lerpFunc(drive.from, drive.to, progressValue);
                                drive.pInfo.SetValue(m_Component, value, new object[0]);
                            }
                            break;
                    }
                }
            }
        }

        protected override void UpdateValue(float t)
        {

        }

        public List<UguiTweenMasterDrive> DriveList
        {
            get
            {
                return m_DriveList;
            }
        }

        /// <summary>
        /// 能否驱动
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanDrive(Type type)
        {
            return driveLinkDict.ContainsKey(type);
        }

        /// <summary>
        /// 获取驱动属性绑定的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetDriveType(Type type)
        {
            if (driveLinkDict.ContainsKey(type))
            {
                return driveLinkDict[type].driveType;
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
            var chFrom = Convert.ToInt32(from);
            var chTo = Convert.ToInt32(to);
            var value = (int)Mathf.Lerp(chFrom, chTo, t);
            var ch= Convert.ToChar(value);

            return ch;
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
}