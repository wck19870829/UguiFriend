using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [System.Serializable]
    /// <summary>
    /// 轴心点集合
    /// </summary>
    public class UguiPivotSet:List<UguiPivot>
    {
        public void TryAdd(UguiPivot pivot)
        {
            if (IndexOf(pivot) >= 0)
                return;

            Add(pivot);
        }

        public bool TryRemove(UguiPivot pivot)
        {
            return Remove(pivot);
        }
    }
}