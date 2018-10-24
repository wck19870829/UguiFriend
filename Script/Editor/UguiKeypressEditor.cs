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
        protected SerializedProperty shiftKeyCodeProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            keyCodeProp = serializedObject.FindProperty("m_KeyCode");
            shiftKeyCodeProp = serializedObject.FindProperty("m_ShiftKeyCode");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(keyCodeProp);
            EditorGUILayout.PropertyField(shiftKeyCodeProp);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}