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
        protected Dictionary<string, UguiTweenMasterDrive> unselectDriveDict;

        public override void OnInspectorGUI()
        {
            var master = target as UguiTweenMaster;
            if (selectAnimBool == null)
                selectAnimBool = new AnimBool();
            if (unselectAnimBool == null)
                unselectAnimBool = new AnimBool();
            if (unselectDriveDict == null)
                unselectDriveDict = new Dictionary<string, UguiTweenMasterDrive>();
            var component = serializedObject.FindProperty("m_Component");
            var selectList = serializedObject.FindProperty("m_DriveList");
            var cacheTarget = component.objectReferenceValue;

            base.OnInspectorGUI();

            if (cacheTarget != component.objectReferenceValue)
            {
                cacheTarget = component.objectReferenceValue;
                selectList.ClearArray();
                unselectDriveDict.Clear();
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
                            if (UguiTweenMultDrive.CanDrive(prop.PropertyType))
                            {
                                var from = prop.GetValue(cacheTarget, new object[0]);
                                var to = from;

                                var driveInfo = ScriptableObject.CreateInstance(UguiTweenMaster.GetDriveType(from.GetType())) as UguiTweenMasterDrive;
                                driveInfo.from = from;
                                driveInfo.to = to;
                                driveInfo.animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                                driveInfo.driveName = prop.Name;
                                driveInfo.driveType = UguiTweenMasterDrive.DriveType.Property;

                                unselectDriveDict.Add(driveInfo.driveName, driveInfo);
                            }
                        }
                    }

                    //字段
                    var fieldArr = cacheTarget.GetType().GetFields(bindingAttr);
                    foreach (var field in fieldArr)
                    {
                        if (UguiTweenMultDrive.CanDrive(field.FieldType))
                        {
                            var from = field.GetValue(cacheTarget);
                            var to = from;

                            var driveInfo = ScriptableObject.CreateInstance(UguiTweenMaster.GetDriveType(from.GetType())) as UguiTweenMasterDrive;
                            driveInfo.from = from;
                            driveInfo.to = to;
                            driveInfo.animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                            driveInfo.driveName = field.Name;
                            driveInfo.driveType = UguiTweenMasterDrive.DriveType.Field;

                            unselectDriveDict.Add(driveInfo.driveName, driveInfo);
                        }
                    }
                }
            }

            if (cacheTarget != null)
            {
                //绘制需要驱动的属性和字段
                var removeList = new List<int>();
                UguiEditorTools.DrawFadeGroup(
                    selectAnimBool,
                    new GUIContent("激活"),
                    () =>
                    {
                        for (var i = 0; i < selectList.arraySize; i++)
                        {
                            using (var horizontalScope = new EditorGUILayout.HorizontalScope())
                            {
                                var isSelect = EditorGUILayout.Toggle(true);
                                if (!isSelect)
                                {
                                    removeList.Add(i);
                                }
                                EditorGUILayout.PropertyField(selectList.GetArrayElementAtIndex(i));
                            }
                        }
                        for (var i = removeList.Count - 1; i >= 0; i--)
                        {
                            selectList.DeleteArrayElementAtIndex(i);
                        }
                    },
                    Repaint
                );

                EditorGUILayout.Space();

                //绘制不需要驱动的属性和字段
                UguiEditorTools.DrawFadeGroup(
                    unselectAnimBool,
                    new GUIContent("Property&Field"),
                    () =>
                    {
                        foreach (var item in unselectDriveDict.Values)
                        {
                            var isSelect = EditorGUILayout.ToggleLeft(item.driveName, false);
                            if (isSelect)
                            {
                                master.DriveList.Add(item);
                            }
                        }
                        foreach (var drive in master.DriveList)
                        {
                            unselectDriveDict.Remove(drive.driveName);
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
        protected SerializedObject so;
        protected List<SerializedProperty> props;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (var scope = new EditorGUI.PropertyScope(position, label, property))
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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SetDrawProps(property);

            return UguiEditorTools.GetPropsHeight(props);
        }

        protected virtual void SetDrawProps(SerializedProperty property)
        {
            if (property.objectReferenceValue == null)
                return;

            if (props == null)
                props = new List<SerializedProperty>();
            if (so == null)
                so = new SerializedObject(property.objectReferenceValue);

            props.Clear();

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