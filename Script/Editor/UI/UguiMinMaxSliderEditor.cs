using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiMinMaxSlider),true)]
    public class UguiMinMaxSliderEditor : SelectableEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var sliderBlockA=serializedObject.FindProperty("m_SliderBlockA");
            EditorGUILayout.PropertyField(sliderBlockA);

            var sliderBlockB=serializedObject.FindProperty("m_SliderBlockB");
            EditorGUILayout.PropertyField(sliderBlockB);

            var fillRect = serializedObject.FindProperty("m_FillRect");
            EditorGUILayout.PropertyField(fillRect);

            var handleRect=serializedObject.FindProperty("m_HandleRect");
            EditorGUILayout.PropertyField(handleRect);

            var direction=serializedObject.FindProperty("m_Direction");
            EditorGUILayout.PropertyField(direction);

            var minValue=serializedObject.FindProperty("m_MinValue");
            EditorGUILayout.PropertyField(minValue);

            var maxValue = serializedObject.FindProperty("m_MaxValue");
            EditorGUILayout.PropertyField(maxValue);

            var wholeNUmbers = serializedObject.FindProperty("m_WholeNUmbers");
            EditorGUILayout.PropertyField(wholeNUmbers);

            serializedObject.ApplyModifiedProperties();
        }
    }
}