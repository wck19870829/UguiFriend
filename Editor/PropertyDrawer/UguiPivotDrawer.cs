using UnityEngine;
using System.Collections;
using UnityEditor;

namespace RedScarf.UguiFriend
{
    [CustomPropertyDrawer(typeof(UguiPivot), true)]
    public class UguiPivotDrawer : PropertyDrawer
    {
        const int pivotTexSize = 64;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var tex=UguiEditorTools.LoadTex("Pivot");
            EditorGUI.DrawTextureTransparent(position, tex,ScaleMode.ScaleToFit);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return pivotTexSize;
        }
    }
}