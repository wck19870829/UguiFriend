﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiLine),true)]
    public class UguiLineEditor : RawImageEditor
    {
        public override void OnInspectorGUI()
        {
            var points = serializedObject.FindProperty("m_Points");
            var lineStyle = serializedObject.FindProperty("m_LineStyle");
            var thickness = serializedObject.FindProperty("m_Thickness");
            var simpleDistance = serializedObject.FindProperty("m_SimpleDistance");
            var gradient = serializedObject.FindProperty("m_Gradient");

            EditorGUILayout.PropertyField(lineStyle);
            EditorGUILayout.Slider(thickness, 0.1f, 100f);
            EditorGUILayout.PropertyField(gradient);

            var style = (UguiLine.LineStyle)lineStyle.intValue;
            if (style==UguiLine.LineStyle.Bezier)
            {
                EditorGUILayout.IntSlider(simpleDistance, UguiMathf.Bezier.minSimpleDistance, UguiMathf.Bezier.maxSimpleDistance);
            }
            else if(style == UguiLine.LineStyle.Straight)
            {

            }

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(points, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}