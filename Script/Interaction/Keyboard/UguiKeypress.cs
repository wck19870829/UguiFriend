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
        public const char emptyCharacter = char.MinValue;                           //空字符
        protected static readonly HashSet<KeyCode> keypadSet;                       //小键盘上的键
        protected static readonly HashSet<KeyCode> keepPressSet;                    //可以挂起的按键
        protected static readonly Dictionary<KeyCode, string> keyCodeNameDict;
        protected static readonly Dictionary<KeyCode, char> shiftCharDict;
        protected static readonly Dictionary<KeyCode, char> normalCharDict;

        [SerializeField] protected KeyCode m_KeyCode;
        protected Text nameText;
        protected char m_Character;
        protected KeypressState m_State;
        protected UguiKeyboard keyboard;
        protected int keyDownCount;
        protected bool m_Init;

        public Action<UguiKeypress> OnRealKeyDown;                                       //按键按下
        public Action<UguiKeypress> OnRealKeyUp;                                         //按键弹起
        public Action<UguiKeypress> OnEnter;

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
            normalCharDict = new Dictionary<KeyCode, char>()
            {
                { KeyCode.Alpha0,'0'},
                { KeyCode.Alpha1,'1'},
                { KeyCode.Alpha2,'2'},
                { KeyCode.Alpha3,'3'},
                { KeyCode.Alpha4,'4'},
                { KeyCode.Alpha5,'5'},
                { KeyCode.Alpha6,'6'},
                { KeyCode.Alpha7,'7'},
                { KeyCode.Alpha8,'8'},
                { KeyCode.Alpha9,'9'},
                { KeyCode.BackQuote,'`'},
                { KeyCode.Minus,'-'},
                { KeyCode.Equals,'=' },
                { KeyCode.LeftBracket,'['},
                { KeyCode.RightBracket,']'},
                { KeyCode.Backslash,'\\'},
                { KeyCode.Semicolon,';'},
                { KeyCode.Quote,'\'' },
                { KeyCode.Comma,',' },
                { KeyCode.Period,'.'},
                { KeyCode.Slash,'/' }
            };
            shiftCharDict = new Dictionary<KeyCode, char>()
            {
                { KeyCode.BackQuote,'~'},
                { KeyCode.Minus,'_'},
                { KeyCode.Equals,'+' },
                { KeyCode.Alpha0,')'},
                { KeyCode.Alpha1,'!'},
                { KeyCode.Alpha2,'@'},
                { KeyCode.Alpha3,'#'},
                { KeyCode.Alpha4,'$'},
                { KeyCode.Alpha5,'%'},
                { KeyCode.Alpha6,'^'},
                { KeyCode.Alpha7,'&'},
                { KeyCode.Alpha8,'*'},
                { KeyCode.Alpha9,'('},
                { KeyCode.LeftBracket,'{' },
                { KeyCode.RightBracket,'}' },
                { KeyCode.Backslash,'|' },
                { KeyCode.Comma,'<' },
                { KeyCode.Period,'>'},
                { KeyCode.Slash,'?' },
                { KeyCode.Semicolon,':' },
                { KeyCode.Quote,'"' }
            };
            keyCodeNameDict = new Dictionary<KeyCode, string>()
            {
                { KeyCode.Equals,"="},
                { KeyCode.Space,""},
                { KeyCode.Backspace,"Backspace" },
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
                { KeyCode.Colon,":"},
                { KeyCode.DoubleQuote,"\"" },
                { KeyCode.Escape,"Esc"},
                { KeyCode.LeftControl,"Ctrl"},
                { KeyCode.RightControl,"Ctrl" },
                { KeyCode.KeypadEquals,"="},
                { KeyCode.KeypadPeriod,"."},
                { KeyCode.KeypadDivide,"/"},
            };
            keypadSet = new HashSet<KeyCode>();
            var keyCodeValues = Enum.GetValues(typeof(KeyCode));
            foreach (var value in keyCodeValues)
            {
                var str = value.ToString();
                if (str.StartsWith("Keypad"))
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
            if (!m_Init)
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

                m_Init = true;
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
            UpdateState();
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        internal virtual void UpdateState()
        {
            if (keyboard.IsShiftPress)
            {

            }
            else
            {

            }

            if (nameText == null)
                nameText = GetComponentInChildren<Text>();
            if (nameText != null)
            {
                var shiftNameStr = "";
                var nameStr = "";
                if (keyCodeNameDict.ContainsKey(m_KeyCode))
                {
                    nameStr = keyCodeNameDict[m_KeyCode];
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
                        OnRealKeyDown.Invoke(this);
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
                        OnRealKeyDown.Invoke(this);
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
                        OnRealKeyUp.Invoke(this);
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
                        OnRealKeyUp.Invoke(this);
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
                OnEnter.Invoke(this);
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
        /// 当前字符
        /// </summary>
        public char Character
        {
            get
            {
                return m_Character;
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