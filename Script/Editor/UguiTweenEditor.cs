using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiTween), true)]
    public class UguiTweenEditor : Editor
    {
        protected AnimBool stateFoldout;

        protected virtual void OnEnable()
        {
            stateFoldout = new AnimBool();
        }

        public override void OnInspectorGUI()
        {
            var tween = (UguiTween)target;

            var comp = serializedObject.FindProperty("m_Component");
            if (comp != null)
            {
                EditorGUILayout.PropertyField(comp, new GUIContent("Target"));
            }

            var from = serializedObject.FindProperty("m_From");
            if (from != null)
            {
                EditorGUILayout.PropertyField(from);
            }

            var to = serializedObject.FindProperty("m_To");
            if (to != null)
            {
                EditorGUILayout.PropertyField(to);
            }

            var duration = serializedObject.FindProperty("m_Duration");
            if (duration != null)
            {
                EditorGUILayout.PropertyField(duration);
                duration.floatValue = Mathf.Max(0, duration.floatValue);
            }

            var animationCurve = serializedObject.FindProperty("m_AnimationCurve");
            if (animationCurve != null)
            {
                EditorGUILayout.PropertyField(animationCurve, GUILayout.Height(40));
            }

            var playOnEnable = serializedObject.FindProperty("playOnEnable");
            if (playOnEnable != null)
            {
                EditorGUILayout.PropertyField(playOnEnable);
            }

            var playStyle = serializedObject.FindProperty("m_PlayStyle");
            if (playStyle != null)
            {
                EditorGUILayout.PropertyField(playStyle);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}