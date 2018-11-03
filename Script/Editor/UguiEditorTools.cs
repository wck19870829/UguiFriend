using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Reflection;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 编辑器工具
    /// </summary>
    public static class UguiEditorTools
    {
        static UguiEditorTools()
        {

        }

        public static void DrawObject(object target)
        {
            var type = target.GetType();
            var attrs=type.GetCustomAttributes(typeof(PropertyDrawer), true);
            if (attrs!=null&&attrs.Length>0)
            {
                Debug.LogFormat("Type:{0}", target);
            }
            //if (type.IsAssignableFrom(typeof(Vector2)))
            //{

            //}

            ////使用内置渲染
            //EditorGUILayout.PropertyField(property, label, includeChildren, options);
        }
    }
}