using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 线段
    /// </summary>
    public class UguiLine : Graphic
    {
        protected virtual void DrawLine()
        {
            
        }
    }

    /// <summary>
    /// 线段分段
    /// </summary>
    public class UguiLineSegment : ScriptableObject
    {
        public Vector3 start;
        public Vector3 end;
        public Color color;
    }
}