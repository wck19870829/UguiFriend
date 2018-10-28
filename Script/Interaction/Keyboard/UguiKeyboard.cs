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
    public abstract class UguiKeyboard : UIBehaviour
    {
        const string keyUpdate = "CheckKeyDownState";

        [Range(0.1f,1f)]
        [SerializeField]protected float keyInterval = 0.1f;

        [Range(0f,1f)]
        [SerializeField]protected float keyIntervalDelay = 0.2f;

        protected Dictionary<KeyCode, UguiKeypress> keyDict;
        protected UguiKeypress[] keypressArr;
        protected HashSet<UguiKeypress> waitingKeyDownStateSet;
        protected HashSet<UguiKeypress> keyDownStateSet;
        protected AudioSource audioSource;

        [Header("Sound")]
        [SerializeField] protected AudioClip keyDownSound;
        [SerializeField] protected AudioClip keyUpSound;
        [SerializeField] protected AudioClip enterSound;

        public Action<KeyCode,string> OnKeyDown;
        public Action<KeyCode,string> OnKeyUp;
        public Action<KeyCode,string> OnKey;

        static UguiKeyboard()
        {

        }

        protected UguiKeyboard()
        {
            keyDict = new Dictionary<KeyCode, UguiKeypress>();
            waitingKeyDownStateSet = new HashSet<UguiKeypress>();
            keyDownStateSet = new HashSet<UguiKeypress>();
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

        protected virtual void OnGUI()
        {
            //if(KeyCode.None!= Event.current.keyCode)
            //    Debug.Log(Event.current.keyCode);
        }

        /// <summary>
        /// 重置键盘到初始状态
        /// </summary>
        public virtual void ResetKeyboard()
        {
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
                    OnKey.Invoke(item.CurrentKeyCode,item.Character.ToString());
                }
            }
        }

        IEnumerator BeginCheckKeyDownState(UguiKeypress keypress)
        {
            yield return new WaitForSeconds(keyIntervalDelay);

            waitingKeyDownStateSet.Remove(keypress);
            keyDownStateSet.Add(keypress);
        }

        protected virtual void CheckStateChange(KeyCode keyCode, UguiKeypress.KeypressState keypressState)
        {
            foreach (var keypress in keypressArr)
            {
                keypress.UpdateState();
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
            if (!waitingKeyDownStateSet.Contains(keypress))
            {
                waitingKeyDownStateSet.Add(keypress);
                StartCoroutine(BeginCheckKeyDownState(keypress));
            }
            CheckStateChange(keypress.CurrentKeyCode, UguiKeypress.KeypressState.Press);
            if (audioSource != null && keyDownSound != null)
            {
                audioSource.PlayOneShot(keyDownSound);
            }

            if (OnKeyDown != null)
            {
                OnKeyDown.Invoke(keypress.CurrentKeyCode, keypress.Character);
            }
        }

        protected virtual void OnKeyUpHandler(UguiKeypress keypress)
        {
            keyDownStateSet.Remove(keypress);
            CheckStateChange(keypress.CurrentKeyCode, UguiKeypress.KeypressState.Normal);
            if (audioSource != null && keyUpSound != null)
            {
                audioSource.PlayOneShot(keyUpSound);
            }

            if (OnKeyUp != null)
            {
                OnKeyUp.Invoke(keypress.CurrentKeyCode, keypress.Character);
            }
        }

        protected abstract string ProcessCharacter(UguiKeypress keypress);

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