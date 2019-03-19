using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiGridLayoutGroup),true)]
    public class UguiGridLayoutGroupEditor : UguiLayoutGroupEditor
    {
        protected override void DrawHead()
        {
            base.DrawHead();

            var cellSize = serializedObject.FindProperty("m_CellSize");
            EditorGUILayout.PropertyField(cellSize);

            var spacing = serializedObject.FindProperty("m_Spacing");
            EditorGUILayout.PropertyField(spacing);

            var constraint = serializedObject.FindProperty("m_Constraint");
            EditorGUILayout.PropertyField(constraint);

            if (constraint.intValue == (int)UguiGridLayoutGroup.Constraint.FixedColumnCount
                || constraint.intValue == (int)UguiGridLayoutGroup.Constraint.FixedRowCount)
            {
                var flexibleCount = serializedObject.FindProperty("m_FlexibleCount");
                EditorGUILayout.PropertyField(flexibleCount);
            }
        }
    }
}