using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [CustomPropertyDrawer(typeof(UguiTweenMasterDrive),true)]
    public class UguiTweenMasterDriveDrawer : PropertyDrawer
    {
        protected SerializedObject so;
        protected float height;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            height = 0f;
            var propHeight = 0f;
            var indentLevel = EditorGUI.indentLevel;

            if (property.objectReferenceValue != null)
            {
                if (so == null)
                    so = new SerializedObject(property.objectReferenceValue);

                var driveName = so.FindProperty("driveName");
                propHeight = EditorGUI.GetPropertyHeight(driveName);
                EditorGUI.LabelField(new Rect(position.x, position.y + height, position.width, propHeight), driveName.stringValue,EditorStyles.boldLabel);
                height += propHeight;

                EditorGUI.indentLevel++;

                var fromValue = so.FindProperty("fromValue");
                if (fromValue != null)
                {
                    propHeight = EditorGUI.GetPropertyHeight(fromValue);
                    EditorGUI.PropertyField(new Rect(position.x, position.y + height, position.width, propHeight), fromValue, new GUIContent("From"));
                    height += propHeight;
                }

                var toValue = so.FindProperty("toValue");
                if (toValue != null)
                {
                    propHeight = EditorGUI.GetPropertyHeight(fromValue);
                    EditorGUI.PropertyField(new Rect(position.x, position.y + height, position.width, propHeight), toValue, new GUIContent("To"));
                    height += propHeight;
                }

                var animationCurve = so.FindProperty("animationCurve");
                propHeight = EditorGUI.GetPropertyHeight(animationCurve);
                EditorGUI.PropertyField(new Rect(position.x, position.y + height, position.width, propHeight), animationCurve);
                height += propHeight;

                EditorGUI.indentLevel = indentLevel;
                so.ApplyModifiedProperties();
            }
            else
            {
                EditorGUI.PropertyField(position,property);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Mathf.Max(EditorGUIUtility.singleLineHeight,height);
        }
    }
}