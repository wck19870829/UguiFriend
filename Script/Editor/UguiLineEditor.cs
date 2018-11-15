using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [CustomEditor(typeof(UguiLine),true)]
    public class UguiLineEditor : RawImageEditor
    {
        public override void OnInspectorGUI()
        {
            var points = serializedObject.FindProperty("m_Points");
            var lineStyle = serializedObject.FindProperty("m_LineStyle");
            var thickness = serializedObject.FindProperty("m_Thickness");
            var simpleDistance = serializedObject.FindProperty("m_SimpleDistance");
            var gradient = serializedObject.FindProperty("m_Gradient");
            var thicknessCurve = serializedObject.FindProperty("m_ThicknessCurve");

            EditorGUILayout.PropertyField(lineStyle);
            EditorGUILayout.Slider(thickness, 0.1f, 1000f);
            EditorGUILayout.PropertyField(thicknessCurve);
            EditorGUILayout.PropertyField(gradient);

            var style = (UguiLine.LineStyle)lineStyle.intValue;
            EditorGUILayout.IntSlider(simpleDistance, UguiMathf.Bezier.minSimpleDistance, UguiMathf.Bezier.maxSimpleDistance);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(points, true);

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            var line = target as UguiLine;
            if (IsMouseHit(line))
            {
                Handles.BeginGUI();
                var addIconSize = new Vector2(100, 100);
                var iconPos = Event.current.mousePosition;
                iconPos.y -= addIconSize.y;
                GUI.Box(new Rect(iconPos, addIconSize), "Add point?");
                Handles.EndGUI();

                if (Event.current.type == EventType.MouseDown)
                {
                    if (Event.current.clickCount == 2)
                    {
                        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        var plane = UguiMathf.GetPlane(line.transform);
                        var worldPos = UguiMathf.GetProjectOnPlane(plane, ray.origin);
                        var localPos = line.transform.InverseTransformPoint(worldPos);


                    }
                }

                HandleUtility.Repaint();
            }
        }

        static bool IsMouseHit(UguiLine line)
        {
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            var plane = UguiMathf.GetPlane(line.transform);
            var worldPos = UguiMathf.GetProjectOnPlane(plane, ray.origin);
            var localPos = line.transform.InverseTransformPoint(worldPos);

            return line.IsHit(localPos);
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy|GizmoType.Active|GizmoType.NonSelected,typeof(UguiLine))]
        static void DrawControlPoints(UguiLine line,GizmoType gizmoType)
        {
            var pointsClone = new List<Vector2>(line.Points);
            var cacheColor = Handles.color;
            var controlColor = new Color(0,1,0,0.6f);
            Handles.color = controlColor;
            var snap = Vector2.one;
            for(var i=0;i< pointsClone.Count;i++)
            {
                var controlPointPos = line.transform.TransformPoint(pointsClone[i]);
                var controlPointSize = HandleUtility.GetHandleSize(controlPointPos) * 0.3f;
                if (Handles.Button(controlPointPos, Quaternion.identity, controlPointSize, controlPointSize*1.2f, Handles.SphereCap))
                {
                    Debug.Log("Select");
                }
                else
                {
                    var newPos = Handles.FreeMoveHandle(controlPointPos, Quaternion.identity, controlPointSize, snap, Handles.SphereCap);
                    pointsClone[i] = line.transform.InverseTransformPoint(newPos);
                }
            }
            line.Points = pointsClone;

            Handles.color = cacheColor;
        }
    }
}