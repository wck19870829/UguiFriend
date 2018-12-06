using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.IO;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 预设路径窗口
    /// </summary>
    public class UguiPrefabPathWindow : EditorWindow
    {
        static string savePath;
        static List<string> pathList=new List<string>();

        ReorderableList reorderableList;

        [MenuItem("Tools/UguiFriend/Prefab Path Window")]
        static void Init()
        {
            var window = GetWindow<UguiPrefabPathWindow>();
            window.titleContent = new GUIContent("Prefab path");
            window.Show();
        }

        private void OnGUI()
        {
            if (reorderableList == null)
            {
                reorderableList = new ReorderableList(pathList, typeof(string));
                reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUI.LabelField(rect, pathList[index]);
                };
                reorderableList.onAddCallback = (list) =>
                {
                    var folder=EditorUtility.OpenFolderPanel("Select folder.", Application.dataPath, "");
                    folder = "Assets" + folder.Replace(Application.dataPath,"");
                    if (!pathList.Contains(folder))
                    {
                        pathList.Add(folder);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Title", "Message", "Got it!");
                    }
                };
            }
            reorderableList.DoLayoutList();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save"))
            {
                var newSavePath = EditorUtility.SaveFilePanel("Save prefab path config", savePath, "PrefabPathConfig", "asset");
                if (!string.IsNullOrEmpty(newSavePath))
                {
                    savePath = newSavePath;

                    var config = ScriptableObject.CreateInstance<UguiObjectPrefabConfig>();
                    foreach (var path in pathList)
                    {
                        var allAssets=AssetDatabase.LoadAllAssetsAtPath(path);
                        foreach (var asset in allAssets)
                        {
                            var obj = asset as UguiObject;
                            if (obj!=null)
                            {
                                Debug.Log(obj);
                            }
                        }
                    }
                }
            }
        }
    }
}