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
        /// 注册
        /// 使用UguiHistoryManager.Register(this)方法注册
        /// </summary>
        void Register();

        /// <summary>
        /// 取消注册
        /// 使用UguiHistoryManager.Unregister(this)方法取消注册
        /// </summary>
        void Unregister();

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
        /// 上一步
        /// </summary>
        /// <param name="data"></param>
        void GotoPrevStep(object data);

        /// <summary>
        /// 下一步
        /// </summary>
        /// <param name="data"></param>
        void GotoNextStep(object data);

        /// <summary>
        /// 直接设置状态
        /// </summary>
        /// <param name="data"></param>
        void GotoStep(object data);
    }
}