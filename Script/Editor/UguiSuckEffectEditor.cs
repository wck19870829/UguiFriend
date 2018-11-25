using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiSuckEffect), true)]
    public class UguiSuckEffectEditor : RawImageEditor
    {
        public override void OnInspectorGUI()
        {
            var suckEffect = target as UguiSuckEffect;
            var simpleDist = serializedObject.FindProperty("m_SimpleDist");
            var blackHolePoint = serializedObject.FindProperty("m_BlackHolePoint");
            var percent = serializedObject.FindProperty("m_Percent");
            var duration = serializedObject.FindProperty("m_Duration");

            EditorGUILayout.IntSlider(simpleDist,UguiSuckEffect.minSimpleDist,UguiSuckEffect.maxSimpleDist);
            EditorGUILayout.PropertyField(blackHolePoint);
            EditorGUILayout.Slider(percent, 0, 1);
            duration.floatValue = EditorGUILayout.FloatField("Duration",duration.floatValue);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();

            EditorGUILayout.Space();

            using (var scope=new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Storage"))
                {
                    suckEffect.Storage();
                }
                if (GUILayout.Button("Take out"))
                {
                    suckEffect.TakeOut();
                }
            }
        }

        protected virtual void OnSceneGUI()
        {
            var suckEffect = target as UguiSuckEffect;

            var cacheColor=Handles.color;
            Handles.color = new Color(0, 1, 0, 0.6f);

            var blackHolePoint = serializedObject.FindProperty("m_BlackHolePoint");
            var worldPos = suckEffect.transform.TransformPoint(blackHolePoint.vector2Value);
            var controlPointSize = HandleUtility.GetHandleSize(worldPos) * 0.3f;
            var newWorldPos = Handles.FreeMoveHandle(worldPos, Quaternion.identity, controlPointSize, Vector3.one, Handles.SphereCap);
            blackHolePoint.vector2Value = suckEffect.transform.InverseTransformPoint(newWorldPos);

            Handles.color = cacheColor;

            serializedObject.ApplyModifiedProperties();
        }
    }
}