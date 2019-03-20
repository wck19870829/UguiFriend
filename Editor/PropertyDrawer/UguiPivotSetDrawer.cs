using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [CustomPropertyDrawer(typeof(UguiPivotSet), true)]
    public class UguiPivotSetDrawer : PropertyDrawer
    {
        const int pivotTexSize = 60;
        static int offset = (int)(pivotTexSize / 3f);

        HashSet<UguiPivot> activeSet;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (activeSet == null)
                activeSet = new HashSet<UguiPivot>();

            EditorGUI.BeginProperty(position, label, property);
            GUI.BeginGroup(position);

            activeSet.Clear();
            var valuesProp=property.FindPropertyRelative("values");
            for (var i = 0; i < valuesProp.arraySize; i++)
            {
                var sp = valuesProp.GetArrayElementAtIndex(i);
                var pivot = (UguiPivot)sp.intValue;
                if (!activeSet.Contains(pivot))
                    activeSet.Add(pivot);
            }
            DrawPivot(UguiPivot.TopLeft, activeSet.Contains(UguiPivot.TopLeft), new Rect(offset*0, offset * 0, offset, offset));
            DrawPivot(UguiPivot.Top, activeSet.Contains(UguiPivot.Top), new Rect(offset*1, offset*0, offset, offset));
            DrawPivot(UguiPivot.TopRight, activeSet.Contains(UguiPivot.TopRight), new Rect(offset*2, offset*0, offset, offset));
            DrawPivot(UguiPivot.Left, activeSet.Contains(UguiPivot.Left), new Rect(offset * 0, offset*1, offset, offset));
            DrawPivot(UguiPivot.Center, activeSet.Contains(UguiPivot.Center), new Rect(offset * 1, offset * 1, offset, offset));
            DrawPivot(UguiPivot.Right, activeSet.Contains(UguiPivot.Right), new Rect(offset * 2, offset * 1, offset, offset));
            DrawPivot(UguiPivot.BottomLeft, activeSet.Contains(UguiPivot.BottomLeft), new Rect(offset * 0, offset * 2, offset, offset));
            DrawPivot(UguiPivot.Bottom, activeSet.Contains(UguiPivot.Bottom), new Rect(offset * 1, offset * 2, offset, offset));
            DrawPivot(UguiPivot.BottomRight, activeSet.Contains(UguiPivot.BottomRight), new Rect(offset * 2, offset * 2, offset, offset));

            valuesProp.ClearArray();
            foreach (var value in activeSet)
            {
                valuesProp.InsertArrayElementAtIndex(0);
                var sp=valuesProp.GetArrayElementAtIndex(0);
                sp.intValue = (int)value;
            }

            GUI.EndGroup();
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return pivotTexSize;
        }

        void DrawPivot(UguiPivot pivot, bool active, Rect rect)
        {
            var icon = active
                    ? UguiEditorTools.LoadTex("Pivot_" + pivot + "_Light")
                    : UguiEditorTools.LoadTex("Pivot_" + pivot);
            var style = new GUIStyle(GUIStyle.none);
            var hover = new GUIStyleState();
            style.hover = hover;
            if (GUI.Button(rect, icon, style))
            {
                if (active)
                {
                    activeSet.Remove(pivot);
                }
                else
                {
                    if (!activeSet.Contains(pivot))
                    {
                        activeSet.Add(pivot);
                    }
                }
            }
        }
    }
}