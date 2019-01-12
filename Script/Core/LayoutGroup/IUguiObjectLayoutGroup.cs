using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// Ugui对象容器接口
    /// </summary>
    public interface IUguiObjectLayoutGroup
    {
        /// <summary>
        /// 如果此值不为null,那么强制使用此预设实例化
        /// 一般情况下此值在编辑器模式下指定
        /// </summary>
        UguiObject ItemPrefabSource { get;}
    }
}