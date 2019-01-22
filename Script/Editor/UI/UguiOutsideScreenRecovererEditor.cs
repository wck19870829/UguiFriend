using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace RedScarf.UguiFriend
{
    public class UguiOutsideScreenRecovererEditor : UguiConditionRecovererEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var clipKind = serializedObject.FindProperty("m_ClipKind");
            var select = (Enum)((UguiOutsideScreenRecoverer.ClipKinds)clipKind.intValue);
            clipKind.intValue = (int)((UguiOutsideScreenRecoverer.ClipKinds)EditorGUILayout.EnumPopup("Clip Kind", select));

            if(clipKind.intValue== (int)UguiOutsideScreenRecoverer.ClipKinds.RectLimit)
            {
                var rectLimit = serializedObject.FindProperty("m_RectLimit");
                EditorGUILayout.PropertyField(rectLimit);
            }
            else if (clipKind.intValue == (int)UguiOutsideScreenRecoverer.ClipKinds.ViewportLimit)
            {
                var viewPortDisplayRect = serializedObject.FindProperty("m_ViewPortDisplayRect");
                EditorGUILayout.PropertyField(viewPortDisplayRect);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}