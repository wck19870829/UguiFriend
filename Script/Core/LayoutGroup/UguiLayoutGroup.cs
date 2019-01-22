using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(UguiOutsideScreenRecovererObject))]
    /// <summary>
    /// 布局容器基类
    /// </summary>
    public abstract class UguiLayoutGroup :UIBehaviour,IUguiObjectLayoutGroup
    {
        [SerializeField] protected UguiObject m_ItemPrefabSource;
        [SerializeField] protected float m_RemoveDelay=0.2f;            //移除延迟,可以在此延迟中做动画等
        protected UguiOutsideScreenRecovererObject m_Recoverer;
        protected List<Vector3> m_ChildrenLocalPositionList;
        protected Dictionary<string, UguiObject> m_InSightChildDict;    //视图中可见的子物体
        protected List<UguiObjectData> m_ChildDataList;
        protected Dictionary<string, UguiObjectData> m_ChildDataDict;
        protected Canvas m_Canvas;

        [SerializeField] protected string m_RemoveAnimState;            //移除时播放动画
        [SerializeField] protected string m_AddAnimState;               //添加时播放动画

        public Action OnReposition;                                     //复位回调

        protected UguiLayoutGroup()
        {
            m_InSightChildDict = new Dictionary<string, UguiObject>();
            m_ChildrenLocalPositionList = new List<Vector3>();
            m_ChildDataDict = new Dictionary<string, UguiObjectData>();
            m_ChildDataList = new List<UguiObjectData>();
        }

        protected override void Start()
        {
            base.Start();
            m_Recoverer = GetComponent<UguiOutsideScreenRecovererObject>();
            if (m_Recoverer == null)
                m_Recoverer = gameObject.AddComponent<UguiOutsideScreenRecovererObject>();
            m_Recoverer.OnRecycle += OnItemRecycle;
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
            if (m_Canvas == null)
                m_Canvas = GetComponentInParent<Canvas>();
            if (m_Canvas == null) return;
            if (m_Recoverer == null) return;

            var viewPort = m_Recoverer.ViewPortDisplayRect;
            for (var i = 0; i < m_ChildrenLocalPositionList.Count; i++)
            {
                var worldPoint = transform.TransformPoint(m_ChildrenLocalPositionList[i]);
                if (UguiTools.InScreenViewRect(worldPoint, m_Canvas.rootCanvas, viewPort))
                {
                    var childData = m_ChildDataList[i];
                    //在显示框中创建更新
                    if (!m_InSightChildDict.ContainsKey(childData.guid))
                    {
                        //数据预测位置在显示框内,显示框中无对应的显示对象,那么创建新的
                        var obj = GetItemClone(childData);
                        obj.transform.SetParent(transform);
                        obj.transform.position = worldPoint;
                        m_InSightChildDict.Add(childData.guid, obj);

                        ProcessItemAfterCreated(obj);


                    }
                }
            }
        }

        /// <summary>
        /// 更新子元素局部坐标位置信息
        /// </summary>
        public abstract void UpdateChildrenLocalPosition();

        protected UguiObject GetItemClone(UguiObjectData data)
        {
            var clone = (m_ItemPrefabSource == null)
                    ? UguiObjectPool.Instance.Get(data, transform)
                    : UguiObjectPool.Instance.Get(data, m_ItemPrefabSource, transform);

            return clone;
        }

        /// <summary>
        /// 直接整体强制设置
        /// </summary>
        /// <param name="childDataList"></param>
        public virtual void Set(List<UguiObjectData> childDataList)
        {
            m_ChildrenLocalPositionList.Clear();
            m_ChildDataDict.Clear();
            m_ChildDataList.Clear();

            m_ChildDataList.AddRange(childDataList);
            foreach (var data in m_ChildDataList)
            {
                m_ChildDataDict.Add(data.guid,data);
            }
            var removeSet = new HashSet<string>();
            foreach (var childKey in m_InSightChildDict.Keys)
            {
                if (!m_ChildDataDict.ContainsKey(childKey))
                {
                    removeSet.Add(childKey);
                }
            }
            foreach (var item in removeSet)
            {
                UguiObjectPool.Instance.Push(m_InSightChildDict[item]);
                m_InSightChildDict.Remove(item);
            }

            UpdateChildrenLocalPosition();

            RefreshView();
        }

        /// <summary>
        ///  添加一个元素
        /// </summary>
        /// <param name="data"></param>
        public virtual void AddItem(UguiObjectData data)
        {

        }

        /// <summary>
        /// 移除一个元素
        /// </summary>
        /// <param name="data"></param>
        public virtual void RemoveItem(UguiObjectData data)
        {

        }

        /// <summary>
        /// 移出屏幕回收元素
        /// </summary>
        /// <param name="child"></param>
        protected virtual void OnItemRecycle(UguiObject child)
        {
            m_InSightChildDict.Remove(child.Guid);
        }

        /// <summary>
        /// 创建后处理新建子元素
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void ProcessItemAfterCreated(UguiObject obj)
        {
            
        }

        /// <summary>
        /// 子元素预设
        /// 如此值不为null,那么通过复制此值创建子元素.否则通过Data类型绑定的实体创建
        /// </summary>
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

        /// <summary>
        /// 轴向
        /// </summary>
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