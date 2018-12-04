using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 实现历史记录的元素
    /// </summary>
    public interface IUguiHistoryElement
    {
        /// <summary>
        /// 标识符,存储状态时作为Key值
        /// </summary>
        string GUID { get; }

        /// <summary>
        /// 深度复制当前状态,用于存储到历史记录中
        /// </summary>
        /// <returns></returns>
        object DeepCloneState();

        /// <summary>
        /// 返回上一步
        /// </summary>
        /// <param name="data"></param>
        void GotoPrevState(object data);

        /// <summary>
        /// 返回下一步
        /// </summary>
        /// <param name="data"></param>
        void GotoNextState(object data);

        /// <summary>
        /// 直接设置状态
        /// </summary>
        /// <param name="data"></param>
        void GotoState(object data);
    }
}