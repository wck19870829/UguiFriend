using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 布局容器基类
    /// </summary>
    public abstract class UguiLayoutGroup : LayoutGroup
    {
        [SerializeField] protected UguiObject m_PrefabSource;
        [SerializeField] protected bool m_Optimize;                     //是否使用优化模式,如果使用优化模式,会复用容器内元素,同时一些功能会受到限制
        [SerializeField] protected Rect m_ViewPortDisplayRect;          //视图坐标系显示区域,显示区域内的物体才会被创建更新
        protected List<Vector3> m_ChildrenLocalPositionList;
        protected List<UguiObject> m_Children;
        protected Dictionary<string, UguiObject> m_ChildrenDict;
        protected List<UguiObjectData> m_childrenDataList;
        protected Canvas m_Canvas;

        public Action OnReposition;

        protected UguiLayoutGroup()
        {
            m_Children = new List<UguiObject>();
            m_ChildrenDict = new Dictionary<string, UguiObject>();
            m_ViewPortDisplayRect = Rect.MinMaxRect(-0.2f, -0.2f, 1.2f, 1.2f);
            m_ChildrenLocalPositionList = new List<Vector3>();
        }

        protected virtual void Update()
        {
            UpdateChildrenWhenOptimize();
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            m_Canvas = GetComponentInParent<Canvas>();
        }

        /// <summary>
        /// 当使用优化模式时,动态更新子元素
        /// </summary>
        protected virtual void UpdateChildrenWhenOptimize()
        {
            if (m_Optimize)
            {
                if(m_Canvas==null)
                    m_Canvas = GetComponentInParent<Canvas>();

                for (var i=0;i< m_ChildrenLocalPositionList.Count;i++)
                {
                    var worldPoint =  transform.TransformPoint(m_ChildrenLocalPositionList[i]);
                    var screenPoint = RectTransformUtility.WorldToScreenPoint(m_Canvas.rootCanvas.worldCamera, worldPoint);
                    var viewportPoint = UguiMathf.ScreenPoint2ViewportPoint(screenPoint);
                    var data = m_childrenDataList[i];
                    if (m_ViewPortDisplayRect.Contains(viewportPoint))
                    {
                        //在显示框中创建更新等
                        if (!m_ChildrenDict.ContainsKey(data.guid))
                        {
                            //数据预测位置在显示框内,显示框中无对应的显示对象,那么创建新的
                            var obj=(m_PrefabSource==null)
                                    ?UguiObjectPool.Instance.Get(data,transform)
                                    :UguiObjectPool.Instance.Get(data,m_PrefabSource, transform);
                            m_ChildrenDict.Add(data.guid, obj);
                            m_Children.Add(obj);
                        }
                        m_ChildrenDict[data.guid].transform.position = worldPoint;
                    }
                    else
                    {
                        //超出了显示区域外放入到池中
                        if (m_ChildrenDict.ContainsKey(data.guid))
                        {
                            var obj = m_ChildrenDict[data.guid];
                            m_Children.Remove(obj);
                            m_ChildrenDict.Remove(data.guid);
                            UguiObjectPool.Instance.Push(obj);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新子元素局部坐标位置信息
        /// </summary>
        protected abstract void UpdateChildrenLocalPosition();

        /// <summary>
        /// 子元素列表
        /// [注]当使用优化模式时不可使用
        /// </summary>
        public virtual List<UguiObject> Children
        {
            get
            {
                if (m_Optimize)
                {
                    throw new Exception("Please do not get children when optimize is ture.");
                }

                return m_Children;
            }
        }

        /// <summary>
        /// 子元素数据
        /// </summary>
        public virtual List<UguiObjectData> ChildrenDataList
        {
            get
            {
                return m_childrenDataList;
            }
            set
            {
                Set(value);
            }
        }

        /// <summary>
        /// 设置子元素数据
        /// </summary>
        /// <param name="childrenDataList"></param>
        public virtual void Set(List<UguiObjectData> childrenDataList)
        {
            m_childrenDataList = childrenDataList;
            UpdateChildrenLocalPosition();

            if (!m_Optimize)
            {
                UguiTools.SetChildrenDatas(m_Children, transform, childrenDataList, m_PrefabSource);
                for (var i=0;i< m_Children.Count;i++)
                {
                    m_Children[i].transform.localPosition = m_ChildrenLocalPositionList[i];
                }
            }
        }

        public virtual void Reposition()
        {

        }
    }
}