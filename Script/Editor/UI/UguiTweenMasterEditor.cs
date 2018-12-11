using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiTweenMaster), true)]
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
            var driveList = serializedObject.FindProperty("m_DriveList");

            base.OnInspectorGUI();

            if (component.objectReferenceValue != null)
            {
                //绘制需要驱动的属性和字段
                UguiEditorTools.DrawFadeGroup(
                    selectAnimBool,
                    new GUIContent("Drive list"),() =>
                    {
                        if (selectReoList == null)
                        {
                            selectReoList = new ReorderableList(serializedObject, driveList);
                            selectReoList.elementHeight = 100;
                            selectReoList.drawElementCallback=(rect,index,isActive, isFocused) => 
                            {
                                EditorGUI.PropertyField(rect,selectReoList.serializedProperty.GetArrayElementAtIndex(index));
                            };
                            selectReoList.onRemoveCallback = (list) => 
                            {
                                master.DriveList.RemoveAt(selectReoList.index);
                            };
                            selectReoList.onAddCallback = (list)=>
                            {
                                UguiTweenMasterWindow.OpenWindow(component.objectReferenceValue,master);
                            };
                        }
                        var elementHeight = 60;
                        for (var i = 0;i< selectReoList.serializedProperty.arraySize; i++)
                        {
                            var itemHeight = (int)EditorGUI.GetPropertyHeight(selectReoList.serializedProperty.GetArrayElementAtIndex(i));
                            elementHeight = Mathf.Max(elementHeight, itemHeight);
                        }
                        selectReoList.elementHeight = elementHeight+10;
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

        protected override void DrawAnimCurve()
        {
            
        }
    }
}