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
        protected static readonly float keyInterval = 0.1f;
        protected static readonly float keyIntervalDelay = 0.2f;

        protected Dictionary<KeyCode, UguiKeypress> keyDict;
        protected UguiKeypress[] keypressArr;
        protected Dictionary<KeyCode, int> keyDownStateDict;
        protected AudioSource audioSource;
        protected bool m_IsShiftPress;
        protected bool m_IsCapsLockOpen;
        protected bool m_IsUpper;

        protected GameObject cacheSelectObject;
        protected InputField cacheInputField;
        protected int cacheCaretPosition;
        protected int cacheSelectionAnchorPosition;
        protected int cacheSelectionFocusPosition;

        [Header("Sound")]
        [SerializeField] protected AudioClip keyDownSound;
        [SerializeField] protected AudioClip keyUpSound;

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
            }
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddMissingComponent<AudioSource>();

            InvokeRepeating(keyUpdate, keyInterval, keyInterval);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Init();
        }

        protected virtual void Update()
        {
            if (cacheSelectObject!=EventSystem.current)
            {
                cacheSelectObject = EventSystem.current.currentSelectedGameObject;
                if (cacheSelectObject != null)
                {
                    cacheInputField = cacheSelectObject.GetComponent<InputField>();
                    if (cacheInputField!=null)
                    {
                        //Debug.LogFormat("{0}  {1}  {2}",
                        //    cacheInputField.caretPosition,
                        //    cacheInputField.selectionFocusPosition,
                        //    cacheInputField.selectionAnchorPosition);
                        cacheCaretPosition = cacheInputField.caretPosition;
                        cacheSelectionAnchorPosition = cacheInputField.selectionAnchorPosition;
                        cacheSelectionFocusPosition = cacheInputField.selectionFocusPosition;
                    }
                }
            }
        }

        public virtual void Init()
        {
            m_IsCapsLockOpen = false;
            m_IsShiftPress = false;
            m_IsUpper = false;
            foreach (var keypress in keypressArr)
            {
                keypress.Init();
            }
        }

        internal void ForcusOnInputField()
        {
            if (cacheSelectObject!=null&&cacheInputField != null)
            {
                cacheInputField.ActivateInputField();
                cacheInputField.Select();
                cacheInputField.selectionAnchorPosition = cacheSelectionAnchorPosition;
                cacheInputField.selectionFocusPosition = cacheSelectionFocusPosition;
                cacheInputField.caretPosition = cacheCaretPosition;
                cacheInputField.ForceLabelUpdate();
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

        protected virtual void CheckStateChange(KeyCode keyCode,bool isKeyDown)
        {
            m_IsShiftPress = false;
            if (keyDict.ContainsKey(KeyCode.LeftShift))
            {
                if (keyDict[KeyCode.LeftShift].IsPress)
                {
                    m_IsShiftPress = true;
                }
            }
            if (keyDict.ContainsKey(KeyCode.RightShift))
            {
                if (keyDict[KeyCode.RightShift].IsPress)
                {
                    m_IsShiftPress = true;
                }
            }

            if (keyCode == KeyCode.CapsLock)
            {
                if (!isKeyDown)
                {
                    m_IsCapsLockOpen = !m_IsCapsLockOpen;
                }
            }
        }

        protected void OnKeyDownHandle(KeyCode keyCode)
        {
            if (!keyDownStateDict.ContainsKey(keyCode))
            {
                keyDownStateDict.Add(keyCode, 0);
                StartCoroutine(BeginCheckKeyDownState(keyCode));
            }
            CheckStateChange(keyCode,true);
            if (audioSource != null && keyDownSound != null)
            {
                audioSource.PlayOneShot(keyDownSound);
            }

            if (OnKeyDown != null)
            {
                OnKeyDown.Invoke(keyCode);
            }
        }

        protected void OnKeyUpHandle(KeyCode keyCode)
        {
            keyDownStateDict.Remove(keyCode);
            CheckStateChange(keyCode, false);
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
        /// 大小写是否锁定
        /// </summary>
        public bool IsCapsLockOpen { get { return m_IsCapsLockOpen; } }

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