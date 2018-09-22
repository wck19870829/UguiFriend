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
        public Vector2 spacine = new Vector2(100,100);
        public Axis startAxis=Axis.Horizontal;
        public int constraintCount;

        public override void Create(List<DisplayObjectData> dataList)
        {
            m_DataList = dataList;
        }

        protected virtual void Update()
        {
            Checker();
        }

        public override List<DisplayObject> Children
        {
            get
            {
                Debug.LogErrorFormat("不应当使用此属性.");
                return null;
            }
        }

        protected void Checker()
        {
            if (m_DataList==null) return;
            if (!container) return;

            var index = 0;
            switch (startAxis)
            {
                case Axis.Horizontal:
                    foreach (var data in m_DataList)
                    {

                        index++;
                    }
                    break;

                case Axis.Vertical:
                    foreach (var data in m_DataList)
                    {

                        index++;
                    }
                    break;
            }
        }

        public enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }
    }
}
