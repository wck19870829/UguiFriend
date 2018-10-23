using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 虚拟键盘
    /// </summary>
    public class UguiKeyboard : UIBehaviour
    {
        const string keyUpdate = "CheckKeyDownState";
        protected static readonly float keyInterval = 0.1f;
        protected static readonly float keyIntervalDelay = 0.2f;

        protected Dictionary<KeyCode,UguiKeypress> keyDict;
        protected Dictionary<KeyCode, int> keyDownStateDict;

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

            var keypressArr = GetComponentsInChildren<UguiKeypress>();
            foreach (var key in keypressArr)
            {
                if (keyDict.ContainsKey(key.KeyCode))
                {
                    Debug.LogErrorFormat("{0} is repeat!",key.KeyCode);
                }
                else
                {
                    keyDict.Add(key.KeyCode,key);
                }

                key.OnKeyDown += OnKeyDownHandle;
                key.OnKeyUp += OnKeyUpHandle;
            }
            CallKeyUpdate();
        }

        protected void CallKeyUpdate()
        {
            CancelInvoke(keyUpdate);
            InvokeRepeating(keyUpdate, keyInterval, keyInterval);
        }

        void CheckKeyDownState()
        {
            if (OnKey!=null)
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

        protected void OnKeyDownHandle(KeyCode keyCode)
        {
            if (!keyDownStateDict.ContainsKey(keyCode))
            {
                keyDownStateDict.Add(keyCode, 0);
                BeginCheckKeyDownState(keyCode);
            }

            if (OnKeyDown != null)
            {
                OnKeyDown.Invoke(keyCode);
            }
        }

        protected void OnKeyUpHandle(KeyCode keyCode)
        {
            keyDownStateDict.Remove(keyCode);

            if (OnKeyUp != null)
            {
                OnKeyUp.Invoke(keyCode);
            }
        }

        /// <summary>
        /// 按下按键
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="duration">按下状态持续时间</param>
        public virtual void Keystroke(KeyCode keyCode,float duration= 1)
        {
            if (keyDict.ContainsKey(keyCode))
            {
                keyDict[keyCode].Keystroke(duration);
            }
            else
            {
                Debug.LogErrorFormat("{0} is not found.",keyCode);
            }
        }
    }
}