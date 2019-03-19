using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;
using System.IO;
using UnityEditorInternal;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 编辑器工具
    /// </summary>
    public static class UguiEditorTools
    {
        public const int defaultLabelWidth = 120;
        public static readonly string pluginsPath;
        public static readonly string resPath;
        static readonly string texPath;
        public static readonly string configPath;

        static UguiEditorTools()
        {
            var directories = Directory.GetDirectories(Application.dataPath, "UguiFriend", SearchOption.AllDirectories);
            foreach (var dir in directories)
            {
                var newDir = dir.Replace("\\","/");
                if (newDir.Contains("RedScarf/UguiFriend"))
                {
                    pluginsPath = "Assets"+newDir.Replace(Application.dataPath, "")+"/";
                    resPath = pluginsPath + "EditorResources/";
                    texPath = resPath + "Texture/";
                    configPath = resPath + "Config/";
                    break;
                }
            }
        }

        #region 顶部菜单面板工具

        #endregion

        /// <summary>
        /// 保存预设
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        public static void SaveConfig(ScriptableObject obj)
        {
            if (obj == null) return;

            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadConfig<T>()where T:ScriptableObject
        {
            var path = configPath + typeof(T).Name + ".asset";
            var config= AssetDatabase.LoadAssetAtPath<T>(path);
            if (config==null)
            {
                config = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(config, path);
            }

            return config;
        }

        public static Texture2D LoadTex(string name)
        {
            var path = texPath + name+".png";
            var tex=AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            return tex;
        }

        static Vector2 startPoint;

        /// <summary>
        /// 绘制层级排序
        /// </summary>
        /// <param name="title"></param>
        /// <param name="sp"></param>
        public static void DrawSortingLayer(GUIContent title,SerializedProperty sp)
        {
            using (var scope=new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(title);

                var layerName = SortingLayer.IDToName(sp.intValue);
                if (GUILayout.Button(layerName, EditorStyles.popup))
                {
                    var menu = new GenericMenu();
                    foreach(var layer in SortingLayer.layers)
                    {
                        menu.AddItem(
                                new GUIContent(layer.name),
                                layerName == layer.name,
                                (x)=> {
                                    sp.intValue = SortingLayer.NameToID(x.ToString());
                                    sp.serializedObject.ApplyModifiedProperties();
                                    UguiSortingLayer.SetDirty();
                                },
                                layer.name
                                );
                    }

                    menu.AddSeparator("");

                    menu.AddItem(
                        new GUIContent("Add Sorting Layer"),
                        false,
                        () =>
                        {
                            Selection.activeObject = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
                        }
                        );
                    menu.ShowAsContext();
                }
            }
        }

        /// <summary>
        /// 绘制选取
        /// </summary>
        public static void DrawSelection()
        {
            if (Event.current != null)
            {
                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        startPoint = Event.current.mousePosition;
                        break;

                    case EventType.MouseDrag:
                        var endPoint = Event.current.mousePosition;
                        Handles.BeginGUI();
                        Handles.RectangleCap(0, startPoint, Quaternion.identity, 100);
                        Handles.EndGUI();
                        break;

                    case EventType.MouseUp:

                        break;
                }
            }
        }

        /// <summary>
        /// 绘制可隐藏的属性
        /// </summary>
        /// <param name="stateFoldout"></param>
        /// <param name="guiContent"></param>
        /// <param name="drawContentAction"></param>
        /// <param name="onValueChanged"></param>
        public static void DrawFadeGroup(AnimBool stateFoldout, GUIContent guiContent, Action drawContentAction, UnityAction onValueChanged)
        {
            if (stateFoldout == null)
                return;
            if (onValueChanged != null)
            {
                stateFoldout.valueChanged.RemoveListener(onValueChanged);
                stateFoldout.valueChanged.AddListener(onValueChanged);
            }

            stateFoldout.target = EditorGUILayout.Foldout(stateFoldout.target, guiContent);

            using (var fadeScope = new EditorGUILayout.FadeGroupScope(stateFoldout.faded))
            {
                if (fadeScope.visible)
                {
                    if (drawContentAction != null)
                    {
                        drawContentAction.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// 绘制属性
        /// </summary>
        /// <param name="position"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        public static float DrawProps(Rect position, List<SerializedProperty> props)
        {
            if (props == null || props.Count == 0)
                return 0;

            var yOffset = position.y;
            for (var i = 0; i < props.Count; i++)
            {
                if (props[i] != null)
                {
                    var height = EditorGUI.GetPropertyHeight(props[i]);
                    var rect = new Rect(0, yOffset, position.width, height);
                    EditorGUI.PropertyField(rect, props[i]);
                    yOffset += height;
                }
            }

            return yOffset;
        }

        /// <summary>
        /// 获取属性的高度
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        public static float GetPropsHeight(List<SerializedProperty> props)
        {
            if (props == null || props.Count == 0)
                return 0;

            var height = 0f;
            foreach (var prop in props)
            {
                height += EditorGUI.GetPropertyHeight(prop);
            }

            return height;
        }

        /// <summary>
        /// 绘制方形
        /// </summary>
        /// <param name="rect"></param>
        public static void DrawRect(Rect rect,Color color)
        {
            var cacheColor = Handles.color;
            Handles.color = color;

            var p1 = new Vector2(rect.xMin, rect.yMin);
            var p2 = new Vector2(rect.xMax, rect.yMin);
            var p3 = new Vector2(rect.xMax, rect.yMax);
            var p4 = new Vector2(rect.xMin, rect.yMax);
            Handles.DrawLine(p1,p2);
            Handles.DrawLine(p2, p3);
            Handles.DrawLine(p3, p4);
            Handles.DrawLine(p4, p1);

            Handles.color = cacheColor;
        }
    }
}