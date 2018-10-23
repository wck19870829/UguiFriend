using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(Button))]
    /// <summary>
    /// 键盘按键
    /// </summary>
    public class UguiKeypress : UIBehaviour,IPointerDownHandler,IPointerUpHandler
    {
        [SerializeField] protected KeyCode m_KeyCode;
        protected Button button;
        protected bool m_Press;

        public Action<KeyCode> OnKeyDown;
        public Action<KeyCode> OnKeyUp;

        protected override void Awake()
        {
            base.Awake();

            button = GetComponent<Button>();
        }

        public void KeyDown()
        {
            if (m_Press) return;

            m_Press = true;
            if (OnKeyDown != null)
            {
                OnKeyDown.Invoke(m_KeyCode);
            }
        }

        public void KeyUp()
        {
            if (!m_Press) return;

            m_Press = false;
            if (OnKeyUp != null)
            {
                OnKeyUp.Invoke(m_KeyCode);
            }
        }

        /// <summary>
        /// 按下按键
        /// </summary>
        /// <param name="duration">按下状态持续时间</param>
        public virtual void Keystroke(float duration = 1)
        {
            if (m_Press) return;

            CancelInvoke("KeyUp");
            if (duration <= 0)
            {
                KeyDown();
                KeyUp();
            }
            else
            {
                KeyDown();
                Invoke("KeyUp",duration);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            KeyDown();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            KeyUp();
        }

        public virtual KeyCode KeyCode
        {
            get
            {
                return m_KeyCode;
            }
        }

        public virtual bool IsPress
        {
            get
            {
                return m_Press;
            }
        }
    }
}