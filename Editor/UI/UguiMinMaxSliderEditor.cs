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

            EditorGUILayout.Space();

            var sliderBlockMin = serializedObject.FindProperty("m_SliderBlockMin");
            EditorGUILayout.PropertyField(sliderBlockMin);

            var sliderBlockMax = serializedObject.FindProperty("m_SliderBlockMax");
            EditorGUILayout.PropertyField(sliderBlockMax);

            var handleSlideArea = serializedObject.FindProperty("m_HandleSlideArea");
            EditorGUILayout.PropertyField(handleSlideArea);

            var minValueText = serializedObject.FindProperty("m_MinValueText");
            EditorGUILayout.PropertyField(minValueText);

            var maxValueText = serializedObject.FindProperty("m_MaxValueText");
            EditorGUILayout.PropertyField(maxValueText);

            var minLimitText = serializedObject.FindProperty("m_MinLimitText");
            EditorGUILayout.PropertyField(minLimitText);

            var maxLimitText = serializedObject.FindProperty("m_MaxLimitText");
            EditorGUILayout.PropertyField(maxLimitText);

            var minValue=serializedObject.FindProperty("m_MinValue");
            EditorGUILayout.PropertyField(minValue);

            var maxValue = serializedObject.FindProperty("m_MaxValue");
            EditorGUILayout.PropertyField(maxValue);

            var minLimit = serializedObject.FindProperty("m_MinLimit");
            EditorGUILayout.PropertyField(minLimit);

            var maxLimit = serializedObject.FindProperty("m_MaxLimit");
            EditorGUILayout.PropertyField(maxLimit);

            var direction = serializedObject.FindProperty("m_Direction");
            EditorGUILayout.PropertyField(direction);

            var wholeNumbers = serializedObject.FindProperty("m_WholeNumbers");
            EditorGUILayout.PropertyField(wholeNumbers);

            serializedObject.ApplyModifiedProperties();
        }
    }
}