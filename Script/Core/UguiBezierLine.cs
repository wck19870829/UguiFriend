using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 贝塞尔曲线
    /// </summary>
    public class UguiBezierLine : UIBehaviour
    {

    }

    /// <summary>
    /// 线段分段
    /// </summary>
    public class UguiBezierLineSegment : ScriptableObject
    {
        public Vector3 start;
        public Vector3 end;
        public Color color;
    }
}