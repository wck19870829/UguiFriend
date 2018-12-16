using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 旋转物体
    /// </summary>
    public class UguiRotateObject : UIBehaviour,IPointerDownHandler,IPointerUpHandler
    {
        protected List<PointerEventData> pointerDataList;
        protected List<Vector2> deltaList;
        protected List<Vector2> positionList;

        protected override void Awake()
        {
            base.Awake();
            pointerDataList = new List<PointerEventData>();
            deltaList = new List<Vector2>();
            positionList = new List<Vector2>();
        }

        protected virtual void Update()
        {
            deltaList.Clear();
            positionList.Clear();
            if (pointerDataList.Count>0)
            {
                if (pointerDataList.Count == 1)
                {
                    //一个点绕中心点旋转

                }
                else
                {
                    //多点绕多点中心点旋转
                    foreach (var pointer in pointerDataList)
                    {
                        positionList.Add(pointer.position);
                        deltaList.Add(pointer.delta);
                    }
                    var center = UguiMathf.GetCenter(positionList);
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pointerDataList.Remove(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pointerDataList.Add(eventData);
        }
    }
}