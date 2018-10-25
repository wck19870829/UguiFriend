using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    public class UguiKeyboardInputField : InputField
    {
        public override void OnDeselect(BaseEventData eventData)
        {
            Debug.Log("Deselect");
            if (eventData != null)
            {
                var keyboard = eventData.selectedObject.GetComponentInParent<UguiKeyboard>();
                if (keyboard != null)
                {
                    return;
                }
            }

            base.OnDeselect(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            //base.OnPointerUp(eventData);
        }
    }
}