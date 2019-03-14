using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    [DisallowMultipleComponent]
    public sealed class UguiMultPointSimulationInputModule : StandaloneInputModule
    {
        static int defaultTouchIconSize = 20;
        static int touchIconSize;
        static int pointerId;
        static int selectId;

        Dictionary<int, PointerEventData> pointerDict;
        Texture2D touchTex;
        Texture2D currentTouchTex;

        protected override void Awake()
        {
            base.Awake();

            touchTex = Resources.Load<Texture2D>("UguiFriend/Texture/TouchPoint");
            currentTouchTex = Resources.Load<Texture2D>("UguiFriend/Texture/CurrentTouchPoint");
            pointerDict = new Dictionary<int, PointerEventData>();
        }

        private void OnGUI()
        {
            touchIconSize = (int)(defaultTouchIconSize * Mathf.Max(Screen.width / 1024f, Screen.height / 768f));

            //渲染点
            foreach (var pointer in pointerDict.Values)
            {
                var pos = new Rect(
                            pointer.position.x - touchIconSize * 0.5f,
                            Screen.height - pointer.position.y - touchIconSize * 0.5f,
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

        public override void Process()
        {
            var mouseData = GetMousePointerEventData(0);
            var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;

            ProcessMousePressState(leftButtonData);
            ProcessMove(leftButtonData.buttonData);
            ProcessDrag(leftButtonData.buttonData);

            UpdateVirtualPoints(leftButtonData.buttonData);
        }

        /// <summary>
        /// Process the current mouse press.
        /// </summary>
        private void ProcessMousePressState(MouseButtonEventData data)
        {
            var pointerEvent = data.buttonData;
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if (data.PressedThisFrame())
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                // didnt find a press handler... search for a click handler
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // Debug.Log("Pressed: " + newPressed);

                float time = Time.unscaledTime;

                if (newPressed == pointerEvent.lastPress)
                {
                    var diffTime = time - pointerEvent.clickTime;
                    if (diffTime < 0.3f)
                        ++pointerEvent.clickCount;
                    else
                        pointerEvent.clickCount = 1;

                    pointerEvent.clickTime = time;
                }
                else
                {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
            }

            // PointerUp notification
            if (data.ReleasedThisFrame())
            {
                // Debug.Log("Executing pressup on: " + pointer.pointerPress);
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick and Drop events
                if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
                {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                }
                else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                // redo pointer enter / exit to refresh state
                // so that if we moused over somethign that ignored it before
                // due to having pressed on something else
                // it now gets it.
                if (currentOverGo != pointerEvent.pointerEnter)
                {
                    HandlePointerExitAndEnter(pointerEvent, null);
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                }
            }
        }

        private void UpdateVirtualPoints(PointerEventData eventData)
        {
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
                    eventData.pointerId = pointerId;
                    eventData.dragging = false;
                    eventData.pointerDrag = null;

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
                    var pointer = pointerDict[selectId];
                    pointer.position = eventData.position;
                }
            }
        }
    }
}