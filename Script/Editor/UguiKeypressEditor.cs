using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;
using System;

namespace RedScarf.UguiFriend
{
    //[CustomEditor(typeof(UguiKeypress), true)]
    public class UguiKeypressEditor : Editor
    {
        protected SerializedProperty keyCodeProp;
        protected SerializedProperty shiftKeyCodeProp;

        protected virtual void OnEnable()
        {
            keyCodeProp = serializedObject.FindProperty("m_KeyCode");
            shiftKeyCodeProp = serializedObject.FindProperty("m_ShiftKeyCode");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(keyCodeProp);
            EditorGUILayout.PropertyField(shiftKeyCodeProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}