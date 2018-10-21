using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 容器接口，操控容器从而使容器内物体位移旋转缩放等变换
    /// 
    /// + IUguiChildControl
    /// |-Content(操控此物体)
    /// </summary>
    public interface IUguiChildControl
    {
        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="position"></param>
        void SetContentPosition(Vector3 position);

        /// <summary>
        /// 设置旋转
        /// </summary>
        /// <param name="angle"></param>
        void SetContentRotation(Vector3 angle);

        /// <summary>
        /// 设置缩放
        /// </summary>
        /// <param name="scale"></param>
        void SetContentScale(Vector3 scale);

        /// <summary>
        /// 容器
        /// </summary>
        Transform Content { get; }
    }

    /// <summary>
    /// 通过位置权重控制特效接口
    /// </summary>
    public interface IUguiEffectControlByPosition
    {
        float GetWeight(Transform target);
    }
}