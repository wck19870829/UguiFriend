using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 预设路径窗口
    /// </summary>
    public class UguiObjectPrefabPathWindow : EditorWindow
    {
        static string savePath;
        static HashSet<string> pathSet=new HashSet<string>();

        [MenuItem("Tools/UguiFriend/Object Prefab Path Window")]
        static void Init()
        {
            var window = GetWindow<UguiObjectPrefabPathWindow>();
            window.titleContent = new GUIContent("Prefab path");
            window.Show();
        }

        private void OnGUI()
        {
            using (var verticalScope = new EditorGUILayout.VerticalScope())
            {
                foreach (var path in pathSet)
                {
                    using (var horizontalScope = new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.TextField(path);
                        GUILayout.Button("-", GUILayout.Width(EditorGUIUtility.singleLineHeight));
                    }
                }
            }


            EditorGUILayout.Space();

            if (GUILayout.Button("Save"))
            {
                var newSavePath = EditorUtility.SaveFilePanel("Save prefab path config", savePath, "PrefabPathConfig", "asset");
                if (!string.IsNullOrEmpty(newSavePath))
                {
                    savePath = newSavePath;


                }
            }
        }
    }
}