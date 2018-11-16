using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;

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

        public static void DrawSphereCap()
        {

        }

        static Vector2 startPoint;

        /// <summary>
        /// 绘制选取
        /// </summary>
        public static void DrawSelection()
        {
            if (Event.current != null)
            {
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        startPoint = Event.current.mousePosition;
                        break;

                    case EventType.MouseDrag:
                        var endPoint = Event.current.mousePosition;
                        Handles.BeginGUI();
                        Handles.RectangleCap(0, startPoint, Quaternion.identity, 100);
                        Handles.EndGUI();
                        break;

                    case EventType.MouseUp:

                        break;
                }
            }
        }

        /// <summary>
        /// 绘制可隐藏的属性
        /// </summary>
        /// <param name="stateFoldout"></param>
        /// <param name="guiContent"></param>
        /// <param name="drawContentAction"></param>
        /// <param name="onValueChanged"></param>
        public static void DrawFadeGroup(AnimBool stateFoldout,GUIContent guiContent,Action drawContentAction,UnityAction onValueChanged)
        {
            if (stateFoldout == null)
                return;
            if (onValueChanged!=null)
            {
                stateFoldout.valueChanged.RemoveListener(onValueChanged);
                stateFoldout.valueChanged.AddListener(onValueChanged);
            }

            stateFoldout.target = EditorGUILayout.Foldout(stateFoldout.target, guiContent);

            using (var fadeScope = new EditorGUILayout.FadeGroupScope(stateFoldout.faded))
            {
                if (fadeScope.visible)
                {
                    if (drawContentAction != null)
                    {
                        drawContentAction.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// 绘制属性
        /// </summary>
        /// <param name="position"></param>
        /// <param name="props"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取属性的高度
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
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