using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 通过位置权重控制位置
    /// </summary>
    public class UguiPositionControlByPosition : UguiEffectControlByPosition<Transform, Vector3>
    {
        protected Dictionary<Transform, Vector3> originDict;            //保存原点

        protected UguiPositionControlByPosition()
        {
            originDict = new Dictionary<Transform, Vector3>();
        }

        /// <summary>
        /// 缓存原点
        /// </summary>
        /// <param name="item"></param>
        /// <param name="origin"></param>
        public virtual void CacheOrigin(Transform item,Vector3 origin)
        {
            if (!originDict.ContainsKey(item))
            {
                originDict.Add(item,origin);
            }
            else
            {
                originDict[item] = origin;
            }
        }

        protected override void UpdateItem(Transform item, float weight)
        {
            if (!originDict.ContainsKey(item))
                throw new Exception("You must be cache item origin.");

            var offset= Vector3.Lerp(valueFrom, valueTo, weight);
            if (isCenterMirror)
            {
                var normal = Vector3.up;
                offset = Vector3.Reflect(offset, normal);
            }
            item.localPosition = originDict[item] + offset;
        }
    }
}