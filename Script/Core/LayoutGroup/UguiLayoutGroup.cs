using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 布局容器基类
    /// </summary>
    public abstract class UguiLayoutGroup :UIBehaviour,IUguiObjectLayoutGroup
    {
        protected const float reserveTimePerFrame = 10;                 //每帧计算时间上限(毫秒)

        [SerializeField] protected UguiObject m_ItemPrefabSource;
        [SerializeField] protected Rect m_ViewPortDisplayRect;          //视图坐标系显示区域,显示区域内的物体才会被创建更新

        [SerializeField] protected float m_RemoveDelay;                 //移除延迟,可以在此延迟中做动画等

        protected List<Vector3> m_ChildrenLocalPositionList;
        protected Dictionary<string, UguiObject> m_InSightChildDict;    //视图中可见的子物体
        protected HashSet<string> tempSet;
        protected HashSet<UguiObject> removeSet;
        protected List<UguiObjectData> m_ChildDataList;
        protected Canvas m_Canvas;

        public Action<UguiObject> OnRemoveItem;                         //移除子元素          
        public Action<UguiObject> OnCreateItem;                         //创建出新的子元素
        public Action OnReposition;                                     //复位回调

        protected UguiLayoutGroup()
        {
            m_InSightChildDict = new Dictionary<string, UguiObject>();
            m_ViewPortDisplayRect = Rect.MinMaxRect(-0.2f, -0.2f, 1.2f, 1.2f);
            m_ChildrenLocalPositionList = new List<Vector3>();
            tempSet = new HashSet<string>();
            removeSet = new HashSet<UguiObject>();
        }

        protected virtual void LateUpdate()
        {
            RefreshView();
        }

        /// <summary>
        /// 刷新视图
        /// </summary>
        protected void RefreshView()
        {
            if (m_Canvas == null) m_Canvas = GetComponentInParent<Canvas>();
            if (m_Canvas == null) return;

            for (var i = 0; i < m_ChildrenLocalPositionList.Count; i++)
            {
                var worldPoint = transform.TransformPoint(m_ChildrenLocalPositionList[i]);
                var screenPoint = RectTransformUtility.WorldToScreenPoint(m_Canvas.rootCanvas.worldCamera, worldPoint);
                var viewportPoint = UguiMathf.ScreenPoint2ViewportPoint(screenPoint);
                var childData = m_ChildDataList[i];
                if (m_ViewPortDisplayRect.Contains(viewportPoint))
                {
                    //在显示框中创建更新等
                    if (!m_InSightChildDict.ContainsKey(childData.guid))
                    {
                        //数据预测位置在显示框内,显示框中无对应的显示对象,那么创建新的
                        var obj = (m_ItemPrefabSource == null)
                                ? UguiObjectPool.Instance.Get(childData, transform)
                                : UguiObjectPool.Instance.Get(childData, m_ItemPrefabSource, transform);
                        obj.transform.position = worldPoint;
                        m_InSightChildDict.Add(childData.guid, obj);

                        ProcessItemAfterCreated(obj);

                        if (OnCreateItem != null)
                        {
                            OnCreateItem.Invoke(obj);
                        }
                    }
                    m_InSightChildDict[childData.guid].transform.position = worldPoint;
                }
                else
                {
                    //超出了显示区域外隐藏
                    if (m_InSightChildDict.ContainsKey(childData.guid))
                    {
                        var obj = m_InSightChildDict[childData.guid];
                        if (!removeSet.Contains(obj))
                        {
                            removeSet.Add(obj);
                        }
                    }
                }
            }

            foreach (var obj in removeSet)
            {
                StartCoroutine(RemoveItemDelay(obj));
            }
            removeSet.Clear();
        }

        IEnumerator RemoveItemDelay(UguiObject obj)
        {
            try
            {
                m_InSightChildDict.Remove(obj.Guid);

                if (OnRemoveItem != null)
                {
                    OnRemoveItem.Invoke(obj);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            yield return new WaitForSeconds(m_RemoveDelay);

            removeSet.Remove(obj);
            UguiObjectPool.Instance.Push(obj);
        }

        /// <summary>
        /// 更新子元素局部坐标位置信息
        /// </summary>
        public abstract void UpdateChildrenLocalPosition();

        protected void Set(List<UguiObjectData> childDataList)
        {
            m_ChildDataList = childDataList;
            m_ChildrenLocalPositionList.Clear();
            tempSet.Clear();
            foreach (var childData in childDataList)
            {
                tempSet.Add(childData.guid);
            }
            foreach (var item in m_InSightChildDict)
            {
                if (!tempSet.Contains(item.Key))
                {
                    if (!removeSet.Contains(item.Value))
                    {
                        removeSet.Add(item.Value);
                    }
                }
            }

            UpdateChildrenLocalPosition();
        }

        /// <summary>
        /// 创建后处理新建子元素
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void ProcessItemAfterCreated(UguiObject obj)
        {

        }

        public UguiObject ItemPrefabSource
        {
            get
            {
                return m_ItemPrefabSource;
            }
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