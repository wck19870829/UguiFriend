using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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
        protected bool m_Shift;

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
            OnKey += (x) => {
                Debug.LogFormat("onkey:{0}", x);
            };
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void Start()
        {
            base.Start();

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

        protected virtual void CheckShiftStateChange()
        {
            m_Shift = false;
            if (keyDict.ContainsKey(KeyCode.LeftShift))
            {
                if (keyDict[KeyCode.LeftShift].IsPress)
                {
                    m_Shift = true;
                }
            }
            if (keyDict.ContainsKey(KeyCode.RightShift))
            {
                if (keyDict[KeyCode.RightShift].IsPress)
                {
                    m_Shift = true;
                }
            }
            foreach (var keypress in keypressArr)
            {
                keypress.SetShiftState(m_Shift);
            }
        }

        protected void OnKeyDownHandle(KeyCode keyCode)
        {
            if (!keyDownStateDict.ContainsKey(keyCode))
            {
                keyDownStateDict.Add(keyCode, 0);
                StartCoroutine(BeginCheckKeyDownState(keyCode));
            }
            CheckShiftStateChange();
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
            CheckShiftStateChange();
            if (audioSource != null && keyUpSound != null)
            {
                audioSource.PlayOneShot(keyUpSound);
            }

            if (OnKeyUp != null)
            {
                OnKeyUp.Invoke(keyCode);
            }
        }

        public bool Shift
        {
            get
            {
                return m_Shift;
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