using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 优化的DisplayObject容器
    /// 元素会重复使用,适用于背包等需要创建大量物体的容器
    /// </summary>
    public class GoodDisplayObjectContent:DisplayObjectContent
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList"></param>
        /// <param name="scale"></param>
        public override void Create<T>(List<T> dataList)
        {

        }
    }
}
