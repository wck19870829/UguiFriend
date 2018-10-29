﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 虚拟键盘配套的文字输入框
    /// </summary>
    public class UguiKeyboardInputField : InputField
    {
        [SerializeField] protected UguiKeyboard m_UguiKeyboard;

        protected override void Awake()
        {
            BindKeyboard();

            base.Awake();
        }

        protected override void OnDestroy()
        {
            UnbindKeyboard();

            base.OnDestroy();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            if (m_UguiKeyboard != null)
            {
                m_UguiKeyboard.gameObject.SetActive(true);
            }
        }

        protected virtual void BindKeyboard()
        {
            if (m_UguiKeyboard != null)
            {
                m_UguiKeyboard.OnKeyDown -= OnKeyboardInputHandler;
                m_UguiKeyboard.OnKey -= OnKeyboardInputHandler;
                m_UguiKeyboard.OnKeyDown += OnKeyboardInputHandler;
                m_UguiKeyboard.OnKey += OnKeyboardInputHandler;
            }
        }

        protected virtual void UnbindKeyboard()
        {
            if (m_UguiKeyboard != null)
            {
                m_UguiKeyboard.OnKeyDown -= OnKeyboardInputHandler;
                m_UguiKeyboard.OnKey -= OnKeyboardInputHandler;
            }
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            var pe = eventData as PointerEventData;
            if (pe!=null)
            {
                if (pe.pointerPressRaycast.gameObject)
                {
                    var keyboard = pe.pointerPressRaycast.gameObject.GetComponentInParent<UguiKeyboard>();
                    if (keyboard != null)
                    {
                        //拦截点击键盘丢失焦点
                        return;
                    }
                }
            }

            base.OnDeselect(eventData);
        }

        protected virtual void OnKeyboardInputHandler(Event processingEvent)
        {
            ProcessEvent(processingEvent);
            UpdateLabel();
        }

        /// <summary>
        /// 键盘
        /// </summary>
        public UguiKeyboard UguiKeyboard
        {
            get
            {
                return m_UguiKeyboard;
            }
            set
            {
                UnbindKeyboard();
                m_UguiKeyboard = value;
                BindKeyboard();
            }
        }
    }
}