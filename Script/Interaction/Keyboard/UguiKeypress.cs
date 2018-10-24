using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [ExecuteInEditMode]
    /// <summary>
    /// 键盘按键
    /// </summary>
    public class UguiKeypress : Selectable
    {
        protected static readonly HashSet<KeyCode> keepPressSet;                    //可以挂起的按键
        protected static readonly Dictionary<KeyCode, string> nameDict;

        [SerializeField] protected KeyCode m_KeyCode;
        [SerializeField] protected KeyCode m_ShiftKeyCode;
        protected Text nameText;
        protected bool m_Press;
        protected UguiKeyboard keyboard;
        protected int keyDownCount;
        protected bool isCapsLockOpen;
        protected bool isShiftPress;
        protected bool isUpper;

        public Action<KeyCode> OnRealKeyDown;                                       //按键按下
        public Action<KeyCode> OnRealKeyUp;                                         //按键弹起

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
                { KeyCode.Space,""},
                { KeyCode.Backspace,"←Backspace" },
                { KeyCode.CapsLock,"Caps Lock" },
                { KeyCode.LeftShift,"Shift" },
                { KeyCode.RightShift,"Shift" },
                { KeyCode.LeftAlt,"Alt" },
                { KeyCode.RightAlt,"Alt" },
                { KeyCode.At,"@" },
                { KeyCode.Exclaim,"!"},
                { KeyCode.Hash,"#" },
                { KeyCode.Dollar,"$" },
                { KeyCode.Caret,"^"},
                { KeyCode.Ampersand,"&"},
                { KeyCode.Asterisk,"*"},
                { KeyCode.LeftParen,"("},
                { KeyCode.RightParen,")"},
                { KeyCode.Underscore,"_"},
                { KeyCode.KeypadPlus,"+"},
                { KeyCode.Minus,"-"},
                { KeyCode.BackQuote,"`" },
                { KeyCode.Backslash,"\\"},
                { KeyCode.LeftBracket,"["},
                { KeyCode.RightBracket,"]"},
                { KeyCode.Colon,":"},
                { KeyCode.Semicolon,";"},
                { KeyCode.DoubleQuote,"\"" },
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

            keyboard = GetComponentInParent<UguiKeyboard>();
            if (keyboard == null)
                throw new Exception("Keyboard is null.");

            UpdateDisplayName();
        }

        public virtual void Init()
        {

        }

        protected virtual KeyCode GetCurrentKeyCode()
        {
            if (isShiftPress&&m_ShiftKeyCode!=KeyCode.None)
            {
                return m_ShiftKeyCode;
            }

            return m_KeyCode;
        }

        /// <summary>
        /// 更新显示
        /// </summary>
        protected virtual void UpdateDisplayName()
        {
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

        protected void KeyDown()
        {
            if (keepPressSet.Contains(m_KeyCode))
            {
                keyDownCount++;
                if (keyDownCount == 1)
                {
                    m_Press = true;
                    PlayKeyDownAnim();
                    if (OnRealKeyDown != null)
                    {
                        OnRealKeyDown.Invoke(GetCurrentKeyCode());
                    }
                }
            }
            else
            {
                if (!m_Press)
                {
                    m_Press = true;
                    PlayKeyDownAnim();
                    if (OnRealKeyDown != null)
                    {
                        OnRealKeyDown.Invoke(GetCurrentKeyCode());
                    }
                }
            }
        }

        protected void KeyUp()
        {
            if (keepPressSet.Contains(m_KeyCode))
            {
                if (keyDownCount == 2)
                {
                    keyDownCount = 0;
                    m_Press = false;
                    PlayKeyUpAnim();
                    if (OnRealKeyUp != null)
                    {
                        OnRealKeyUp.Invoke(GetCurrentKeyCode());
                    }
                }
            }
            else
            {
                if (m_Press)
                {
                    m_Press = false;
                    PlayKeyUpAnim();
                    if (OnRealKeyUp != null)
                    {
                        OnRealKeyUp.Invoke(GetCurrentKeyCode());
                    }
                }
            }
        }

        protected virtual void PlayKeyDownAnim()
        {
            DoStateTransition(SelectionState.Pressed, false);
        }

        protected virtual void PlayKeyUpAnim()
        {
            DoStateTransition(SelectionState.Normal, false);
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
                Invoke("KeyUp", duration);
            }
        }

        internal virtual void SetState(bool shiftPress,bool upper,bool capsLockOpen)
        {
            isShiftPress = shiftPress;
            isUpper = upper;
            isCapsLockOpen = capsLockOpen;

            UpdateDisplayName();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (keyboard != null)
            {
                keyboard.ForcusOnInputField();
            }

            KeyDown();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            KeyUp();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (m_Press)
            {
                DoStateTransition(SelectionState.Pressed, false);
            }
            else
            {
                DoStateTransition(SelectionState.Normal, false);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!m_Press)
            {
                DoStateTransition(SelectionState.Highlighted, false);
            }
        }

        /// <summary>
        /// 未按下Shift键对应的下档键
        /// </summary>
        public KeyCode KeyCode
        {
            get
            {
                return m_KeyCode;
            }
        }

        /// <summary>
        /// 按下Shift键对应的上档键
        /// </summary>
        public KeyCode ShiftKeyCode
        {
            get
            {
                return m_ShiftKeyCode;
            }
        }

        /// <summary>
        /// 是否按下
        /// </summary>
        public virtual bool IsPress
        {
            get
            {
                return m_Press;
            }
        }
    }
}