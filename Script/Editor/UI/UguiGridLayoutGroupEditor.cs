using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiGridLayoutGroup))]
    public class UguiGridLayoutGroupEditor : UguiLayoutGroupEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}