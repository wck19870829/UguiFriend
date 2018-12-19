using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 容器接口
    /// </summary>
    public interface IUguiContent
    {
        /// <summary>
        /// 子元素
        /// </summary>
        List<UguiObject> Children { get; }

        /// <summary>
        /// 设置数据填充实体
        /// </summary>
        /// <param name="dataList"></param>
        void Set(List<UguiObjectData> dataList);

        /// <summary>
        /// 复位
        /// </summary>
        void Reposition();

        /// <summary>
        /// 复位事件
        /// </summary>
        Action OnReposition { get; set; }
    }
}