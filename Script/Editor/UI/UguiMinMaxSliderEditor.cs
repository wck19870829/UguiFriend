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

            var limitLineStart = serializedObject.FindProperty("m_LimitLineStart");
            EditorGUILayout.PropertyField(limitLineStart);

            var limitLineEnd = serializedObject.FindProperty("m_LimitLineEnd");
            EditorGUILayout.PropertyField(limitLineEnd);

            var fillRect = serializedObject.FindProperty("m_FillRect");
            EditorGUILayout.PropertyField(fillRect);

            var handleRect=serializedObject.FindProperty("m_HandleRect");
            EditorGUILayout.PropertyField(handleRect);

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