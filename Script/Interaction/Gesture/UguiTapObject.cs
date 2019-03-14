using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 点击物体
    /// </summary>
    public class UguiTapObject : UIBehaviour, 
        IPointerClickHandler
    {
        public Action<PointerEventData> OnTapEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 1)
            {
                Debug.Log("Tap");
            }
        }
    }
}