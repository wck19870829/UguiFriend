using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiTweenMultDrive),true)]
    public class UguiTweenMultDriveEditor : Editor
    {
        protected SerializedProperty driveListProp;
        protected SerializedProperty componentProp;
        protected Dictionary<string, UguiTweenDriveInfo> driveDict;

        protected virtual void OnEnable()
        {
            driveListProp = serializedObject.FindProperty("driveList");
            componentProp = serializedObject.FindProperty("m_Component");
            driveDict = new Dictionary<string, UguiTweenDriveInfo>();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var component = target as UguiTweenMultDrive;
            var flags = BindingFlags.Instance|BindingFlags.NonPublic;
            var driveList = component.GetType().GetField("driveList", flags).GetValue(component)as UguiTweenMultDrive[];

            driveDict.Clear();
            for (var i=0;i< driveListProp.arraySize;i++)
            {
                var itemProp=driveListProp.GetArrayElementAtIndex(i);
                if (itemProp.objectReferenceValue!=null)
                {
                    var driveInfo = itemProp.objectReferenceValue as UguiTweenDriveInfo;
                    if (!driveDict.ContainsKey(driveInfo.fieldName))
                    {
                        driveDict.Add(driveInfo.fieldName, driveInfo);
                    }
                }
            }

            driveListProp.ClearArray();
            if (componentProp.objectReferenceValue!=null)
            {
                var bindingAttr = BindingFlags.Instance |
                    BindingFlags.Public;
                var propArr = componentProp.objectReferenceValue.GetType().GetProperties(bindingAttr);
                foreach (var prop in propArr)
                {
                    //绘制可驱动属性
                    if (prop.CanRead&&prop.CanWrite)
                    {
                        if (UguiTweenMultDrive.CanDrive(prop.PropertyType))
                        {
                            using (var scope = new EditorGUILayout.HorizontalScope())
                            {
                                var toggle = EditorGUILayout.Toggle(driveDict.ContainsKey(prop.Name));

                                var cacheEnabled = GUI.enabled;

                                GUI.enabled = toggle;
                                EditorGUILayout.LabelField(prop.Name);
                                if (!toggle)
                                {
                                    driveDict.Remove(prop.Name);
                                }
                                else
                                {
                                    if (!driveDict.ContainsKey(prop.Name))
                                    {
                                        driveDict.Add(prop.Name, new UguiTweenDriveInfo(prop.Name));
                                    }
                                }

                                GUI.enabled = cacheEnabled;
                            }
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomPropertyDrawer(typeof(UguiTweenDriveInfo))]
    public class UguiTweenDriveInfoEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.LabelField(label);
        }
    }
}