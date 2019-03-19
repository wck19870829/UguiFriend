using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiSuckImage), true)]
    public class UguiSuckImageEditor : RawImageEditor
    {
        public override void OnInspectorGUI()
        {
            var suckEffect = target as UguiSuckImage;
            var simpleDist = serializedObject.FindProperty("m_SimpleDist");
            var blackHolePoint = serializedObject.FindProperty("m_BlackHolePoint");
            var percent = serializedObject.FindProperty("m_Percent");
            var duration = serializedObject.FindProperty("m_Duration");

            EditorGUILayout.IntSlider(simpleDist,UguiSuckImage.minSimpleDist,UguiSuckImage.maxSimpleDist);
            EditorGUILayout.PropertyField(blackHolePoint);
            EditorGUILayout.Slider(percent, 0, 1);
            duration.floatValue = EditorGUILayout.FloatField("Duration",duration.floatValue);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (Application.isPlaying)
            {
                using (var scope = new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Storage"))
                    {
                        suckEffect.Storage();
                    }
                    if (GUILayout.Button("TakeOut"))
                    {
                        suckEffect.TakeOut();
                    }
                }
            }
        }

        protected virtual void OnSceneGUI()
        {
            var suckEffect = target as UguiSuckImage;

            var cacheColor=Handles.color;
            Handles.color = new Color(0, 1, 0, 0.6f);

            //绘制黑洞吸入点
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