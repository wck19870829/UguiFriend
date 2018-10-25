﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(AudioSource))]
    /// <summary>
    /// 虚拟键盘
    /// </summary>
    public class UguiKeyboard : UIBehaviour
    {
        const string keyUpdate = "CheckKeyDownState";
        protected static readonly float keyInterval = 0.1f;
        protected static readonly float keyIntervalDelay = 0.2f;

        protected Dictionary<KeyCode, UguiKeypress> keyDict;
        protected UguiKeypress[] keypressArr;
        protected Dictionary<KeyCode, int> keyDownStateDict;
        protected AudioSource audioSource;
        protected bool m_IsShiftPress;
        protected bool m_IsCtrlPress;
        protected bool m_IsAltPress;
        protected bool m_IsCapsLock;
        protected bool m_IsNumLock;
        protected bool m_IsScrollLock;
        protected bool m_IsUpper;

        [Header("Sound")]
        [SerializeField] protected AudioClip keyDownSound;
        [SerializeField] protected AudioClip keyUpSound;
        [SerializeField] protected AudioClip enterSound;

        public Action<KeyCode> OnKeyDown;
        public Action<KeyCode> OnKeyUp;
        public Action<KeyCode> OnKey;

        protected UguiKeyboard()
        {
            keyDict = new Dictionary<KeyCode, UguiKeypress>();
            keyDownStateDict = new Dictionary<KeyCode, int>();
        }

        protected override void Awake()
        {
            base.Awake();
            keypressArr = GetComponentsInChildren<UguiKeypress>();
        }

        protected override void Start()
        {
            base.Start();

            foreach (var key in keypressArr)
            {
                if (key.KeyCode == KeyCode.None)
                {
                    Debug.LogErrorFormat("Keypress's key code is None.");
                }
                else if (keyDict.ContainsKey(key.KeyCode))
                {
                    Debug.LogErrorFormat("{0} is repeat! Key:{1}", key.KeyCode, key);
                }
                else
                {
                    keyDict.Add(key.KeyCode, key);
                }

                if (key.ShiftKeyCode != KeyCode.None)
                {
                    if (keyDict.ContainsKey(key.ShiftKeyCode))
                    {
                        Debug.LogErrorFormat("{0} is repeat! Key:{1}", key.KeyCode,key);
                    }
                    else
                    {
                        keyDict.Add(key.ShiftKeyCode, key);
                    }
                }

                key.OnRealKeyDown += OnKeyDownHandle;
                key.OnRealKeyUp += OnKeyUpHandle;
                key.OnEnter += OnEnterHandle;
            }
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddMissingComponent<AudioSource>();

            InvokeRepeating(keyUpdate, keyInterval, keyInterval);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ResetKeyboard();
        }

        /// <summary>
        /// 重置键盘
        /// </summary>
        public virtual void ResetKeyboard()
        {
            m_IsCapsLock = false;
            m_IsShiftPress = false;
            m_IsAltPress = false;
            m_IsCtrlPress = false;
            m_IsUpper = false;
            foreach (var keypress in keypressArr)
            {
                keypress.ResetKeypress();
            }
        }

        void CheckKeyDownState()
        {
            if (OnKey != null)
            {
                foreach (var item in keyDownStateDict)
                {
                    if (item.Value > 0)
                    {
                        OnKey.Invoke(item.Key);
                    }
                }
            }
        }

        IEnumerator BeginCheckKeyDownState(KeyCode keyCode)
        {
            yield return new WaitForSeconds(keyIntervalDelay);

            if (keyDownStateDict.ContainsKey(keyCode))
            {
                keyDownStateDict[keyCode] = 1;
            }
        }

        protected virtual void CheckStateChange(KeyCode keyCode, UguiKeypress.KeypressState keypressState)
        {
            m_IsShiftPress = false;
            SetKeepPressState(KeyCode.LeftShift,ref m_IsShiftPress);
            SetKeepPressState(KeyCode.RightShift, ref m_IsShiftPress);

            m_IsAltPress = false;
            SetKeepPressState(KeyCode.LeftAlt, ref m_IsAltPress);
            SetKeepPressState(KeyCode.RightAlt, ref m_IsAltPress);

            m_IsCtrlPress = false;
            SetKeepPressState(KeyCode.LeftControl, ref m_IsCtrlPress);
            SetKeepPressState(KeyCode.RightControl, ref m_IsCtrlPress);

            m_IsCapsLock = false;
            SetKeepPressState(KeyCode.CapsLock, ref m_IsCapsLock);

            m_IsScrollLock = false;
            SetKeepPressState(KeyCode.ScrollLock, ref m_IsCapsLock);

            m_IsNumLock = false;
            SetKeepPressState(KeyCode.Numlock, ref m_IsCapsLock);

            m_IsUpper = (m_IsCapsLock==m_IsShiftPress)?
                        false:
                        (m_IsCapsLock||m_IsShiftPress);

            foreach (var keypress in keypressArr)
            {
                keypress.UpdateView();
            }
        }

        protected void SetKeepPressState(KeyCode keyCode,ref bool state)
        {
            if (keyDict.ContainsKey(keyCode))
            {
                if (keyDict[keyCode].State == UguiKeypress.KeypressState.Press)
                {
                    state = true;
                }
            }
        }

        protected virtual void OnEnterHandle(KeyCode keyCode)
        {
            if (audioSource != null && enterSound != null)
            {
                audioSource.PlayOneShot(enterSound);
            }
        }

        protected virtual void OnKeyDownHandle(KeyCode keyCode)
        {
            if (!keyDownStateDict.ContainsKey(keyCode))
            {
                keyDownStateDict.Add(keyCode, 0);
                StartCoroutine(BeginCheckKeyDownState(keyCode));
            }
            CheckStateChange(keyCode,UguiKeypress.KeypressState.Press);
            if (audioSource != null && keyDownSound != null)
            {
                audioSource.PlayOneShot(keyDownSound);
            }

            if (OnKeyDown != null)
            {
                OnKeyDown.Invoke(keyCode);
            }
        }

        protected virtual void OnKeyUpHandle(KeyCode keyCode)
        {
            keyDownStateDict.Remove(keyCode);
            CheckStateChange(keyCode, UguiKeypress.KeypressState.Normal);
            if (audioSource != null && keyUpSound != null)
            {
                audioSource.PlayOneShot(keyUpSound);
            }

            if (OnKeyUp != null)
            {
                OnKeyUp.Invoke(keyCode);
            }
        }

        /// <summary>
        /// Shift键是否按下
        /// </summary>
        public bool IsShiftPress { get { return m_IsShiftPress; } }

        /// <summary>
        /// Alt键是否按下
        /// </summary>
        public bool IsAltPress { get { return m_IsAltPress; } }

        /// <summary>
        /// Ctrl键是否按下
        /// </summary>
        public bool IsCtrlPress { get { return m_IsCtrlPress; } }

        /// <summary>
        /// 大小写是否锁定
        /// </summary>
        public bool IsCapsLock { get { return m_IsCapsLock; } }

        /// <summary>
        /// 数字键锁定
        /// </summary>
        public bool IsNumLock { get { return m_IsNumLock; } }

        /// <summary>
        /// 滚动锁定
        /// </summary>
        public bool IsScrollLock { get { return m_IsScrollLock; } }

        /// <summary>
        /// 是否为大写
        /// </summary>
        public bool IsUpper { get { return m_IsUpper; } }

        /// <summary>
        /// 按下按键
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="duration">按下状态持续时间</param>
        public virtual void Keystroke(KeyCode keyCode, float duration = 1)
        {
            if (keyDict.ContainsKey(keyCode))
            {
                keyDict[keyCode].Keystroke(duration);
            }
            else
            {
                Debug.LogErrorFormat("{0} is not found.", keyCode);
            }
        }
    }
}