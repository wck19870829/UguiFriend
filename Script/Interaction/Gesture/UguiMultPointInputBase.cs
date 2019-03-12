using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 多点触控输入基类
    /// </summary>
    public abstract class UguiMultPointInputBase : UIBehaviour,
        IPointerDownHandler,
        IPointerUpHandler
    {
        protected Dictionary<int, PointerEventData> pointerDataDict;

        protected UguiMultPointInputBase()
        {
            pointerDataDict = new Dictionary<int, PointerEventData>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!pointerDataDict.ContainsKey(eventData.pointerId))
                pointerDataDict.Add(eventData.pointerId,eventData);

            foreach (var pointer in pointerDataDict)
            {

            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pointerDataDict.Remove(eventData.pointerId);
        }
    }
}