using UnityEngine;
using System.Collections;
using UnityEditor.UI;
using UnityEditor;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiKeyboardInputField),true)]
    public class UguiKeyboardInputFieldEditor : InputFieldEditor
    {
        protected SerializedProperty uguiKeyboardProp;

        protected override void OnEnable()
        {
            uguiKeyboardProp = serializedObject.FindProperty("m_UguiKeyboard");

            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(uguiKeyboardProp);
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}