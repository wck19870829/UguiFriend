using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEditorInternal;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiLayer))]
    public class UguiLayerEditor : Editor
    {
        const string configPath = "UguiFriend/Config/LayerInfoConfig";

        public override void OnInspectorGUI()
        {
            var layer = serializedObject.FindProperty("m_Layer");
            var order = serializedObject.FindProperty("m_Order");
            var globalOrder = serializedObject.FindProperty("m_GlobalOrder");
            var autoSort = serializedObject.FindProperty("m_AutoSort");

            var enabled = GUI.enabled;

            //设置层
            var infoConfig = Resources.Load<LayerInfoConfig>(configPath);
            var displayOptions = new List<GUIContent>();
            var optionValues = new List<int>();
            for (var i = 0; i < infoConfig.nameList.Count; i++)
            {
                var name = infoConfig.nameList[i];
                if (!string.IsNullOrEmpty(name))
                {
                    displayOptions.Add(new GUIContent(name));
                    optionValues.Add(i);
                }
            }
            using (var scope=new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.IntPopup(layer, displayOptions.ToArray(), optionValues.ToArray());
                if (GUILayout.Button("Editor", GUILayout.Width(50)))
                {
                    Selection.activeObject = infoConfig;
                }
            }

            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                autoSort.boolValue = EditorGUILayout.ToggleLeft("AutoSort", autoSort.boolValue,GUILayout.Width(80));

                GUILayout.FlexibleSpace();

                GUI.enabled = !autoSort.boolValue;
                EditorGUILayout.PropertyField(order);
                GUI.enabled = true;
            }

            GUI.enabled = enabled;

            serializedObject.ApplyModifiedProperties();
        }
    }

    [Serializable]
    /// <summary>
    /// 序列化保存
    /// </summary>
    public class LayerInfoConfig : ScriptableObject
    {
        public List<string> nameList;
    }

    [CustomEditor(typeof(LayerInfoConfig))]
    public class LayerInfoConfigEditor : Editor
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