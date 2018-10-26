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
        public const char emptyCharacter = char.MinValue;                               //空字符
        protected static HashSet<KeyCode> keepPressSet;                                 //可以挂起的按键

        [SerializeField] protected KeyCode m_KeyCode;
        protected Text nameText;
        protected char m_Character;
        protected KeypressState m_State;
        protected UguiKeyboard keyboard;
        protected int keyDownCount;
        protected bool m_Init;

        public Action<UguiKeypress> OnRealKeyDown;                                      //按键按下
        public Action<UguiKeypress> OnRealKeyUp;                                        //按键弹起
        public Action<UguiKeypress> OnEnter;                                            //鼠标等进入按键

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

        }

        protected bool IsDigit(KeyCode keyCode)
        {
            char c;
            if (!char.TryParse(keyCode.ToString(), out c))
                return false;

            return Char.IsDigit(c);
        }

        protected bool IsNumber(KeyCode keyCode)
        {
            char c;
            if (!char.TryParse(keyCode.ToString(), out c))
                return false;

            return Char.IsNumber(c);
        }

        protected bool IsLetter(KeyCode keyCode)
        {
            char c;
            if (!char.TryParse(keyCode.ToString(), out c))
                return false;

            return Char.IsLetter(c);
        }

        protected virtual void KeyDown()
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
        /// 按键对应的KeyCode
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