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

        int selectIndex = -1;

        private void OnSceneGUI()
        {
            var line = target as UguiLine;
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            var plane = UguiMathf.GetPlane(line.transform);
            var worldPos = UguiMathf.GetProjectOnPlane(plane, ray.origin);
            var localPos = (Vector2)line.transform.InverseTransformPoint(worldPos);

            var cacheColor = Handles.color;
            var controlColor = new Color(0, 1, 0, 0.6f);
            var selectColor = Color.yellow;
            var snap = Vector2.one;

            if (Event.current.rawType == EventType.KeyDown)
            {
                if (Event.current.keyCode==KeyCode.Escape)
                {
                    selectIndex = -1;
                }
            }

            for (var i = 0; i < line.Points.Count; i++)
            {
                var controlPointPos = line.transform.TransformPoint(line.Points[i]);
                var controlPointSize = HandleUtility.GetHandleSize(controlPointPos) * 0.3f;
                if (selectIndex != i)
                {
                    Handles.color = controlColor;
                    if (Handles.Button(controlPointPos, Quaternion.identity, controlPointSize, controlPointSize, Handles.SphereCap))
                    {
                        selectIndex = i;
                    }
                }
                else
                {
                    Handles.color = selectColor;
                    Handles.SphereCap(0, controlPointPos, Quaternion.identity, controlPointSize * 1.2f);
                    var newPos = Handles.PositionHandle(controlPointPos, Quaternion.identity);
                    if (newPos != (Vector3)line.Points[i])
                        line.Points[i] = line.transform.InverseTransformPoint(newPos);
                }
            }

            Handles.color = cacheColor;

            if (line.IsHit(localPos))
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
                        var scoreList = new List<Vector2>(line.Points);
                        var scoreDict = new Dictionary<Vector2, float>();
                        for (var i=1;i<line.Points.Count;i++)
                        {
                            var prev = line.Points[i - 1];
                            var current = line.Points[i];
                            var segmentLine = new UguiMathf.Line2(prev, current);
                            var dist = segmentLine.Distance(localPos);
                            var dot = Vector2.Dot(localPos-prev,current-localPos);
                            var score = dot / Mathf.Max(dist,0.001f);
                            scoreDict.Add(prev,score);
                        }
                        scoreDict.Add(line.Points[line.Points.Count-1],-1f);
                        scoreList.Sort((a,b)=> 
                        {
                            var scoreA = scoreDict[a];
                            var scoreB = scoreDict[b];

                            if (scoreA == scoreB) return 0;
                            return scoreA > scoreB ? -1 : 1;
                        });
                        var insertIndex=line.Points.IndexOf(scoreList[0]);
                        line.Points.Insert(insertIndex, localPos);
                    }
                }

                line.Points = line.Points;
                EditorUtility.SetDirty(line);
                line.OnRebuildRequested();
                HandleUtility.Repaint();
            }
        }
    }
}