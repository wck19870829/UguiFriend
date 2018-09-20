using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(Text))]
    /// <summary>
    /// 打字机
    /// </summary>
    public class TypewriterEffect : MonoBehaviour
    {
        const string runningFunc = "Running";

        public float interval = 0.05f;
        public bool playOnEnable;

        Text target;
        StringBuilder strBuilder;
        string cacheStr;
        float cacheStartTime;

        public Action OnEffectComplete;

        private void Awake()
        {
            strBuilder = new StringBuilder(256);
            target = GetComponent<Text>();
        }

        private void OnEnable()
        {
            if (playOnEnable)
            {
                Show(target.text);
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void Show(string context)
        {
            strBuilder.Length = 0;
            target.text = string.Empty;
            cacheStr = context;
            cacheStartTime = Time.time;

            StopAllCoroutines();
            StartCoroutine(runningFunc);
        }

        IEnumerator Running()
        {
            yield return new WaitWhile(()=> 
            {
                return Time.time- cacheStartTime >= interval ? false:true;
            });

            cacheStartTime = Time.time;
            if (strBuilder.Length < cacheStr.Length)
            {
                strBuilder.Append(cacheStr[strBuilder.Length]);
                target.text = strBuilder.ToString();
            }

            if(strBuilder.Length< cacheStr.Length)
            {
                StartCoroutine(runningFunc);
            }
            else
            {
                if (OnEffectComplete != null)
                {
                    OnEffectComplete.Invoke();
                }
            }
        }
    }
}