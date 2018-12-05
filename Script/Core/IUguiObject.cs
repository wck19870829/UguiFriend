using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    public interface IUguiObject
    {
        /// <summary>
        /// 数据
        /// </summary>
        UguiObjectData Data { get; set; }

        /// <summary>
        /// GUID
        /// </summary>
        string Guid { get; }

        /// <summary>
        /// 快照
        /// </summary>
        /// <returns></returns>
        UguiObjectData Snapshoot();

        /// <summary>
        /// 上一步
        /// </summary>
        /// <param name="data"></param>
        void GotoPrevStep(UguiObjectData data);

        /// <summary>
        /// 下一步
        /// </summary>
        /// <param name="data"></param>
        void GotoNextStep(UguiObjectData data);

        /// <summary>
        /// 直接设置状态
        /// </summary>
        /// <param name="data"></param>
        void GotoStep(UguiObjectData data);
    }
}