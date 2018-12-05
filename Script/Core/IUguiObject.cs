using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    public interface IUguiObject
    {
        /// <summary>
        /// 数据
        /// </summary>
        object Data { get; set; }
    }
}