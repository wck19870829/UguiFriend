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

        protected override void OnEnable()
        {
            base.OnEnable();

            keyCodeProp = serializedObject.FindProperty("m_KeyCode");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(keyCodeProp);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}