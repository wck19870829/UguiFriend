using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEditorInternal;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiSortingLayer),true)]
    public class UguiSortingLayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var sortingLayerID = serializedObject.FindProperty("m_SortingLayerID");
            var sortingOrder = serializedObject.FindProperty("m_SortingOrder");
            var globalOrder = serializedObject.FindProperty("m_GlobalOrder");
            var autoSort = serializedObject.FindProperty("m_AutoSort");

            var enabled = GUI.enabled;
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = UguiEditorTools.defaultLabelWidth;

            UguiEditorTools.DrawSortingLayer(new GUIContent("Sorting Layer"),sortingLayerID);

            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                autoSort.boolValue = EditorGUILayout.ToggleLeft("Auto order", autoSort.boolValue,GUILayout.Width(EditorGUIUtility.labelWidth));
                GUI.enabled = !autoSort.boolValue;
                sortingOrder.intValue=Mathf.Clamp(EditorGUILayout.IntField(sortingOrder.intValue), UguiSortingLayer.sortingOrderMin, UguiSortingLayer.sortingOrderMax);
            }

            GUI.enabled = enabled;
            EditorGUIUtility.labelWidth=labelWidth;

            if (serializedObject.ApplyModifiedProperties())
            {
                UguiSortingLayer.SetDirty();
            }
        }
    }
}