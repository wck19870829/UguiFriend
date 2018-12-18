using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [DisallowMultipleComponent]
    /// <summary>
    /// 简单实现的手势
    /// 复杂情况考虑使用其他专业手势插件
    /// </summary>
    public sealed class UguiGestureListener : UIBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler
    {
        static float s_LongPressCheckTime = 0.2f;

        Vector3 worldCenter;                                  //物体世界坐标中点
        List<PointerEventData> pointerEventDataList;          //输入的点
        List<PointInfo> worldPointInfoList;                   //屏幕映射到世界坐标系物体上的点
        List<PointInfo> screenPointInfoList;                  //屏幕坐标点记录
        List<Vector2> screenPointList;                        //屏幕坐标所有点集合

        public event Action OnPointerDown;          //按下
        public event Action OnPointerUp;            //抬起
        public event Action OnPointerEnter;         //进入
        public event Action OnPointerExit;          //移出
        public event Action OnPointerClick;         //单击
        public event Action OnPointerDoubleClick;   //双击
        public event Action OnLongPress;            //长按
        public event Action OnPinch;                //捏合
        public event Action OnRotation;             //旋转
        public event Action OnPan;                  //拖拽,慢速移动
        public event Action OnSwipe;                //滑动,快速移动

        protected override void Awake()
        {
            base.Awake();

            pointerEventDataList = new List<PointerEventData>();
            worldPointInfoList = new List<PointInfo>();
            screenPointInfoList = new List<PointInfo>();
            screenPointList = new List<Vector2>();
        }

        void Update()
        {
            if (pointerEventDataList.Count > 0)
            {
                worldPointInfoList.Clear();
                screenPointInfoList.Clear();
                screenPointList.Clear();

                var currentCamera = pointerEventDataList[0].pressEventCamera;
                foreach (var pointer in pointerEventDataList)
                {
                    var prev = pointer.position - pointer.delta;
                    var current = pointer.position;
                    var screenPointInfo = new PointInfo(prev, current);
                    screenPointInfoList.Add(screenPointInfo);

                    screenPointList.Add(pointer.position);

                    var prevWorldPoint = Vector3.zero;
                    var currentWorldPoint = Vector3.zero;
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, prev, currentCamera, out prevWorldPoint);
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, current, currentCamera, out currentWorldPoint);
                    var worldPointInfo = new PointInfo(prevWorldPoint, currentWorldPoint);
                    worldPointInfoList.Add(worldPointInfo);
                }
                var screenCenter = UguiMathf.GetCenter(screenPointList);
                var worldCenter = Vector3.zero;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, screenCenter, currentCamera, out worldCenter);

                var deltaScore = 0f;
                var refDir = Vector3.zero;
                foreach (var info in screenPointInfoList)
                {
                    refDir += info.dir;
                }
                refDir /= screenPointInfoList.Count;
                foreach (var info in screenPointInfoList)
                {
                    deltaScore += Vector3.Dot(info.dir, refDir);
                }
                deltaScore /= screenPointInfoList.Count;

                if (deltaScore > 0)
                {
                    //相同方向移动

                }
                else
                {
                    //不同方向移动

                }
            }
        }

        void CheckLongPress()
        {
            if (pointerEventDataList.Count > 0)
            {
                if (OnLongPress!=null)
                {
                    OnLongPress.Invoke();
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            pointerEventDataList.Add(eventData);
            if (pointerEventDataList.Count == 1)
            {
                Invoke("CheckLongPress", s_LongPressCheckTime);
            }

            if (OnPointerDown != null)
            {
                OnPointerDown.Invoke();
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (OnPointerEnter != null)
            {
                OnPointerEnter.Invoke();
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (OnPointerExit != null)
            {
                OnPointerExit.Invoke();
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            CancelInvoke();
            pointerEventDataList.Remove(eventData);

            if (OnPointerUp != null)
            {
                OnPointerUp.Invoke();
            }

            if (pointerEventDataList.Count == 0)
            {
                Debug.Log(eventData.clickCount);
            }
        }

        struct PointInfo
        {
            public Vector3 prev;
            public Vector3 current;
            public Vector3 dir;

            public PointInfo(Vector3 prev, Vector3 current)
            {
                this.prev = prev;
                this.current = current;
                dir = current - prev;
            }
        }
    }
}