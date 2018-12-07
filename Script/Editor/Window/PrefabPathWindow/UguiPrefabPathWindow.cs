using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.IO;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 预设路径窗口
    /// </summary>
    public class UguiPrefabPathWindow : EditorWindow
    {
        const string presetKey = "UguiPrefabPathWindowPreset";
        static UguiPrefabPathWindowPreset preset;

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
            if (preset == null)
            {
                preset = UguiEditorTools.LoadConfig<UguiPrefabPathWindowPreset>();
            }
            if (reorderableList == null)
            {
                reorderableList = new ReorderableList(preset.pathList, typeof(string));
                reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUI.LabelField(rect, preset.pathList[index]);
                };
                reorderableList.onAddCallback = (list) =>
                {
                    var folder = EditorUtility.OpenFolderPanel("Select folder.", Application.dataPath, "");
                    if (folder.StartsWith(Application.dataPath))
                    {
                        folder = "Assets" + folder.Replace(Application.dataPath, "");
                        if (!preset.pathList.Contains(folder))
                        {
                            preset.pathList.Add(folder);
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Error", "The path already exists.", "Got it!");
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", "The folder must be subfolder of \"Application.dataPath\".", "Got it!");
                    }
                };
            }
            reorderableList.DoLayoutList();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save"))
            {
                if (string.IsNullOrEmpty(preset.savePath))
                    preset.savePath = "Assets";
                var newSavePath = EditorUtility.SaveFilePanel("Save prefab path config", preset.savePath, "PrefabPathConfig", "asset");
                if (!string.IsNullOrEmpty(newSavePath))
                {
                    newSavePath="Assets" + newSavePath.Replace(Application.dataPath, "");
                    preset.savePath = newSavePath;

                    var config = ScriptableObject.CreateInstance<UguiObjectPrefabConfig>();
                    var allAssetPaths = AssetDatabase.GetAllAssetPaths();
                    var typeDict = new Dictionary<string,string>();
                    for(var i=0;i< allAssetPaths.Length;i++)
                    {
                        EditorUtility.DisplayProgressBar("Title","Info",(float)i/ allAssetPaths.Length);

                        var assetPath=allAssetPaths[i];
                        foreach (var selectPath in preset.pathList)
                        {
                            if (assetPath.StartsWith(selectPath))
                            {
                                var obj=AssetDatabase.LoadAssetAtPath<UguiObject>(assetPath);
                                if (obj!=null)
                                {
                                    var finalPath = assetPath.Replace(selectPath,"").TrimStart('/');
                                    finalPath=(Path.GetDirectoryName(finalPath)+"/"+Path.GetFileNameWithoutExtension(finalPath)).TrimStart('/');
                                    var typeName = obj.GetType().FullName;
                                    if (!typeDict.ContainsKey(typeName))
                                    {
                                        typeDict.Add(typeName,assetPath);
                                        var info = new UguiObjectPrefabConfigItem(typeName, finalPath);
                                        config.infos.Add(info);
                                    }
                                    else
                                    {
                                        Debug.LogErrorFormat("The type already exists! Path:{0}  Current:{1}",typeDict[typeName],assetPath);
                                    }
                                }
                            }
                        }
                    }

                    EditorUtility.ClearProgressBar();

                    AssetDatabase.CreateAsset(config, preset.savePath);
                    EditorUtility.SetDirty(config);
                    AssetDatabase.SaveAssets();

                    EditorUtility.DisplayDialog("Successful", "Update object prefab path successful!", "Got it!");
                }
            }
        }

        private void OnDisable()
        {
            UguiEditorTools.SaveConfig(preset);
        }
    }
}