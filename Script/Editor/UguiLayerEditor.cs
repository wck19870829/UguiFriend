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
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = UguiEditorTools.defaultLabelWidth;

            //设置层
            var infoConfig = Resources.Load<UguiLayerInfoConfig>(configPath);
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
                autoSort.boolValue = EditorGUILayout.ToggleLeft("Auto order", autoSort.boolValue,GUILayout.Width(100));
                GUI.enabled = !autoSort.boolValue;
                order.intValue=Mathf.Clamp(EditorGUILayout.IntField(order.intValue,GUILayout.Width(40)),0, UguiLayer.orderMax);

                GUILayout.Space(20);

                GUI.enabled = false;
                EditorGUILayout.IntField("Global", globalOrder.intValue);
                GUI.enabled = true;
            }

            GUI.enabled = enabled;
            EditorGUIUtility.labelWidth=labelWidth;

            if (serializedObject.ApplyModifiedProperties())
            {
                UguiLayer.SetDirty();
            }
        }
    }
}