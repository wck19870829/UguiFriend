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
            var assembly = Assembly.GetAssembly(type);

            ////使用内置渲染
            //EditorGUILayout.PropertyField(property, label, includeChildren, options);
        }
    }
}