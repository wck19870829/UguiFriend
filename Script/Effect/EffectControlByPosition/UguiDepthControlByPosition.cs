using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 通过位置权重控制层级
    /// </summary>
    public class UguiDepthControlByPosition : UguiEffectControlByPosition<RectTransform, int>
    {
        protected Dictionary<RectTransform, float> weightDict;

        protected UguiDepthControlByPosition()
        {
            weightDict = new Dictionary<RectTransform, float>();
        }

        protected override void MidifyChildren()
        {
            weightDict.Clear();
            foreach (var item in cacheItmeList)
            {
                if (!weightDict.ContainsKey(item))
                {
                    weightDict.Add(item, GetWeight(item));
                }
            }
            cacheItmeList.Sort((a, b) =>
            {
                var weightA = weightDict[a];
                var weightB = weightDict[b];
                if (weightA == weightB) return 0;
                return weightA > weightB ? 1 : -1;
            });
            foreach (var item in cacheItmeList)
            {
                item.SetAsFirstSibling();
            }
        }

        protected override void UpdateItem(RectTransform item, float weight)
        {

        }
    }
}