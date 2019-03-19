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
        static HashSet<string> driveAllSet;
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

            driveAllSet = new HashSet<string>();
            var bindingAttr = BindingFlags.Instance | BindingFlags.Public;

            //属性
            var propArr = cacheTarget.GetType().GetProperties(bindingAttr);
            foreach (var prop in propArr)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    if (UguiTweenMaster.CanDrive(prop.PropertyType))
                    {
                        driveAllSet.Add(prop.Name);
                    }
                }
            }

            //字段
            var fieldArr = cacheTarget.GetType().GetFields(bindingAttr);
            foreach (var field in fieldArr)
            {
                if (UguiTweenMaster.CanDrive(field.FieldType))
                {
                    driveAllSet.Add(field.Name);
                }
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
                foreach (var item in driveAllSet)
                {
                    var isSelect = cacheMaster.ContainDrive(item);
                    var newState = EditorGUILayout.ToggleLeft(item, isSelect);
                    if (isSelect != newState)
                    {
                        if (newState)
                        {
                            cacheMaster.AddDrive(item);
                        }
                        else
                        {
                            cacheMaster.RemoveDrive(item);
                        }
                    }
                }
            }
        }
    }
}