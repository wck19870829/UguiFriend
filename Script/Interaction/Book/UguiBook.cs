using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 书
    /// </summary>
    public class UguiBook : UIBehaviour, 
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        [SerializeField][Range(1,20)] protected int aliveCount = 3;             //同时贮存页面
        protected PointerEventData dragData;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (dragData == null)
            {
                dragData = eventData;

                Debug.Log("Begin drag");
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragData==eventData)
            {
                Debug.Log("Drag");
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (dragData == eventData)
            {
                dragData = null;

                Debug.Log("End drag");
            }
        }

        /// <summary>
        /// 跳转到某页
        /// </summary>
        /// <param name="pageNum"></param>
        /// <returns>true:跳转成功;false:跳转失败</returns>
        public virtual bool GotoPage(int pageNum)
        {


            return false;
        }

        /// <summary>
        /// 上一页
        /// </summary>
        /// <returns>true:跳转成功;false:跳转失败</returns>
        public virtual bool PrevPage()
        {


            return false;
        }

        /// <summary>
        /// 下一页
        /// </summary>
        /// <returns>true:跳转成功;false:跳转失败</returns>
        public virtual bool NextPage()
        {


            return false;
        }

        /// <summary>
        /// 页码总数
        /// </summary>
        public int TotalPage
        {
            get
            {
                return 0;
            }
        }
    }
}