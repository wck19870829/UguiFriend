using UnityEngine;
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

        [Range(0.05f, 1f)]
        [SerializeField] protected float keyInterval = 0.08f;

        [Range(0.1f, 1f)]
        [SerializeField] protected float keyIntervalDelay = 0.2f;

        internal bool callNativeKeyboard;                                   //调用原生键盘事件
        protected Dictionary<KeyCode, UguiKeypress> keyDict;
        protected UguiKeypress[] keypressArr;
        protected HashSet<UguiKeypress> waitingKeyDownStateSet;
        protected HashSet<UguiKeypress> keyDownStateSet;
        protected AudioSource audioSource;
        protected Event m_ProcessingEvent;

        [Header("Sound")]
        [SerializeField] protected AudioClip keyDownSound;
        [SerializeField] protected AudioClip keyUpSound;
        [SerializeField] protected AudioClip enterSound;

        public Action<Event> OnKeyDown;
        public Action<Event> OnKeyUp;
        public Action<Event> OnKey;

        static UguiKeyboard()
        {

        }

        protected UguiKeyboard()
        {
            keyDict = new Dictionary<KeyCode, UguiKeypress>();
            waitingKeyDownStateSet = new HashSet<UguiKeypress>();
            keyDownStateSet = new HashSet<UguiKeypress>();
            m_ProcessingEvent = new Event();
        }

        protected override void Awake()
        {
            base.Awake();
            keypressArr = GetComponentsInChildren<UguiKeypress>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ResetKeyboard();
        }

        protected override void Start()
        {
            base.Start();

            foreach (var key in keypressArr)
            {
                if (key.RawKeyCode == KeyCode.None)
                {
                    Debug.LogErrorFormat("Keypress's key code is None.");
                }
                else if (keyDict.ContainsKey(key.RawKeyCode))
                {
                    Debug.LogErrorFormat("{0} is repeat! Key:{1}", key.RawKeyCode, key);
                }
                else
                {
                    keyDict.Add(key.RawKeyCode, key);
                }

                key.OnRealKeyDown += OnKeyDownHandler;
                key.OnRealKeyUp += OnKeyUpHandler;
                key.OnEnter += OnEnterHandler;
            }
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            InvokeRepeating(keyUpdate, keyInterval, keyInterval);
        }

        /// <summary>
        /// 重置键盘到初始状态
        /// </summary>
        public virtual void ResetKeyboard()
        {
            m_ProcessingEvent = new Event();

            waitingKeyDownStateSet.Clear();
            keyDownStateSet.Clear();
            foreach (var keypress in keypressArr)
            {
                keypress.ResetKeypress();
            }
        }

        protected virtual void CheckKeyDownState()
        {
            if (OnKey != null)
            {
                foreach (var item in keyDownStateSet)
                {
                    OnKey.Invoke(m_ProcessingEvent);
                }
            }
        }

        IEnumerator BeginCheckKeyDownState(UguiKeypress keypress)
        {
            yield return new WaitForSeconds(keyIntervalDelay);

            if (waitingKeyDownStateSet.Contains(keypress))
            {
                waitingKeyDownStateSet.Remove(keypress);
                keyDownStateSet.Add(keypress);
            }
        }

        protected virtual void CheckStateChange(UguiKeypress keypress)
        {
            var shiftPress = false;
            SetKeepPressState(KeyCode.LeftShift, ref shiftPress);
            SetKeepPressState(KeyCode.RightShift, ref shiftPress);

            var altPress = false;
            SetKeepPressState(KeyCode.LeftAlt, ref altPress);
            SetKeepPressState(KeyCode.RightAlt, ref altPress);

            var ctrlPress = false;
            SetKeepPressState(KeyCode.LeftControl, ref ctrlPress);
            SetKeepPressState(KeyCode.RightControl, ref ctrlPress);

            var capsLock = false;
            SetKeepPressState(KeyCode.CapsLock, ref capsLock);

            var scrollLock = false;
            SetKeepPressState(KeyCode.ScrollLock, ref scrollLock);

            var numLock = false;
            SetKeepPressState(KeyCode.Numlock, ref numLock);

            var windowsPress = false;
            SetKeepPressState(KeyCode.LeftWindows, ref windowsPress);
            SetKeepPressState(KeyCode.RightWindows, ref windowsPress);

            var commandPress = false;
            SetKeepPressState(KeyCode.LeftCommand, ref commandPress);
            SetKeepPressState(KeyCode.RightCommand, ref commandPress);

            m_ProcessingEvent.numeric = numLock;
            m_ProcessingEvent.alt = altPress;
            m_ProcessingEvent.control = ctrlPress;
            m_ProcessingEvent.capsLock = capsLock;
            m_ProcessingEvent.shift = shiftPress;
            m_ProcessingEvent.command = windowsPress|commandPress;

            var modifiers = EventModifiers.None;
            if (altPress) modifiers = modifiers | EventModifiers.Alt;
            if (ctrlPress) modifiers = modifiers | EventModifiers.Control;
            if (capsLock) modifiers = modifiers | EventModifiers.CapsLock;
            if (shiftPress) modifiers = modifiers | EventModifiers.Shift;
            if (m_ProcessingEvent.command) modifiers = modifiers | EventModifiers.Command;
            if (numLock) modifiers = modifiers | EventModifiers.Numeric;
            if (m_ProcessingEvent.functionKey) modifiers = modifiers | EventModifiers.FunctionKey;
            m_ProcessingEvent.modifiers = modifiers;

            m_ProcessingEvent.character = keypress.Character;
            m_ProcessingEvent.keyCode = keypress.CurrentKeyCode;
            m_ProcessingEvent.type = (keypress.State == UguiKeypress.KeypressState.Normal) ?
                        EventType.KeyUp :
                        EventType.KeyDown;

            foreach (var k in keypressArr)
            {
                k.UpdateState();
            }
        }

        protected void SetKeepPressState(KeyCode keyCode, ref bool state)
        {
            if (keyDict.ContainsKey(keyCode))
            {
                if (keyDict[keyCode].State == UguiKeypress.KeypressState.Press)
                {
                    state = true;
                }
            }
        }

        protected virtual void OnEnterHandler(UguiKeypress keypress)
        {
            if (audioSource != null && enterSound != null)
            {
                audioSource.PlayOneShot(enterSound);
            }
        }

        protected virtual void OnKeyDownHandler(UguiKeypress keypress)
        {
            if (!UguiKeypress.IsKeepPressKey(keypress.RawKeyCode))
            {
                if (!waitingKeyDownStateSet.Contains(keypress))
                {
                    waitingKeyDownStateSet.Add(keypress);
                    StartCoroutine(BeginCheckKeyDownState(keypress));
                }
            }

            CheckStateChange(keypress);

            if (audioSource != null && keyDownSound != null)
            {
                audioSource.PlayOneShot(keyDownSound);
            }

            if (OnKeyDown != null)
            {
                OnKeyDown.Invoke(m_ProcessingEvent);
            }
        }

        protected virtual void OnKeyUpHandler(UguiKeypress keypress)
        {
            keyDownStateSet.Remove(keypress);
            waitingKeyDownStateSet.Remove(keypress);

            CheckStateChange(keypress);

            if (audioSource != null && keyUpSound != null)
            {
                audioSource.PlayOneShot(keyUpSound);
            }

            if (OnKeyUp != null)
            {
                OnKeyUp.Invoke(m_ProcessingEvent);
            }
        }

        /// <summary>
        /// 虚拟键盘的事件，用法类似于Event.current
        /// </summary>
        public Event ProcessingEvent
        {
            get
            {
                return m_ProcessingEvent;
            }
        }

        /// <summary>
        /// 是否大写
        /// </summary>
        public bool IsUpper
        {
            get
            {
                var isUpper = (m_ProcessingEvent.capsLock == m_ProcessingEvent.shift) ?
                            false :
                            (m_ProcessingEvent.capsLock || m_ProcessingEvent.shift);
                return isUpper;
            }
        }

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