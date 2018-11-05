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
        protected Object cacheTarget;
        protected Vector2 driveScrollPos;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var master = target as UguiTweenMaster;
            var component = serializedObject.FindProperty("m_Component");
            if (cacheTarget!=component.objectReferenceValue)
            {
                cacheTarget = component.objectReferenceValue;

                if (cacheTarget!=null)
                {
                    var driveAll = new List<UguiTweenMasterDrive>();
                    var bindingAttr = BindingFlags.Instance | BindingFlags.Public;
                    var propArr = cacheTarget.GetType().GetProperties(bindingAttr);
                    foreach (var prop in propArr)
                    {
                        if (prop.CanRead && prop.CanWrite)
                        {
                            if (UguiTweenMultDrive.CanDrive(prop.PropertyType))
                            {
                                var from = prop.GetValue(cacheTarget, new object[0]);
                                var to = from;

                                var driveInfo = ScriptableObject.CreateInstance(UguiTweenMaster.GetDriveType(from.GetType())) as UguiTweenMasterDrive;
                                driveInfo.from = from;
                                driveInfo.to = to;
                                driveInfo.animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                                driveInfo.active = false;
                                driveInfo.propName = prop.Name;
                                driveAll.Add(driveInfo);
                            }
                        }
                    }
                }
            }

            if (component.objectReferenceValue != null)
            {
                //绘制激活属性
                var driveProp = serializedObject.FindProperty("driveList");
                if(driveProp.arraySize==0)driveProp.InsertArrayElementAtIndex(0);
                for (var i = 0; i < driveProp.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(driveProp.GetArrayElementAtIndex(i));
                }

                using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(driveScrollPos, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight*5)))
                {
                    //绘制未激活属性

                    driveScrollPos = scrollViewScope.scrollPosition;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("选择组件!", MessageType.Warning);
            }

            //var driveListProp = serializedObject.FindProperty("driveList");
            //EditorGUILayout.PropertyField(driveListProp, true);

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

    [CustomPropertyDrawer(typeof(UguiTweenMasterDrive), true)]
    public class UguiTweenMasterDriveEditor : PropertyDrawer
    {
        protected SerializedObject so;
        protected List<SerializedProperty> props;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (var scope=new EditorGUI.PropertyScope(position, label, property))
            {
                if (property.objectReferenceValue == null)
                {
                    EditorGUI.LabelField(position, new GUIContent("null"));
                }
                else
                {
                    SetDrawProps(property);

                    UguiEditorTools.DrawProps(position, props);

                    so.ApplyModifiedProperties();
                }
            }
        }

        //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        //{
        //    SetDrawProps(property);

        //    return UguiEditorTools.GetPropsHeight(props);
        //}

        protected virtual void SetDrawProps(SerializedProperty property)
        {
            if (property.objectReferenceValue == null)
                return;

            if (props == null)
                props = new List<SerializedProperty>();
            if (so == null)
                so = new SerializedObject(property.objectReferenceValue);

            props.Clear();

            var active = so.FindProperty("active");
            props.Add(active);

            var animationCurve = so.FindProperty("animationCurve");
            props.Add(animationCurve);

            var fromValue = so.FindProperty("fromValue");
            if (fromValue != null)
            {
                props.Add(fromValue);
            }
            var toValue = so.FindProperty("toValue");
            if (toValue != null)
            {
                props.Add(toValue);
            }
        }
    }
}