using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;
using System.IO;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 编辑器工具
    /// </summary>
    public static class UguiEditorTools
    {
        public const int defaultLabelWidth = 120;
        public static readonly string pluginsPath;

        static UguiEditorTools()
        {
            var directories = Directory.GetDirectories(Application.dataPath, "UguiFriend", SearchOption.AllDirectories);
            foreach (var dir in directories)
            {
                var newDir = dir.Replace("\\","/");
                if (newDir.Contains("RedScarf/UguiFriend"))
                {
                    pluginsPath = "Assets"+newDir.Replace(Application.dataPath, "");
                    break;
                }
            }
        }

        #region 顶部菜单面板工具

        //[MenuItem("Tools/UguiFriend/Update Object Prefab Config")]
        ///// <summary>
        ///// 更新数据与对象映射关系
        ///// </summary>
        //static void UpdateObjectPrefabConfig()
        //{
        //    var configPath = "UguiFriend/Config/ObjectPrefabConfig";
        //    var config=Resources.Load<UguiObjectPrefabConfig>(configPath);
        //    if (config==null)
        //    {
        //        config = ScriptableObject.CreateInstance<UguiObjectPrefabConfig>();
        //        AssetDatabase.CreateAsset(config, pluginsPath + "/Resources/" + configPath + ".asset");
        //        AssetDatabase.Refresh();
        //    }

        //    var typeSet = new HashSet<string>();
        //    config.infos.Clear();
        //    var allAssetPaths = AssetDatabase.GetAllAssetPaths();
        //    foreach (var assetPath in allAssetPaths)
        //    {
        //        var obj=AssetDatabase.LoadAssetAtPath<UguiObject>(assetPath);
        //        if (obj!=null)
        //        {
        //            var pathWithoutExtension = Path.GetDirectoryName(assetPath) + "/" + Path.GetFileNameWithoutExtension(assetPath);
        //            if (typeSet.Contains(obj.GetType().FullName))
        //            {
        //                Debug.LogErrorFormat("预设冗余:{0}", pathWithoutExtension);
        //            }

        //            var info = new UguiObjectPrefabConfigItem(obj.GetType().FullName, pathWithoutExtension);
        //            config.infos.Add(info);
        //        }
        //    }
        //}

        #endregion

        /// <summary>
        /// 获取游戏视图框
        /// </summary>
        /// <returns></returns>
        public static void GetGameViewRect(out int width, out int height)
        {
            var screenSize = UnityStats.screenRes.Split('×');
            width = int.Parse(screenSize[0]);
            height = int.Parse(screenSize[1]);
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
    }
}