using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

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

        public static float DrawProps(Rect position,List<SerializedProperty> props)
        {
            if (props == null||props.Count==0)
                return 0;

            var yOffset = position.y;
            for (var i=0;i<props.Count;i++)
            {
                if (props[i] != null)
                {
                    var height = EditorGUI.GetPropertyHeight(props[i]);
                    var rect = new Rect(0, yOffset, position.width, height);
                    EditorGUI.PropertyField(rect, props[i]);
                    yOffset += height;
                }
            }

            return yOffset;
        }

        public static float GetPropsHeight(List<SerializedProperty> props)
        {
            if (props == null || props.Count == 0)
                return 0;

            var height = 0f;
            foreach (var prop in props)
            {
                height+=EditorGUI.GetPropertyHeight(prop);
            }

            return height;
        }
    }
}