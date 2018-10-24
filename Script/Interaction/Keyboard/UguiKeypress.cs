using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Animation))]
    /// <summary>
    /// 键盘按键
    /// </summary>
    public class UguiKeypress : UIBehaviour,IPointerDownHandler,IPointerUpHandler
    {
        protected const string KeyDownAnim = "KeyDown";
        protected const string KeyUpAnim = "KeyUp";
        protected static readonly HashSet<KeyCode> keepPressSet;                    //可以挂起的按键
        protected static readonly Dictionary<KeyCode, string> nameDict;

        [SerializeField] protected KeyCode m_KeyCode;   
        [SerializeField] protected KeyCode m_ShiftKeyCode;
        protected Text nameText;
        protected bool m_Press;
        protected UguiKeyboard keyboard;
        protected int clickCount;
        protected Animation m_Anim;

        public Action<KeyCode> OnKeyDown;
        public Action<KeyCode> OnKeyUp;

        static UguiKeypress()
        {
            keepPressSet = new HashSet<KeyCode>()
            {
                KeyCode.LeftShift,
                KeyCode.RightShift,
                KeyCode.LeftCommand,
                KeyCode.RightCommand,
                KeyCode.LeftControl,
                KeyCode.RightControl,
                KeyCode.CapsLock,
                KeyCode.LeftWindows,
                KeyCode.RightWindows,
                KeyCode.ScrollLock
            };
            nameDict = new Dictionary<KeyCode, string>()
            {
                {KeyCode.Space,""},
                {KeyCode.Backspace,"←" },
                {KeyCode.CapsLock,"Caps Lock" },
                {KeyCode.LeftShift,"Shift" },
                {KeyCode.RightShift,"Shift" },
                {KeyCode.LeftAlt,"Alt" },
                {KeyCode.RightAlt,"Alt" },
                {KeyCode.At,"@" },
                {KeyCode.Exclaim,"!"},
                {KeyCode.Hash,"#" },
                {KeyCode.Dollar,"$" },
                { KeyCode.Caret,"^"},
                { KeyCode.Ampersand,"&"},
                { KeyCode.Asterisk,"*"},
                { KeyCode.LeftParen,"("},
                { KeyCode.RightParen,")"},
                { KeyCode.Underscore,"_"},
                { KeyCode.KeypadPlus,"+"},
                { KeyCode.Minus,"-"},
                {KeyCode.BackQuote,"`" },
                { KeyCode.Backslash,"\\"},
                { KeyCode.LeftBracket,"["},
                { KeyCode.RightBracket,"]"},
                { KeyCode.Colon,":"},
                { KeyCode.Semicolon,";"},
                {KeyCode.DoubleQuote,"\"" },
                { KeyCode.Escape,"Esc"},

                //小键盘
                { KeyCode.KeypadPeriod,"."},
                { KeyCode.KeypadDivide,"/"},

            };
        }

        protected override void Awake()
        {
            base.Awake();

            if (m_KeyCode == KeyCode.None)
                throw new Exception("Key code is None.");

            if (m_Anim == null)
                m_Anim = GetComponent<Animation>();
            if (m_Anim == null)
                m_Anim = gameObject.AddComponent<Animation>();

            keyboard = GetComponentInParent<UguiKeyboard>();
            if (keyboard == null)
                throw new Exception("Keyboard is null.");

            if (nameText == null)
                nameText = GetComponentInChildren<Text>();
            if (nameText != null)
            {
                var nameStr = "";
                if (m_ShiftKeyCode != KeyCode.None)
                {
                    if (nameDict.ContainsKey(m_ShiftKeyCode))
                    {
                        nameStr += nameDict[m_ShiftKeyCode] + "\n";
                    }
                    else
                    {
                        nameStr += m_ShiftKeyCode + "\n";
                    }
                }
                if (nameDict.ContainsKey(m_KeyCode))
                {
                    nameStr += nameDict[m_KeyCode];
                }
                else
                {
                    nameStr += m_KeyCode.ToString();
                }
                nameText.text = nameStr;
            }
        }

        public void KeyDown()
        {
            if (keepPressSet.Contains(m_KeyCode))
            {
                clickCount++;
                if (clickCount == 1)
                {
                    m_Press = true;
                    PlayKeyDownAnim();
                    if (OnKeyDown != null)
                    {
                        OnKeyDown.Invoke(m_KeyCode);
                    }
                }
            }
            else
            {
                if (!m_Press)
                {
                    m_Press = true;
                    PlayKeyDownAnim();
                    if (OnKeyDown != null)
                    {
                        OnKeyDown.Invoke(m_KeyCode);
                    }
                }
            }
        }

        public void KeyUp()
        {
            if (keepPressSet.Contains(m_KeyCode))
            {
                if (clickCount==2)
                {
                    clickCount = 0;
                    m_Press = false;
                    PlayKeyUpAnim();
                    if (OnKeyUp != null)
                    {
                        OnKeyUp.Invoke(m_KeyCode);
                    }
                }
            }
            else
            {
                if (m_Press)
                {
                    m_Press = false;
                    PlayKeyUpAnim();
                    if (OnKeyUp != null)
                    {
                        OnKeyUp.Invoke(m_KeyCode);
                    }
                }
            }
        }

        protected virtual void PlayKeyDownAnim()
        {
            m_Anim.Play(KeyDownAnim);
        }

        protected virtual void PlayKeyUpAnim()
        {
            m_Anim.Play(KeyUpAnim);
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

        public KeyCode KeyCode
        {
            get
            {
                return m_KeyCode;
            }
        }

        public KeyCode ShiftKeyCode
        {
            get
            {
                return m_ShiftKeyCode;
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