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
            var duration = serializedObject.FindProperty("m_Duration");
            var playOnEnable = serializedObject.FindProperty("playOnEnable");
            var playStyle = serializedObject.FindProperty("m_PlayStyle");

            if (comp != null)
            {
                EditorGUILayout.PropertyField(comp, new GUIContent("Target"));
            }

            DrawFrom();
            DrawTo();

            if (duration != null)
            {
                EditorGUILayout.PropertyField(duration);
                duration.floatValue = Mathf.Max(0, duration.floatValue);
            }

            DrawAnimCurve();

            if (playOnEnable != null)
            {
                EditorGUILayout.PropertyField(playOnEnable);
            }

            if (playStyle != null)
            {
                EditorGUILayout.PropertyField(playStyle);
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawAnimCurve()
        {
            var animationCurve = serializedObject.FindProperty("m_AnimationCurve");
            if (animationCurve != null)
            {
                EditorGUILayout.PropertyField(animationCurve, GUILayout.Height(40));
            }
        }

        protected virtual void DrawFrom()
        {
            var from = serializedObject.FindProperty("m_From");
            if (from != null)
            {
                EditorGUILayout.PropertyField(from);
            }
        }

        protected virtual void DrawTo()
        {
            var to = serializedObject.FindProperty("m_To");
            if (to != null)
            {
                EditorGUILayout.PropertyField(to);
            }
        }
    }
}