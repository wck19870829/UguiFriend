using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Text;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 键盘按键
    /// </summary>
    public abstract class UguiKeypress : Selectable
    {
        public const char emptyCharacter = char.MinValue;                               //空字符

        protected static HashSet<KeyCode> keepPressSet;                                 //可以挂起的按键
        protected static Dictionary<KeyCode, string> inputCharacterDict;                //可以输入的字符
        protected static Dictionary<KeyCode, string> displayNameDict;                   //显示名称
        protected static Dictionary<KeyCode, KeyCode> numLockDict;                      //NumLock锁定时按键映射

        [SerializeField] protected bool m_AutoName=true;
        [SerializeField] protected KeyCode m_RawKeyCode;                                //按键原始按键码
        protected KeyCode m_CurrentKeyCode;                                             //当前状态的按键码
        protected Text nameText;
        protected KeypressState m_State;
        protected UguiKeyboard keyboard;
        protected int keyDownCount;
        protected bool m_Init;

        public Action<UguiKeypress> OnRealKeyDown;                                      //按键按下
        public Action<UguiKeypress> OnRealKeyUp;                                        //按键弹起
        public Action<UguiKeypress> OnEnter;                                            //鼠标等进入按键

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
            numLockDict = new Dictionary<KeyCode, KeyCode>()
            {
                {KeyCode.Keypad0,KeyCode.Insert },
                {KeyCode.Keypad1,KeyCode.End },
                {KeyCode.Keypad2,KeyCode.DownArrow },
                {KeyCode.Keypad3,KeyCode.PageDown },
                {KeyCode.Keypad4,KeyCode.LeftArrow },
                {KeyCode.Keypad6,KeyCode.RightArrow },
                {KeyCode.Keypad7,KeyCode.Home },
                {KeyCode.Keypad8,KeyCode.UpArrow },
                {KeyCode.Keypad9,KeyCode.PageUp },
                {KeyCode.KeypadPeriod,KeyCode.Delete }
            };
            inputCharacterDict = new Dictionary<KeyCode, string>()
            {
                { KeyCode.Keypad0,"0" },
                { KeyCode.Keypad1,"1" },
                { KeyCode.Keypad2,"2" },
                { KeyCode.Keypad3,"3" },
                { KeyCode.Keypad4,"4" },
                { KeyCode.Keypad5,"5" },
                { KeyCode.Keypad6,"6" },
                { KeyCode.Keypad7,"7" },
                { KeyCode.Keypad8,"8" },
                { KeyCode.Keypad9,"9" },
                { KeyCode.KeypadDivide,"/" },
                { KeyCode.KeypadMinus,"-" },
                { KeyCode.KeypadMultiply,"*" },
                { KeyCode.KeypadPeriod,"." },
                { KeyCode.KeypadPlus,"+" },
                { KeyCode.KeypadEnter,"\r\n" },
                { KeyCode.Return,"\r\n"}
            };
            var keyCodeValues = Enum.GetValues(typeof(KeyCode));
            var encoding = new ASCIIEncoding();
            foreach (KeyCode value in keyCodeValues)
            {
                var vInt = (int)value;
                if (vInt >= 32 && vInt <= 122)
                {
                    //ASCII
                    var str = encoding.GetString(new byte[] { (byte)vInt });
                    char ch;
                    if (Char.TryParse(str, out ch))
                    {
                        inputCharacterDict.Add(value, ch.ToString());
                    }
                }
            }

            displayNameDict = new Dictionary<KeyCode, string>()
            {
                { KeyCode.Backspace,"Backspace" },
                { KeyCode.CapsLock,"Caps Lock" },
                { KeyCode.LeftShift,"Shift" },
                { KeyCode.RightShift,"Shift" },
                { KeyCode.LeftAlt,"Alt" },
                { KeyCode.RightAlt,"Alt" },
                { KeyCode.Escape,"Esc"},
                { KeyCode.LeftControl,"Ctrl"},
                { KeyCode.RightControl,"Ctrl" },
                { KeyCode.Return,"Enter" },
                { KeyCode.UpArrow,"↑" },
                { KeyCode.DownArrow,"↓" },
                { KeyCode.LeftArrow,"←" },
                { KeyCode.RightArrow,"→" },
                { KeyCode.KeypadEnter,"Enter" },
                { KeyCode.Numlock,"Num\r\nLock" },
                { KeyCode.KeypadEquals,"=" }
            };
            foreach (var item in inputCharacterDict)
            {
                if (!displayNameDict.ContainsKey(item.Key))
                {
                    displayNameDict.Add(item.Key, item.Value.ToString());
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
                    if (m_RawKeyCode == KeyCode.None)
                    {
                        var keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), name);
                        m_RawKeyCode = keyCode;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                if (m_RawKeyCode == KeyCode.None)
                    throw new Exception("Key code is None.");
                m_CurrentKeyCode = m_RawKeyCode;

                keyboard = GetComponentInParent<UguiKeyboard>();
                if (keyboard == null)
                    throw new Exception("Keyboard is null.");

                m_Init = true;
            }
        }

        /// <summary>
        /// 获取显示名称
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        protected virtual string GetKeyCodeDisplayName(KeyCode keyCode)
        {
            if (displayNameDict.ContainsKey(keyCode))
            {
                 return displayNameDict[keyCode];
            }
            else
            {
                return string.Intern(keyCode.ToString());
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
        public abstract void UpdateState();

        /// <summary>
        /// 获得输入字符,如对应的键无输入字符，返回emptyCharacter
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        protected abstract string GetInputCharacter();

        protected virtual void KeyDown()
        {
            if (keepPressSet.Contains(m_RawKeyCode))
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
                if (m_State == KeypressState.Normal)
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

        protected virtual void KeyUp()
        {
            if (keepPressSet.Contains(m_RawKeyCode))
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
        /// 当前状态对应的KeyCode
        /// </summary>
        public KeyCode CurrentKeyCode
        {
            get
            {
                return m_CurrentKeyCode;
            }
        }

        /// <summary>
        /// 原始KeyCode
        /// </summary>
        public KeyCode RawKeyCode
        {
            get
            {
                return m_RawKeyCode;
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
        public string Character
        {
            get
            {
                return GetInputCharacter();
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

        #region 静态方法

        /// <summary>
        /// 是否为字母
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        protected static bool IsLetter(KeyCode keyCode)
        {
            char ch;
            if (!Char.TryParse(string.Intern(keyCode.ToString()), out ch))
                return false;

            return Char.IsLetter(ch);
        }

        #endregion
    }
}