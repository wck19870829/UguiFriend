using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 键盘按键
    /// </summary>
    public class UguiKeypress : Selectable
    {
        protected static readonly HashSet<KeyCode> keypadSet;                       //小键盘上的键
        protected static readonly HashSet<KeyCode> keepPressSet;                    //可以挂起的按键
        protected static readonly Dictionary<KeyCode, string> nameDict;

        [SerializeField] protected KeyCode m_KeyCode;
        [SerializeField] protected KeyCode m_ShiftKeyCode;
        protected Text nameText;
        protected KeypressState m_State;
        protected UguiKeyboard keyboard;
        protected int keyDownCount;
        protected bool init;

        public Action<KeyCode> OnRealKeyDown;                                       //按键按下
        public Action<KeyCode> OnRealKeyUp;                                         //按键弹起
        public Action<KeyCode> OnEnter;

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
                KeyCode.ScrollLock,
                KeyCode.LeftAlt,
                KeyCode.RightAlt,

                //虚拟键盘特殊处理
                KeyCode.CapsLock,
                KeyCode.ScrollLock,
                KeyCode.Numlock
                
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
                { KeyCode.LeftControl,"Ctrl"},
                {KeyCode.RightControl,"Ctrl" },

                //小键盘
                { KeyCode.KeypadPeriod,"."},
                { KeyCode.KeypadDivide,"/"},
            };
            keypadSet = new HashSet<KeyCode>();
            var keyCodeValues = Enum.GetValues(typeof(KeyCode));
            foreach (var value in keyCodeValues)
            {
                if (value.ToString().StartsWith("Keypad"))
                {
                    keypadSet.Add((KeyCode)value);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Init();
            ResetKeypress();
        }

        protected virtual void Init()
        {
            if (!init)
            {
                try
                {
                    if (m_KeyCode == KeyCode.None)
                    {
                        var keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), name);
                        m_KeyCode = keyCode;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                if (m_KeyCode == KeyCode.None)
                    throw new Exception("Key code is None.");

                keyboard = GetComponentInParent<UguiKeyboard>();
                if (keyboard == null)
                    throw new Exception("Keyboard is null.");

                init = true;
            }
        }

        /// <summary>
        /// 重置按键
        /// </summary>
        public virtual void ResetKeypress()
        {
            Init();

            keyDownCount = 0;
            m_State = KeypressState.Normal;
            PlayKeyUpAnim(true);
            UpdateView();
        }

        protected virtual KeyCode GetCurrentKeyCode()
        {
            if (keyboard.IsShiftPress&&m_ShiftKeyCode!=KeyCode.None)
            {
                return m_ShiftKeyCode;
            }

            return m_KeyCode;
        }

        /// <summary>
        /// 更新显示
        /// </summary>
        internal virtual void UpdateView()
        {
            if (nameText == null)
                nameText = GetComponentInChildren<Text>();
            if (nameText != null)
            {
                var shiftNameStr = "";
                if (m_ShiftKeyCode != KeyCode.None)
                {
                    if (nameDict.ContainsKey(m_ShiftKeyCode))
                    {
                        shiftNameStr = nameDict[m_ShiftKeyCode] + "\n";
                    }
                    else
                    {
                        shiftNameStr = m_ShiftKeyCode + "\n";
                    }
                }
                var nameStr = "";
                if (nameDict.ContainsKey(m_KeyCode))
                {
                    nameStr = nameDict[m_KeyCode];
                }
                else
                {
                    nameStr = m_KeyCode.ToString();
                }
                if (IsLetter(m_KeyCode))
                {
                    nameStr = keyboard.IsUpper ? nameStr.ToUpper() : nameStr.ToLower();
                }
                nameText.text = shiftNameStr+nameStr;
            }
        }

        protected bool IsLetter(KeyCode keyCode)
        {
            var codeStr=keyCode.ToString();
            if (codeStr.Length > 1||codeStr.Length==0) return false;
            return Char.IsLetter(char.Parse(codeStr));
        }

        protected void KeyDown()
        {
            if (keepPressSet.Contains(m_KeyCode))
            {
                keyDownCount++;
                if (keyDownCount == 1)
                {
                    m_State = KeypressState.Press;
                    PlayKeyDownAnim(false);
                    if (OnRealKeyDown != null)
                    {
                        OnRealKeyDown.Invoke(GetCurrentKeyCode());
                    }
                }
            }
            else
            {
                if (m_State==KeypressState.Normal)
                {
                    m_State = KeypressState.Press;
                    PlayKeyDownAnim(false);
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
                    m_State = KeypressState.Normal;
                    PlayKeyUpAnim(false);
                    if (OnRealKeyUp != null)
                    {
                        OnRealKeyUp.Invoke(GetCurrentKeyCode());
                    }
                }
            }
            else
            {
                if (m_State == KeypressState.Press)
                {
                    m_State = KeypressState.Normal;
                    PlayKeyUpAnim(false);
                    if (OnRealKeyUp != null)
                    {
                        OnRealKeyUp.Invoke(GetCurrentKeyCode());
                    }
                }
            }
        }

        protected virtual void PlayKeyDownAnim(bool instant)
        {
            DoStateTransition(SelectionState.Pressed, instant);
        }

        protected virtual void PlayKeyUpAnim(bool instant)
        {
            DoStateTransition(SelectionState.Normal, instant);
        }

        /// <summary>
        /// 按下按键
        /// </summary>
        /// <param name="duration">按下状态持续时间</param>
        public virtual void Keystroke(float duration = 1)
        {
            if (m_State == KeypressState.Press) return;

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

        public override void OnPointerDown(PointerEventData eventData)
        {
            KeyDown();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            KeyUp();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (m_State==KeypressState.Press)
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
            if (m_State == KeypressState.Normal)
            {
                DoStateTransition(SelectionState.Highlighted, false);
            }
            if (OnEnter != null)
            {
                OnEnter.Invoke(GetCurrentKeyCode());
            }
        }

        /// <summary>
        /// 下档键（必须）
        /// </summary>
        public KeyCode KeyCode
        {
            get
            {
                return m_KeyCode;
            }
        }

        /// <summary>
        /// 上档键
        /// </summary>
        public KeyCode ShiftKeyCode
        {
            get
            {
                return m_ShiftKeyCode;
            }
        }

        /// <summary>
        /// 按键状态
        /// </summary>
        public KeypressState State
        {
            get
            {
                return m_State;
            }
        }

        /// <summary>
        /// 按键状态
        /// </summary>
        public enum KeypressState
        {
            Normal,
            Press
        }
    }
}