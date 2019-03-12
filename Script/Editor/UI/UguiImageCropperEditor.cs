using UnityEngine;
using System.Collections;
using UnityEditor;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiImageCropper))]
    public class UguiImageCropperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        }
    }
}