using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 简单的DisplayObject元素容器
    /// 适用于创建小容量的物体
    /// </summary>
    public class DisplayObjectContent : MonoBehaviour
    {
        [SerializeField]protected Transform container;
        protected List<DisplayObject> m_Children;
        protected Transform pool;
        protected List<DisplayObject> poolList;
        bool m_Init;

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            if (!m_Init)
            {
                m_Children = new List<DisplayObject>();
                poolList = new List<DisplayObject>();
                pool = new GameObject("Pool").transform;
                pool.SetParent(transform.parent);
                pool.gameObject.SetActive(false);
                if (container == null) container = transform;

                 m_Init = true;
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList"></param>
        public virtual void Create<T>(List<T>dataList)where T:DisplayObjectData
        {
            Init();

            PushAll2Pool();
            foreach (var data in dataList)
            {
                var ui = Get(data);
                m_Children.Add(ui);
                ui.transform.SetParent(container);
            }
        }

        protected void PushAll2Pool()
        {
            foreach (var item in m_Children)
            {
                poolList.Add(item);
                item.transform.SetParent(pool);
            }
            m_Children.Clear();
        }

        protected virtual DisplayObject Get(DisplayObjectData data)
        {
            DisplayObject ui = null;
            var bindingInfo = DisplayObject.GetBindingInfo(data);
            if (bindingInfo != null)
            {
                foreach (var item in poolList)
                {
                    if (item.GetType() == bindingInfo.uiType)
                    {
                        ui = item;
                        break;
                    }
                }
            }

            //从池中移除
            if (ui != null)
            {
                poolList.Remove(ui);
            }

            //实例化新的
            if (ui==null)
            {
                ui = DisplayObject.GetInstanceElementByData(data);
                if (ui == null)
                {
                    Debug.LogErrorFormat("{0}数据未绑定到元素！", data);
                }
            }

            return ui;
        }

        /// <summary>
        /// 子元素
        /// </summary>
        public List<DisplayObject> Children
        {
            get
            {
                return m_Children;
            }
        }
    }
}