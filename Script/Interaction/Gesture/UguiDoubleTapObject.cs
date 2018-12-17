using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 双击物体
    /// </summary>
    public class UguiDoubleTapObject : UIBehaviour, IPointerClickHandler
    {
        public Action<PointerEventData> OnDoubleTapEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                Debug.Log("Double Tap");
            }
        }
    }
}