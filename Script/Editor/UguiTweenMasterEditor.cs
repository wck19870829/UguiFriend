using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiTweenMaster))]
    public class UguiTweenMasterEditor : UguiTweenEditor
    {
        protected SerializedProperty driveAllProp;
        protected HashSet<string> driveActiveSet;
        protected Object cacheTarget;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (driveActiveSet == null) driveActiveSet = new HashSet<string>();

            var master = target as UguiTweenMaster;
            var component = serializedObject.FindProperty("m_Component");
            if (cacheTarget!=component.objectReferenceValue)
            {
                cacheTarget = component.objectReferenceValue;
                driveAllProp = null;

                if (cacheTarget!=null)
                {
                    var driveAll = new List<UguiTweenMasterDrive>();
                    var bindingAttr = BindingFlags.Instance | BindingFlags.Public;
                    var propArr = cacheTarget.GetType().GetProperties(bindingAttr);
                    foreach (var prop in propArr)
                    {
                        //绘制可驱动属性
                        if (prop.CanRead && prop.CanWrite)
                        {
                            if (UguiTweenMultDrive.CanDrive(prop.PropertyType))
                            {
                                var from = prop.GetValue(cacheTarget, new object[0]);
                                var to = from;
                                var driveInfo = new UguiTweenMasterDrive(
                                    from,
                                    to,
                                    from.GetType(),
                                    prop.Name,
                                    AnimationCurve.EaseInOut(0,0,1,1)
                                );
                                driveAll.Add(driveInfo);
                            }
                        }
                    }
                    master.driveList = driveAll;
                    serializedObject.Update();
                    Debug.Log("Dirty");
                }
            }

            var driveListProp = serializedObject.FindProperty("driveList");
            EditorGUILayout.PropertyField(driveListProp, true);

            //var driveList = serializedObject.FindProperty("driveList");
            ////EditorGUILayout.PropertyField(driveList,true);
            //for (var i=0;i<driveList.arraySize;i++)
            //{
            //    var itemProp=(driveList.GetArrayElementAtIndex(i));
            //    itemProp.FindPropertyRelative("propName").stringValue="xxxx";
            //}

            //if (driveAllListProp != null)
            //{
            //    for (var i=0;i< driveAllListProp.arraySize;i++)
            //    {
            //        var item = driveAllListProp.GetArrayElementAtIndex(i);
            //    }
            //}

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomPropertyDrawer(typeof(UguiTweenMasterDrive),true)]
    public class UguiTweenMasterDriveEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var activeRect = new Rect(position.position,new Vector2(position.height, position.height));
            EditorGUI.PropertyField(activeRect,property.FindPropertyRelative("active"),new GUIContent(""));
            //EditorGUILayout.PropertyField(property.FindPropertyRelative("propName"));
            //EditorGUILayout.PropertyField(property.FindPropertyRelative("animationCurve"));
            //var rawDataFrom=property.FindPropertyRelative("rawDataFrom").stringValue;
            //var rawDataTo = property.FindPropertyRelative("rawDataTo").stringValue;
        }
    }
}