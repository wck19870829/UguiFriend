using UnityEngine;
using System.Collections;
using UnityEditor;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiLayerInfoConfig))]
    public class UguiLayerInfoConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var nameList = serializedObject.FindProperty("nameList");
            for (var i = 0; i < nameList.arraySize; i++)
            {
                var sp = nameList.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(sp, new GUIContent(i.ToString()));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}