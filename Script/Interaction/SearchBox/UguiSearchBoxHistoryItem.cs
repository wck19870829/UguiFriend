using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 搜索框单条历史记录
    /// </summary>
    public class UguiSearchBoxHistoryItem : UIBehaviour,IPointerClickHandler
    {
        [SerializeField] protected Button m_DeleteButton;
        [SerializeField] protected Text m_HistoryText;

        protected string m_SearchText;

        protected override void Awake()
        {
            base.Awake();

            m_DeleteButton.onClick.AddListener(OnDeleteClickHandle);
        }

        public void SetText(string text)
        {
            m_SearchText = text;
            m_HistoryText.text = text;
        }

        protected virtual void OnDeleteClickHandle()
        {
            var searchBox = GetComponentInParent<UguiSearchBox>();
            if (searchBox)
            {
                searchBox.DeleteHistory(m_SearchText);
            }
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            
        }
    }
}