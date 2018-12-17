using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 手势输入基类
    /// </summary>
    public abstract class UguiGestureInputBase : UIBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        protected List<PointerEventData> pointerDataList;       //输入的点
        protected List<ScreenPointInfo> screenPointInfoList;    //屏幕坐标点记录
        protected List<Vector2> screenPointList;                //屏幕坐标所有点集合
        protected List<WorldPointInfo> worldPointInfoList;      //屏幕映射到世界坐标系物体上的点
        protected Vector2 screenCenter;                         //物体屏幕坐标中点
        protected Vector3 worldCenter;                          //物体世界坐标中点

        protected override void Awake()
        {
            base.Awake();

            pointerDataList = new List<PointerEventData>();
            screenPointInfoList = new List<ScreenPointInfo>();
            screenPointList = new List<Vector2>();
            worldPointInfoList = new List<WorldPointInfo>();
        }

        protected virtual void Update()
        {
            if (pointerDataList.Count > 0)
            {
                CalculateRelativePointers();
            }
        }

        protected virtual void CalculateRelativePointers()
        {
            screenPointInfoList.Clear();
            screenPointList.Clear();
            worldPointInfoList.Clear();
            foreach (var pointer in pointerDataList)
            {
                var prev = pointer.position - pointer.delta;
                var current = pointer.position;
                var screenPointInfo = new ScreenPointInfo(prev, current);
                screenPointInfoList.Add(screenPointInfo);

                screenPointList.Add(pointer.position);

                var prevWorldPoint = Vector3.zero;
                var currentWorldPoint = Vector3.zero;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, prev, pointerDataList[0].pressEventCamera, out prevWorldPoint);
                RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, current, pointerDataList[0].pressEventCamera, out currentWorldPoint);
                var worldPointInfo = new WorldPointInfo(prevWorldPoint, currentWorldPoint);
                worldPointInfoList.Add(worldPointInfo);
            }
            if (pointerDataList.Count == 1)
            {
                //一个点绕中心点旋转
                screenCenter = RectTransformUtility.WorldToScreenPoint(pointerDataList[0].pressEventCamera, transform.position);
            }
            else
            {
                //多点绕多点中心点旋转
                screenCenter = UguiMathf.GetCenter(screenPointList);
            }
            RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, screenCenter, pointerDataList[0].pressEventCamera, out worldCenter);

            DoChange();
        }

        protected abstract void DoChange();

        public void OnPointerUp(PointerEventData eventData)
        {
            pointerDataList.Remove(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pointerDataList.Add(eventData);
        }

        protected struct ScreenPointInfo
        {
            public Vector2 prev;
            public Vector2 current;

            public ScreenPointInfo(Vector2 prev,Vector2 current)
            {
                this.prev = prev;
                this.current = current;
            }
        }

        protected struct WorldPointInfo
        {
            public Vector3 prev;
            public Vector3 current;

            public WorldPointInfo(Vector2 prev, Vector2 current)
            {
                this.prev = prev;
                this.current = current;
            }
        }
    }
}