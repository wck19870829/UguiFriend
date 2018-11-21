using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.AnimatedValues;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiTweenMaster))]
    public class UguiTweenMasterEditor : UguiTweenEditor
    {
        protected AnimBool selectAnimBool;
        protected AnimBool unselectAnimBool;
        protected Vector2 driveScrollPos;
        protected Dictionary<string, UguiTweenMasterDrive> driveAllDict;

        public override void OnInspectorGUI()
        {
            var master = target as UguiTweenMaster;
            if (selectAnimBool == null)
                selectAnimBool = new AnimBool(true);
            if (unselectAnimBool == null)
                unselectAnimBool = new AnimBool(true);
            if (driveAllDict == null)
                driveAllDict = new Dictionary<string, UguiTweenMasterDrive>();
            var component = serializedObject.FindProperty("m_Component");
            var selectList = serializedObject.FindProperty("m_DriveList");
            var cacheTarget = component.objectReferenceValue;

            base.OnInspectorGUI();

            if (cacheTarget != component.objectReferenceValue)
            {
                cacheTarget = component.objectReferenceValue;
                selectList.ClearArray();
                driveAllDict.Clear();
                master.DriveList.Clear();

                if (cacheTarget != null)
                {
                    var bindingAttr = BindingFlags.Instance | BindingFlags.Public;

                    //属性
                    var propArr = cacheTarget.GetType().GetProperties(bindingAttr);
                    foreach (var prop in propArr)
                    {
                        if (prop.CanRead && prop.CanWrite)
                        {
                            if (UguiTweenMaster.CanDrive(prop.PropertyType))
                            {
                                var from = prop.GetValue(cacheTarget, new object[0]);
                                var to = from;

                                var driveInfo = ScriptableObject.CreateInstance(UguiTweenMaster.GetDriveType(from.GetType())) as UguiTweenMasterDrive;
                                driveInfo.from = from;
                                driveInfo.to = to;
                                driveInfo.animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                                driveInfo.driveName = prop.Name;
                                driveInfo.driveType = UguiTweenMasterDrive.DriveType.Property;

                                driveAllDict.Add(driveInfo.driveName, driveInfo);
                            }
                        }
                    }

                    //字段
                    var fieldArr = cacheTarget.GetType().GetFields(bindingAttr);
                    foreach (var field in fieldArr)
                    {
                        if (UguiTweenMaster.CanDrive(field.FieldType))
                        {
                            var from = field.GetValue(cacheTarget);
                            var to = from;

                            var driveInfo = ScriptableObject.CreateInstance(UguiTweenMaster.GetDriveType(from.GetType())) as UguiTweenMasterDrive;
                            driveInfo.from = from;
                            driveInfo.to = to;
                            driveInfo.animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                            driveInfo.driveName = field.Name;
                            driveInfo.driveType = UguiTweenMasterDrive.DriveType.Field;

                            driveAllDict.Add(driveInfo.driveName, driveInfo);
                        }
                    }
                }
            }

            if (cacheTarget != null)
            {
                //绘制需要驱动的属性和字段
                UguiEditorTools.DrawFadeGroup(
                    selectAnimBool,
                    new GUIContent("Select"),
                    () => {
                        var removeList = new List<int>();
                        for (var i = 0; i < selectList.arraySize; i++)
                        {
                            using (var horizontalScope = new EditorGUILayout.HorizontalScope())
                            {
                                var isSelect = EditorGUILayout.Toggle(true, GUILayout.Width(EditorGUIUtility.singleLineHeight));
                                if (!isSelect)
                                {
                                    removeList.Add(i);
                                }
                                EditorGUILayout.PropertyField(selectList.GetArrayElementAtIndex(i));
                            }
                            EditorGUILayout.Space();
                        }
                        for (var i = removeList.Count - 1; i >= 0; i--)
                        {
                            selectList.DeleteArrayElementAtIndex(i);
                            master.DriveList.RemoveAt(i);
                        }
                    },
                    Repaint
                );

                serializedObject.Update();

                //绘制不需要驱动的属性和字段
                UguiEditorTools.DrawFadeGroup(
                    unselectAnimBool,
                    new GUIContent("Unselect"),
                    () =>
                    {
                        foreach (var item in driveAllDict.Values)
                        {
                            if (master.DriveList.IndexOf(item) < 0)
                            {
                                var isSelect = EditorGUILayout.ToggleLeft(item.driveName, false);
                                if (isSelect)
                                {
                                    master.DriveList.Add(item);
                                }
                            }
                        }
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
    public class UguiTweenMasterDriveEditor : PropertyDrawer
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