using UnityEngine;
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
            var subdivide = serializedObject.FindProperty("m_Subdivide");

            EditorGUILayout.PropertyField(lineStyle);
            EditorGUILayout.Slider(thickness, 0.1f, 100f);

            var style = (UguiLine.LineStyle)lineStyle.intValue;
            if (style==UguiLine.LineStyle.Bezier)
            {
                EditorGUILayout.IntSlider(subdivide, UguiMathf.Bezier.defaultSubdivide, 100);
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