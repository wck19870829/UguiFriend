using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(Button))]
    /// <summary>
    /// 键盘按键
    /// </summary>
    public class UguiKeypress : UIBehaviour,IPointerDownHandler,IPointerUpHandler
    {
        static readonly Dictionary<KeyCode, string> specialDisplayNameDict;

        [SerializeField] protected KeyCode m_KeyCode;
        [SerializeField] protected Text nameText;
        protected Button button;
        protected bool m_Press;

        public Action<KeyCode> OnKeyDown;
        public Action<KeyCode> OnKeyUp;

        static UguiKeypress()
        {
            specialDisplayNameDict = new Dictionary<KeyCode, string>()
            {
                { KeyCode.Backspace,"←"}
            };
        }

        protected override void Awake()
        {
            base.Awake();

            button = GetComponent<Button>();
            if (m_KeyCode == KeyCode.None)
            {
                try
                {
                    m_KeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), name);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Key code is error:{0}",e);
                }
            }

            if (nameText == null)
                nameText = GetComponentInChildren<Text>();
            if (nameText == null)
                throw new Exception("Text is null.");
            if (specialDisplayNameDict.ContainsKey(m_KeyCode))
            {
                nameText.text = specialDisplayNameDict[m_KeyCode];
            }
            else
            {
                nameText.text = m_KeyCode.ToString();
            }
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