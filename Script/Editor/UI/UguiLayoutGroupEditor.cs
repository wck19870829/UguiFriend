using UnityEngine;
using System.Collections;
using UnityEditor;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiLayoutGroup),true)]
    public class UguiLayoutGroupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var prefabSource = serializedObject.FindProperty("m_PrefabSource");
            EditorGUILayout.PropertyField(prefabSource);

            var optimize = serializedObject.FindProperty("m_Optimize");
            EditorGUILayout.PropertyField(optimize);
            if (optimize.boolValue)
            {
                var viewPortDisplayRect = serializedObject.FindProperty("m_ViewPortDisplayRect");
                EditorGUILayout.PropertyField(viewPortDisplayRect);
            }

            var childAlignment = serializedObject.FindProperty("m_ChildAlignment");
            EditorGUILayout.PropertyField(childAlignment);

            var padding = serializedObject.FindProperty("m_Padding");
            EditorGUILayout.PropertyField(padding);

            serializedObject.ApplyModifiedProperties();
        }
    }
}