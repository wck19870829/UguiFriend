using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// TweenMaster窗口
    /// </summary>
    public class UguiTweenMasterWindow : EditorWindow
    {
        static Dictionary<string, UguiTweenMasterDrive> driveAllDict;
        static Object cacheTarget;
        static UguiTweenMaster cacheMaster;

        public static void OpenWindow(Object target, UguiTweenMaster master)
        {
            Init(target, cacheMaster);

            var window = EditorWindow.GetWindow<UguiTweenMasterWindow>();
            window.titleContent = new GUIContent("Tewwn Master Window");
            window.Show();
        }

        static void Init(Object target,UguiTweenMaster master)
        {
            cacheTarget = target;
            cacheMaster = master;

            driveAllDict = new Dictionary<string, UguiTweenMasterDrive>();
            var bindingAttr = BindingFlags.Instance | BindingFlags.Public;

            //属性
            var propArr = cacheTarget.GetType().GetProperties(bindingAttr);
            foreach (var prop in propArr)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    if (UguiTweenMaster.CanDrive(prop.PropertyType))
                    {
                        var from = prop.GetValue(cacheTarget, new object[0]);
                        var to = from;

                        var driveInfo = ScriptableObject.CreateInstance(UguiTweenMaster.GetDriveType(from.GetType())) as UguiTweenMasterDrive;
                        driveInfo.from = from;
                        driveInfo.to = to;
                        driveInfo.animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                        driveInfo.driveName = prop.Name;
                        driveInfo.driveType = UguiTweenMasterDrive.DriveType.Property;

                        driveAllDict.Add(driveInfo.driveName, driveInfo);
                    }
                }
            }

            //字段
            var fieldArr = cacheTarget.GetType().GetFields(bindingAttr);
            foreach (var field in fieldArr)
            {
                if (UguiTweenMaster.CanDrive(field.FieldType))
                {
                    var from = field.GetValue(cacheTarget);
                    var to = from;

                    var driveInfo = ScriptableObject.CreateInstance(UguiTweenMaster.GetDriveType(from.GetType())) as UguiTweenMasterDrive;
                    driveInfo.from = from;
                    driveInfo.to = to;
                    driveInfo.animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                    driveInfo.driveName = field.Name;
                    driveInfo.driveType = UguiTweenMasterDrive.DriveType.Field;

                    driveAllDict.Add(driveInfo.driveName, driveInfo);
                }
            }
        }

        private void OnGUI()
        {

        }
    }
}