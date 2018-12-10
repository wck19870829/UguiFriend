using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiTweenMaster),true)]
    public class UguiTweenMasterEditor : UguiTweenEditor
    {
        protected AnimBool selectAnimBool;
        protected AnimBool unselectAnimBool;
        protected Vector2 driveScrollPos;
        protected ReorderableList selectReoList;

        public override void OnInspectorGUI()
        {
            var master = target as UguiTweenMaster;
            if (selectAnimBool == null)
                selectAnimBool = new AnimBool(true);
            if (unselectAnimBool == null)
                unselectAnimBool = new AnimBool(true);
            var component = serializedObject.FindProperty("m_Component");
            var selectList = serializedObject.FindProperty("m_DriveList");

            base.OnInspectorGUI();

            if (component.objectReferenceValue != null)
            {
                //绘制需要驱动的属性和字段
                UguiEditorTools.DrawFadeGroup(
                    selectAnimBool,
                    new GUIContent("Drive list"),
                    () => {
                        if (selectReoList == null)
                        {
                            selectReoList = new ReorderableList(serializedObject, selectList);
                            selectReoList.drawElementCallback=(rect,index,isActive, isFocused) => 
                            {

                            };
                            selectReoList.onRemoveCallback = (list) => 
                            {

                            };
                            selectReoList.onAddCallback = (list)=>
                            {
                                UguiTweenMasterWindow.OpenWindow(component.objectReferenceValue,master);
                            };
                        }
                        selectReoList.DoLayoutList();
                    },
                    Repaint
                );
            }
            else
            {
                EditorGUILayout.HelpBox("选择组件!", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomPropertyDrawer(typeof(UguiTweenMasterDrive), true)]
    public class UguiTweenMasterDriveDrawer : PropertyDrawer
    {
        protected List<SerializedProperty> props;
        protected float height;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            height = 0f;
            var propHeight = 0f;
            if (property.objectReferenceValue!=null)
            {
                var so = new SerializedObject(property.objectReferenceValue);
                var indentLevel = EditorGUI.indentLevel;

                var driveName = so.FindProperty("driveName");
                propHeight = EditorGUI.GetPropertyHeight(driveName);
                EditorGUI.LabelField(new Rect(position.x, position.y+height, position.width, propHeight), driveName.stringValue);
                height += propHeight;

                EditorGUI.indentLevel++;

                var fromValue = so.FindProperty("fromValue");
                propHeight = EditorGUI.GetPropertyHeight(fromValue);
                EditorGUI.PropertyField(new Rect(position.x, position.y + height, position.width, propHeight), fromValue, new GUIContent("From"));
                height += propHeight;

                var toValue = so.FindProperty("toValue");
                propHeight = EditorGUI.GetPropertyHeight(fromValue);
                EditorGUI.PropertyField(new Rect(position.x, position.y + height, position.width, propHeight), toValue,new GUIContent("To"));
                height += propHeight;

                var animationCurve = so.FindProperty("animationCurve");
                propHeight = EditorGUI.GetPropertyHeight(animationCurve);
                EditorGUI.PropertyField(new Rect(position.x, position.y + height, position.width, propHeight), animationCurve);
                height += propHeight;

                EditorGUI.indentLevel = indentLevel;
                so.ApplyModifiedProperties();
            }
            else
            {
                EditorGUI.PropertyField(position,property,new GUIContent("null"));
                height+= EditorGUI.GetPropertyHeight(property);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return height;
        }
    }
}