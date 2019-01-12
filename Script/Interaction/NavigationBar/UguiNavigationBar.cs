using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 顶部通知栏
    /// </summary>
    public class UguiNavigationBar : UIBehaviour
    {
        [SerializeField] protected float interval=1;                            //消息间时间间隔
        [SerializeField] protected UguiNavigationBarItem m_ItemSource;          //条目预设

        List<UguiNavigationBarItemData> messageList;

        protected UguiNavigationBar()
        {
            messageList = new List<UguiNavigationBarItemData>();
        }

        protected virtual void Update()
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
    }
}