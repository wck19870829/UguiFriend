using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// UI工具类
    /// </summary>
    public class UITools
    {
        /// <summary>
        /// 销毁子元素
        /// </summary>
        /// <param name="go"></param>
        public static void DestroyChildren(GameObject go)
        {
            for (var i=go.transform.childCount-1;i>=0; i--)
            {
                GameObject.Destroy(go.transform.GetChild(i).gameObject);
            }
        }
    }
}