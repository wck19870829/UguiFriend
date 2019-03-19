using UnityEngine;
using System.Collections;
using UnityEditor;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiConditionRecoverer),true)]
    public class UguiConditionRecovererEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var updateMode = serializedObject.FindProperty("m_UpdateMode");
            EditorGUILayout.PropertyField(updateMode);

            serializedObject.ApplyModifiedProperties();
        }
    }
}