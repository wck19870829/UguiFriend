using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 简单的UguiWidget元素容器
    /// 适用于创建小容量的物体
    /// </summary>
    public class UguiWidgetContent : MonoBehaviour
    {
        [SerializeField]protected Transform container;
        protected List<UguiWidget> m_Children;
        protected List<UguiWidgetData> m_DataList;
        protected Transform pool;
        protected List<UguiWidget> poolList;
        bool m_Init;
        UguiWidget ui;

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            if (!m_Init)
            {
                m_Children = new List<UguiWidget>();
                poolList = new List<UguiWidget>();
                pool = new GameObject("Pool").transform;
                pool.SetParent(transform.parent);
                pool.transform.position = new Vector3(99999, 99999,99999);
                if (!container)container = transform;

                 m_Init = true;
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="dataList"></param>
        public virtual void Create(List<UguiWidgetData> dataList)
        {
            m_DataList = dataList;

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

        protected virtual UguiWidget Get(UguiWidgetData data)
        {
            var bindingInfo = UguiWidget.GetBindingInfo(data);
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
                ui = UguiWidget.GetInstanceElementByData(data) as UguiWidget;
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
        public virtual List<UguiWidget> Children
        {
            get
            {
                return m_Children;
            }
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        public List<UguiWidgetData> DataList
        {
            get
            {
                return m_DataList;
            }
        }
    }
}