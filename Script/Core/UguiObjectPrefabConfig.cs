using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 对象预设路径信息
    /// </summary>
    public class UguiObjectPrefabConfig : ScriptableObject
    {
        public List<UguiObjectPrefabConfigItem> infos;
    }

    [System.Serializable]
    public struct UguiObjectPrefabConfigItem
    {
        public string objType;
        public string prefabPath;

        public UguiObjectPrefabConfigItem(string objType,string prefabPath)
        {
            this.objType = objType;
            this.prefabPath = prefabPath;
        }
    }
}