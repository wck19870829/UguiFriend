using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(Button))]
    /// <summary>
    /// 点击居中
    /// </summary>
    public class UguiCenterOnClick : MonoBehaviour
    {
        protected Button button;
        protected ScrollRect scrollRect;

        protected virtual void Start()
        {
            button = GetComponent<Button>();
            if (button == null)
                throw new Exception("Button is null.");
            button.onClick.AddListener(OnButtonClick);
        }

        void OnButtonClick()
        {
            if (scrollRect == null)
                scrollRect = GetComponentInParent<ScrollRect>();
            if (scrollRect!=null)
            {
                var centerOnChild = scrollRect.GetComponent<UguiCenterOnChild>();
                if (centerOnChild == null)
                    centerOnChild = scrollRect.gameObject.AddComponent<UguiCenterOnChild>();
                centerOnChild.CenterOn(transform);
            }
        }
    }
}