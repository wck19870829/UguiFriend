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
            var simpleDist = serializedObject.FindProperty("m_SimpleDist");
            EditorGUILayout.IntSlider(simpleDist,1,10);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}