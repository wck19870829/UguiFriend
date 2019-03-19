using UnityEngine;
using System.Collections;
using UnityEditor;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiTweenMasterDrive), true)]
    public class UguiTweenMasterDriveEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var str = "Don not destroy it directly.\r\n"
                    +"If you want to destroy this,please remove it form UguiTweenMaster inspector.";

            EditorGUILayout.HelpBox(str, MessageType.Warning);
        }
    }
}