using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiLayoutGroup),true)]
    public class UguiLayoutGroupEditor : Editor
    {
        protected AnimBool betterEffectAnimBool;

        public override void OnInspectorGUI()
        {
            DrawHead();
            DrawFoot();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawHead()
        {
            var itemPrefabSource = serializedObject.FindProperty("m_ItemPrefabSource");
            EditorGUILayout.PropertyField(itemPrefabSource);

            //var childAlignment = serializedObject.FindProperty("m_ChildAlignment");
            //EditorGUILayout.PropertyField(childAlignment);

            //var padding = serializedObject.FindProperty("m_Padding");
            //EditorGUILayout.PropertyField(padding);
        }

        protected virtual void DrawFoot()
        {
            DrawUseBetterEffect();
        }

        protected void DrawUseBetterEffect()
        {
            if (betterEffectAnimBool == null)
            {
                betterEffectAnimBool = new AnimBool();
            }
            UguiEditorTools.DrawFadeGroup(
                betterEffectAnimBool,
                new GUIContent("Use better effect"),
                () =>
                {
                    var removeDelay = serializedObject.FindProperty("m_RemoveDelay");
                    EditorGUILayout.PropertyField(removeDelay);
                },
                Repaint
                );
        }
    }
}