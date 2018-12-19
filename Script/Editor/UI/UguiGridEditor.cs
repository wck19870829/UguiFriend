using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiGrid))]
    public class UguiGridEditor : GridLayoutGroupEditor
    {
        public override void OnInspectorGUI()
        {
            var prefabSource = serializedObject.FindProperty("m_PrefabSourec");
            EditorGUILayout.PropertyField(prefabSource);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}