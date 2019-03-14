using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(Canvas))]
    public sealed class UguiMultPointSimulationRaycaster : BaseRaycaster
    {
        Canvas canvas;

        protected override void Awake()
        {
            canvas = gameObject.GetComponent<Canvas>();
            if(!canvas)
                canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue;
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {

        }

        public override Camera eventCamera
        {
            get
            {
                return null;
            }
        }

        public override int sortOrderPriority
        {
            get
            {
                return canvas.sortingOrder;
            }
        }

        public override int renderOrderPriority
        {
            get
            {
                return canvas.renderOrder;
            }
        }
    }
}