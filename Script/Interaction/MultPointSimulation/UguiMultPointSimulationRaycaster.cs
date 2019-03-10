using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(Canvas))]
    public sealed class UguiMultPointSimulationRaycaster : BaseRaycaster
    {
        static int defaultTouchIconSize=20;
        static int touchIconSize;
        static int pointerId;
        static int selectId;

        Dictionary<int, PointerEventData> pointerDict;
        Texture2D touchTex;
        Texture2D currentTouchTex;
        Canvas canvas;

        protected override void Awake()
        {
            touchTex = Resources.Load<Texture2D>("UguiFriend/Texture/TouchPoint");
            currentTouchTex = Resources.Load<Texture2D>("UguiFriend/Texture/CurrentTouchPoint");
            pointerDict = new Dictionary<int, PointerEventData>();

            canvas = gameObject.GetComponent<Canvas>();
            if(!canvas)
                canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue;
        }

        private void OnGUI()
        {
            touchIconSize = (int)(defaultTouchIconSize*Mathf.Max(Screen.width/1024f,Screen.height/768f));

            //渲染点
            foreach (var pointer in pointerDict.Values)
            {
                var pos = new Rect(
                            pointer.position.x- touchIconSize*0.5f,
                            Screen.height - pointer.position.y- touchIconSize*0.5f,
                            touchIconSize,
                            touchIconSize);
                if (selectId == pointer.pointerId)
                {
                    GUI.DrawTexture(pos, currentTouchTex);
                }
                else
                {
                    GUI.DrawTexture(pos, touchTex);
                }
            }
        }

        public override Camera eventCamera
        {
            get
            {
                return null;
            }
        }

        public override int sortOrderPriority
        {
            get
            {
                return canvas.sortingOrder;
            }
        }

        public override int renderOrderPriority
        {
            get
            {
                return canvas.renderOrder;
            }
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            //var castResult = new RaycastResult
            //{
            //    gameObject = go,
            //    module = this,
            //    distance = distance,
            //    screenPosition = eventPosition,
            //    index = resultAppendList.Count,
            //    depth = m_RaycastResults[index].depth,
            //    sortingLayer = canvas.sortingLayerID,
            //    sortingOrder = canvas.sortingOrder
            //};

            //删除点
            if (pointerDict.ContainsKey(selectId))
            {
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    pointerDict.Remove(selectId);

                    //模拟PointerUp事件
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                {
                    //添加点
                    var newData = new PointerEventData(EventSystem.current);
                    newData.position = eventData.position;
                    newData.pointerId = pointerId;
                    pointerDict.Add(pointerId, newData);

                    //模拟PointerDown事件

                    pointerId++;
                }

                //选中点
                selectId = -1;
                foreach (var pointer in pointerDict.Values)
                {
                    if (Vector2.Distance(eventData.position, pointer.position) < touchIconSize * 0.5f)
                    {
                        selectId = pointer.pointerId;
                        break;
                    }
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (pointerDict.ContainsKey(selectId))
                {
                    //拖拽点,模拟Drag事件
                    var pointer =pointerDict[selectId];
                    pointer.position = eventData.position;
                }
            }
        }
    }
}