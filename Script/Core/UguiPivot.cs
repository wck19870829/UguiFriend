using UnityEngine;
using System.Collections;
using System;

namespace RedScarf.UguiFriend
{
    [Flags]
    /// <summary>
    /// 轴心点
    /// </summary>
    public enum UguiPivot
    {
        TopLeft         =1,
        Top             =2,
        TopRight        =4,
        Left            =8,
        Center          =16,
        Right           =32,
        BottomLeft      =64,
        Bottom          =128,
        BottomRight     =256
    }
}