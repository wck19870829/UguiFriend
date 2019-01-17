using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 顶部通知栏
    /// </summary>
    public class UguiNavigationBar : UguiLayoutGroup
    {
        [SerializeField] [Range(0.1f,10f)]protected float interval=1;                           //消息间时间间隔

        List<UguiNavigationBarItemData> messageList;
        float m_CacheTime;

        protected UguiNavigationBar()
        {
            messageList = new List<UguiNavigationBarItemData>();
        }

        protected virtual void Update()
        {
            if (Time.time - m_CacheTime >= interval)
            {
                m_CacheTime = Time.time;
                TryShowNext();
            }
        }

        protected virtual void TryShowNext()
        {
            if (messageList.Count > 0)
            {
                var current = messageList[0];
                messageList.RemoveAt(0);

            }
        }

        /// <summary>
        /// 添加一条消息
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(UguiNavigationBarItemData message)
        {
            messageList.Add(message);
        }

        public override void UpdateChildrenLocalPosition()
        {

        }
    }
}