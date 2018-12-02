using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;
using System;

namespace RedScarf.UguiFriend
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UguiKeypress), true)]
    public class UguiKeypressEditor : SelectableEditor
    {
        protected SerializedProperty keyCodeProp;
        protected SerializedProperty autoNameProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            autoNameProp= serializedObject.FindProperty("m_AutoName");
            keyCodeProp = serializedObject.FindProperty("m_RawKeyCode");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(keyCodeProp);
            EditorGUILayout.PropertyField(autoNameProp);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}