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
        [SerializeField] protected Rect m_ViewPortDisplayRect;          //视图坐标系显示区域,显示区域内的物体才会被创建更新
        protected List<Vector3> m_ChildrenLocalPositionList;
        protected Dictionary<string, UguiObject> m_InSightChildDict;    //视图中可见的子物体
        protected List<UguiObjectData> m_ChildrenDataList;
        protected Canvas m_Canvas;

        public Action<UguiObject> OnItemCreate;                         //创建出新的子元素
        public Action OnReposition;                                     //复位回调

        protected UguiLayoutGroup()
        {
            m_InSightChildDict = new Dictionary<string, UguiObject>();
            m_ViewPortDisplayRect = Rect.MinMaxRect(-0.2f, -0.2f, 1.2f, 1.2f);
            m_ChildrenLocalPositionList = new List<Vector3>();
        }

        protected virtual void Update()
        {
            UpdateViews();
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            m_Canvas = GetComponentInParent<Canvas>();
        }

        /// <summary>
        /// 刷新视图
        /// </summary>
        protected virtual void UpdateViews()
        {
            if (m_Canvas == null)
                m_Canvas = GetComponentInParent<Canvas>();
            if (m_Canvas == null) return;

            for (var i = 0; i < m_ChildrenLocalPositionList.Count; i++)
            {
                var worldPoint = transform.TransformPoint(m_ChildrenLocalPositionList[i]);
                var screenPoint = RectTransformUtility.WorldToScreenPoint(m_Canvas.rootCanvas.worldCamera, worldPoint);
                var viewportPoint = UguiMathf.ScreenPoint2ViewportPoint(screenPoint);
                var data = m_ChildrenDataList[i];
                if (m_ViewPortDisplayRect.Contains(viewportPoint))
                {
                    //在显示框中创建更新等
                    if (!m_InSightChildDict.ContainsKey(data.guid))
                    {
                        //数据预测位置在显示框内,显示框中无对应的显示对象,那么创建新的
                        var obj = (m_PrefabSource == null)
                                ? UguiObjectPool.Instance.Get(data, transform)
                                : UguiObjectPool.Instance.Get(data, m_PrefabSource, transform);
                        m_InSightChildDict.Add(data.guid, obj);

                        ProcessItemAfterCreated(obj);

                        if (OnItemCreate != null)
                        {
                            OnItemCreate.Invoke(obj);
                        }
                    }
                    m_InSightChildDict[data.guid].transform.position = worldPoint;
                }
                else
                {
                    //超出了显示区域外放入到池中
                    if (m_InSightChildDict.ContainsKey(data.guid))
                    {
                        var obj = m_InSightChildDict[data.guid];
                        m_InSightChildDict.Remove(data.guid);
                        UguiObjectPool.Instance.Push(obj);
                    }
                }
            }
        }

        /// <summary>
        /// 更新子元素局部坐标位置信息
        /// </summary>
        public abstract void UpdateChildrenLocalPosition();

        /// <summary>
        /// 创建后处理新建子元素
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void ProcessItemAfterCreated(UguiObject obj)
        {

        }

        /// <summary>
        /// 子元素数据
        /// </summary>
        public virtual List<UguiObjectData> ChildrenDataList
        {
            get
            {
                return m_ChildrenDataList;
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
            m_ChildrenDataList = childrenDataList;
            UpdateChildrenLocalPosition();
        }

        /// <summary>
        /// 复位
        /// </summary>
        public virtual void Reposition()
        {


            if (OnReposition!=null)
            {
                OnReposition.Invoke();
            }
        }

        public enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }

        public enum Corner
        {
            UpperLeft = 0,
            UpperRight = 1,
            LowerLeft = 2,
            LowerRight = 3
        }

        public enum Constraint
        {
            Flexible = 0,
            FixedColumnCount = 1,
            FixedRowCount = 2
        }
    }
}