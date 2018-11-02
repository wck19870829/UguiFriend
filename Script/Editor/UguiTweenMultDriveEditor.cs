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
        protected Object cacheTarget;
        protected SerializedProperty drawAllProp;

        protected virtual void OnEnable()
        {
            driveListProp = serializedObject.FindProperty("driveList");
            componentProp = serializedObject.FindProperty("m_Component");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var component = target as UguiTweenMultDrive;
            if(cacheTarget != componentProp.objectReferenceValue)
            {
                cacheTarget = componentProp.objectReferenceValue;

                var driveList = new List<UguiTweenDriveInfo>();
                if (componentProp.objectReferenceValue != null)
                {
                    var bindingAttr = BindingFlags.Instance |
                        BindingFlags.Public;
                    var propArr = componentProp.objectReferenceValue.GetType().GetProperties(bindingAttr);
                    foreach (var prop in propArr)
                    {
                        //绘制可驱动属性
                        if (prop.CanRead && prop.CanWrite)
                        {
                            if (UguiTweenMultDrive.CanDrive(prop.PropertyType))
                            {
                                var driveInfo = new UguiTweenDriveInfo(typeof(Transform),prop.Name);
                                driveList.Add(driveInfo);
                            }
                        }
                    }
                }
                var obj = new SerializedObject(driveList.ToArray());
                drawAllProp = obj.FindProperty("Array");
            }

            if (drawAllProp != null)
            {
                Debug.Log("draw");

                //using (var scope = new EditorGUILayout.ScrollViewScope(Vector2.zero))
                //{

                //}
                //var driveInfoListProp = drawWrapObj.FindProperty("driveInfoList");
                //for (var i = 0; i < driveInfoListProp.arraySize; i++)
                //{
                //    EditorGUILayout.PropertyField(driveInfoListProp.GetArrayElementAtIndex(i),new GUIContent("xx??"));
                //}
                //drawWrapObj.DrawArray("driveInfoList", "title");
            }

            //for (var i=0;i< driveListProp.arraySize;i++)
            //{
            //    var itemProp=driveListProp.GetArrayElementAtIndex(i);
            //    if (itemProp.objectReferenceValue!=null)
            //    {
            //        var driveInfo = itemProp.objectReferenceValue as UguiTweenDriveInfo;
            //        if (!drawObjectDict.ContainsKey(driveInfo.fieldName))
            //        {
            //            drawObjectDict.Add(driveInfo.fieldName, driveInfo);
            //        }
            //    }
            //}

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomPropertyDrawer(typeof(UguiTweenDriveInfo))]
    public class UguiTweenDriveInfoPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var driveInfo = property.serializedObject.targetObject as UguiTweenDriveInfo;

            var fromProp = property.FindPropertyRelative("from");
            //var toProp= property.serializedObject.FindProperty("to");
            //EditorGUILayout.PropertyField(fromProp);
            //EditorGUILayout.LabelField("????");
        }
    }
}