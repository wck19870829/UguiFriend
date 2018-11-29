using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [Serializable]
    /// <summary>
    /// 序列化保存
    /// </summary>
    public class UguiLayerInfoConfig : ScriptableObject
    {
        public List<string> nameList;
    }
}