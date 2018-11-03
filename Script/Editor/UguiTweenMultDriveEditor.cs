using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiTweenMultDrive),true)]
    public class UguiTweenMultDriveEditor : UguiTweenEditor
    {
        public override void OnInspectorGUI()
        {
            var driveListProp = serializedObject.FindProperty("driveList");
            var componentProp = serializedObject.FindProperty("m_Component");

            base.OnInspectorGUI();

            //if (cacheTarget != componentProp.objectReferenceValue)
            //{
            //    cacheTarget = componentProp.objectReferenceValue;
            //    drawAllInfoProp = null;

            //    if (cacheTarget != null)
            //    {
            //        drawAllInfo.Clear();

            //        var bindingAttr = BindingFlags.Instance |
            //                            BindingFlags.Public;
            //        var propArr = cacheTarget.GetType().GetProperties(bindingAttr);
            //        foreach (var prop in propArr)
            //        {
            //            //绘制可驱动属性
            //            if (prop.CanRead && prop.CanWrite)
            //            {
            //                if (UguiTweenMultDrive.CanDrive(prop.PropertyType))
            //                {
            //                    var from = prop.GetValue(cacheTarget, new object[0]);
            //                    var to = from;
            //                    var driveInfo = new UguiTweenDriveInfo(prop.Name, from, to);
            //                    drawAllInfo.Add(driveInfo);
            //                }
            //            }
            //        }
            //    }
            //}

            //if (drawAllInfoProp!=null)
            //{
            //    EditorGUILayout.PropertyField(drawAllInfoProp,true);
            //}

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomPropertyDrawer(typeof(UguiTweenDriveInfo))]
    public class UguiTweenDriveInfoPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (var scope=new EditorGUI.PropertyScope(position,new GUIContent("??"), property))
            {
                //EditorGUI.PropertyField(position,property.FindPropertyRelative("animationCurve"));
                //Debug.Log(driveInfo);

                EditorGUI.LabelField(position, "???");
            }
            //    EditorGUI.PrefixLabel(position, new GUIContent("??"));
            ////EditorGUI.DrawRect(position, Color.red);
            //EditorGUI.ToggleLeft(position, " ", false);
        }
    }
}