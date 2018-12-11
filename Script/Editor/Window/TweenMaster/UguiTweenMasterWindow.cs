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
        static Dictionary<string, System.Type> fieldDict;
        static Dictionary<string, System.Type> propDict;
        static Dictionary<string, System.Type> driveAllDict;
        static Object cacheTarget;
        static UguiTweenMaster cacheMaster;
        protected Vector2 scrollPosition;

        public static void OpenWindow(Object target, UguiTweenMaster master)
        {
            Init(target, master);

            var window = EditorWindow.GetWindow<UguiTweenMasterWindow>();
            window.titleContent = new GUIContent("Tween Master Window");
            window.Show();
        }

        static void Init(Object target,UguiTweenMaster master)
        {
            cacheTarget = target;
            cacheMaster = master;

            fieldDict = new Dictionary<string, System.Type>();
            propDict = new Dictionary<string, System.Type>();
            driveAllDict = new Dictionary<string, System.Type>();
            var bindingAttr = BindingFlags.Instance | BindingFlags.Public;

            //属性
            var propArr = cacheTarget.GetType().GetProperties(bindingAttr);
            foreach (var prop in propArr)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    if (UguiTweenMaster.CanDrive(prop.PropertyType))
                    {
                        propDict.Add(prop.Name, UguiTweenMaster.GetDriveType(prop.PropertyType));
                    }
                }
            }
            foreach (var item in propDict)
            {
                driveAllDict.Add(item.Key,item.Value);
            }

            //字段
            var fieldArr = cacheTarget.GetType().GetFields(bindingAttr);
            foreach (var field in fieldArr)
            {
                if (UguiTweenMaster.CanDrive(field.FieldType))
                {
                    fieldDict.Add(field.Name, UguiTweenMaster.GetDriveType(field.FieldType));
                }
            }
            foreach (var item in fieldDict)
            {
                driveAllDict.Add(item.Key, item.Value);
            }
        }

        private void OnGUI()
        {
            if (cacheMaster == null)
            {

                return;
            }

            using(var scrollScope=new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollScope.scrollPosition;
                foreach (var item in driveAllDict)
                {
                    var isSelect = false;
                    foreach (var drive in cacheMaster.DriveList)
                    {
                        if (drive.driveName == item.Key)
                        {
                            isSelect = true;
                        }
                    }
                    var newState = EditorGUILayout.ToggleLeft(item.Key, isSelect);
                    if (isSelect != newState)
                    {
                        if (newState)
                        {
                            var driveConfig = cacheMaster.gameObject.AddComponent(item.Value) as UguiTweenMasterDrive;
                            driveConfig.driveName = item.Key;
                            if (fieldDict.ContainsKey(item.Key))
                            {
                                driveConfig.driveType = UguiTweenMasterDrive.DriveType.Field;
                            }
                            else if (propDict.ContainsKey(item.Key))
                            {
                                driveConfig.driveType = UguiTweenMasterDrive.DriveType.Property;
                            }
                            cacheMaster.DriveList.Add(driveConfig);
                        }
                        else
                        {
                            cacheMaster.DriveList.RemoveAll((x) =>
                            {
                                return x.driveName == item.Key;
                            });
                            var destroyList = cacheMaster.GetComponents<UguiTweenMasterDrive>();
                            foreach (var drive in destroyList)
                            {
                                if (drive.driveName==item.Key)
                                {
                                    DestroyImmediate(drive);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}