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
            var prefabSource = serializedObject.FindProperty("m_PrefabSourec");
            EditorGUILayout.PropertyField(prefabSource);

            serializedObject.ApplyModifiedProperties();
        }
    }
}