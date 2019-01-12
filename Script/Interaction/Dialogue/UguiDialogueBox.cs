using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 对话窗
    /// </summary>
    public class UguiDialogueBox : UIBehaviour
    {
        [SerializeField] protected Text m_TitleText;
        [SerializeField] protected Text m_MessageText;
        [SerializeField] protected Button m_OkButton;
        [SerializeField] protected Button m_CancelButton;

        protected Action OnOkEvent;
        protected Action OnCancelEvent;

        public void Show(string title,string message,string ok,string cancel,Action onOk,Action onCancel)
        {
            m_TitleText.text = title;
            m_MessageText.text = message;

            OnOkEvent = onOk;
            OnCancelEvent = onCancel;
        }
    }
}