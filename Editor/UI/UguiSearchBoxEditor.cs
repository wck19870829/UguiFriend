using UnityEngine;
using System.Collections;
using UnityEditor.UI;
using UnityEditor;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiSearchBox),true)]
    public class UguiSearchBoxEditor : SelectableEditor
    {
        public override void OnInspectorGUI()
        {
            var searchBox = target as UguiSearchBox;

            base.OnInspectorGUI();

            EditorGUILayout.Space();

            var submitButton = serializedObject.FindProperty("m_SubmitButton");
            EditorGUILayout.PropertyField(submitButton);

            var clearAllHistoryButton = serializedObject.FindProperty("m_ClearAllHistoryButton");
            EditorGUILayout.PropertyField(clearAllHistoryButton);

            var closeHistoryButton = serializedObject.FindProperty("m_CloseHistoryButton");
            EditorGUILayout.PropertyField(closeHistoryButton);

            var inputField = serializedObject.FindProperty("m_InputField");
            EditorGUILayout.PropertyField(inputField);

            var historyContent = serializedObject.FindProperty("m_HistoryContent");
            EditorGUILayout.PropertyField(historyContent);

            var historyGrid = serializedObject.FindProperty("m_HistoryGrid");
            EditorGUILayout.PropertyField(historyGrid);

            var historyItemPrefabSource = serializedObject.FindProperty("m_HistoryItemPrefabSource");
            EditorGUILayout.PropertyField(historyItemPrefabSource);

            var cacheHistory = serializedObject.FindProperty("m_CacheHistory");
            EditorGUILayout.PropertyField(cacheHistory);
            if (cacheHistory.boolValue)
            {
                var cacheCount = serializedObject.FindProperty("m_CacheCount");
                EditorGUILayout.PropertyField(cacheCount);
            }

            var autoCloseHistoryWhenDeselect = serializedObject.FindProperty("m_AutoCloseHistoryWhenDeselect");
            EditorGUILayout.PropertyField(autoCloseHistoryWhenDeselect);

            EditorGUILayout.Space();

            if(GUILayout.RepeatButton(new GUIContent("Clear History Cache")))
            {
                searchBox.ClearHistoryCache();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}