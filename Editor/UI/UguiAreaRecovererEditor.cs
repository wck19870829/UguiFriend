using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace RedScarf.UguiFriend
{
    public class UguiOutsideScreenRecovererEditor : UguiConditionRecovererEditor
    {
        [CustomEditor(typeof(UguiAreaRecoverer),true)]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var clipKind = serializedObject.FindProperty("m_ClipKind");
            var select = (Enum)((UguiAreaRecoverer.ClipKinds)clipKind.intValue);
            clipKind.intValue = (int)((UguiAreaRecoverer.ClipKinds)EditorGUILayout.EnumPopup("Clip Kind", select));

            if(clipKind.intValue== (int)UguiAreaRecoverer.ClipKinds.RectLimit||
                clipKind.intValue == (int)UguiAreaRecoverer.ClipKinds.Bounds)
            {
                var limitObject = serializedObject.FindProperty("m_LimitObject");
                EditorGUILayout.PropertyField(limitObject);
            }
            else if (clipKind.intValue == (int)UguiAreaRecoverer.ClipKinds.ViewportLimit)
            {
                var viewPortDisplayRect = serializedObject.FindProperty("m_ViewPortDisplayRect");
                EditorGUILayout.PropertyField(viewPortDisplayRect);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}