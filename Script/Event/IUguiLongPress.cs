using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 长按接口
    /// </summary>
    public interface IUguiLongPress: IEventSystemHandler
    {
        /// <summary>
        /// 长按开始
        /// </summary>
        void OnLongPressBegin();

        /// <summary>
        /// 长按中
        /// </summary>
        void OnLongPressTick();

        /// <summary>
        /// 长按结束
        /// </summary>
        void OnLongPressEnd();
    }
}