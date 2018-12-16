using UnityEngine;
using System.Collections;
using UnityEditor.UI;
using UnityEditor;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiFreeTransformImage), true)]
    public class UguiFreeTransformImageEditor : RawImageEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}