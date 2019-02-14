using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 图片编辑器
    /// </summary>
    public class UguiImageCropper : UIBehaviour
    {
        [SerializeField] protected RawImage srcImage;               //待处理的图
        [SerializeField] protected Image safeFrame;                 //安全框
        [SerializeField] protected RawImage maskImage;              //遮罩
        [SerializeField] protected Color maskColor;                 //遮罩颜色
        [SerializeField] protected bool showBisectrix;              //安全框显示等分线
        [SerializeField] protected int bisectrixColumn = 2;         //安全框等分线行列数量
        [SerializeField] protected int bisectrixRow = 2;
        [SerializeField] protected Color bisectrixColor;            //安全框等分线颜色

        protected List<RawImage> bisectrixColumnList;
        protected List<RawImage> bisectrixRowList;
        protected List<EventTrigger> safeFrameDragList;
        protected Canvas m_Canvas;

        protected UguiImageCropper()
        {
            showBisectrix = true;
            maskColor = new Color(0,0,0,0.7f);
            bisectrixColor = new Color(1,1,1,0.3f);
            bisectrixColumnList = new List<RawImage>();
            bisectrixRowList = new List<RawImage>();
            safeFrameDragList = new List<EventTrigger>();
        }

        protected override void Awake()
        {
            base.Awake();

            safeFrame.type = Image.Type.Sliced;
            safeFrame.raycastTarget = false;
            for (var i=0;i<4;i++)
            {
                var dragButton = UguiTools.AddChild<RawImage>("DragButton",safeFrame.transform);
                dragButton.rectTransform.sizeDelta = new Vector2(60,60);
                var dragButtonTigger = dragButton.gameObject.AddComponent<EventTrigger>();
                var dragEntry = new EventTrigger.Entry();
                dragEntry.eventID = EventTriggerType.Drag;
                dragEntry.callback.AddListener(OnSafeFrameDrag);
                dragButtonTigger.triggers.Add(dragEntry);
            }

            var srcTrigger=srcImage.gameObject.AddComponent<EventTrigger>();
            var srcEntry = new EventTrigger.Entry();
            srcEntry.eventID = EventTriggerType.Drag;
            srcEntry.callback.AddListener(OnSrcImageDrag);
            srcTrigger.triggers.Add(srcEntry);

            //创建安全框等分线
            for (var i=0;i< bisectrixColumn; i++)
            {
                var line = UguiTools.AddChild<RawImage>("Bisectrix",safeFrame.transform);
                line.color = bisectrixColor;
                line.raycastTarget = false;
                bisectrixColumnList.Add(line);
            }
            for (var i = 0; i < bisectrixRow; i++)
            {
                var line = UguiTools.AddChild<RawImage>("Bisectrix", safeFrame.transform);
                line.color = bisectrixColor;
                line.raycastTarget = false;
                bisectrixRowList.Add(line);
            }

            m_Canvas = GetComponentInParent<Canvas>();
        }

        protected virtual void Update()
        {
            RefreshDisplay();
        }

        protected virtual void RefreshDisplay()
        {
            var rectTransform = transform as RectTransform;
            if (safeFrame && maskImage)
            {
                //更新遮罩显示
                var safeFrameRect = (Rect)UguiTools.GetLocalRectIncludeChildren(safeFrame.rectTransform, rectTransform, true);
                var maskRect = (Rect)UguiTools.GetLocalRectIncludeChildren(maskImage.rectTransform, rectTransform, true);
                var xMin=(safeFrameRect.xMin - maskRect.xMin) / Mathf.Abs(maskRect.width);
                var xMax = (safeFrameRect.xMax - maskRect.xMin) / Mathf.Abs(maskRect.width);
                var yMin = (safeFrameRect.yMin - maskRect.yMin) / Mathf.Abs(maskRect.height);
                var yMax = (safeFrameRect.yMax - maskRect.yMin) / Mathf.Abs(maskRect.height);
                var safeFrameValue = new Vector4(xMin,xMax,yMin,yMax);
                maskImage.material.SetVector("_SafeFrame", safeFrameValue);
            }

            UpdateBisectrix();
        }

        /// <summary>
        /// 更新等分线
        /// </summary>
        protected virtual void UpdateBisectrix()
        {
            if (safeFrame)
            {
                for (var i = 0; i < bisectrixColumn; i++)
                {
                    var line = bisectrixColumnList[i];
                    line.rectTransform.sizeDelta = new Vector2(safeFrame.rectTransform.rect.width, 1);
                }
                for (var i = 0; i < bisectrixRow; i++)
                {
                    var line = bisectrixRowList[i];
                    line.rectTransform.sizeDelta = new Vector2(1, safeFrame.rectTransform.rect.height);
                }
            }
        }

        protected virtual void OnSrcImageDrag(BaseEventData eventData)
        {
            PointerEventData pe = eventData as PointerEventData;
            if (pe != null)
            {
                Vector3 worldPoint;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(safeFrame.rectTransform, pe.position, m_Canvas.rootCanvas.worldCamera, out worldPoint))
                {
                    pe.pointerDrag.transform.position = worldPoint;
                }
            }
        }

        protected virtual void OnSafeFrameDrag(BaseEventData eventData)
        {
            PointerEventData pe = eventData as PointerEventData;
            if (pe!=null)
            {
                Vector3 worldPoint;
                if(RectTransformUtility.ScreenPointToWorldPointInRectangle(safeFrame.rectTransform, pe.position, m_Canvas.rootCanvas.worldCamera, out worldPoint))
                {
                    pe.pointerDrag.transform.position = worldPoint;
                }
            }
        }

        /// <summary>
        /// 设置待处理图片
        /// </summary>
        /// <param name="tex"></param>
        public virtual void SetSrcImage(Texture2D tex)
        {
            if (srcImage)
            {
                srcImage.texture = tex;
            }
        }

        /// <summary>
        /// 设置旋转
        /// </summary>
        /// <param name="angle"></param>
        public virtual void SetSrcRotation(float angle)
        {
            if (srcImage)
            {
                srcImage.transform.localEulerAngles = new Vector3(0,0,angle);
            }
        }

        /// <summary>
        /// 设置缩放
        /// </summary>
        /// <param name="scale"></param>
        public virtual void SetSrcScale(Vector2 scale)
        {
            if (srcImage)
            {
                srcImage.transform.localScale = new Vector3(scale.x,scale.y,1);
            }
        }

        /// <summary>
        /// 设置位移
        /// </summary>
        /// <param name="position"></param>
        public virtual void SetSrcPosition(Vector2 position)
        {
            if (srcImage)
            {
                srcImage.transform.localPosition = position;
            }
        }

        /// <summary>
        /// 获取最终图像
        /// </summary>
        /// <returns></returns>
        public virtual Texture2D GetDestImage()
        {
            if (srcImage&&srcImage.texture)
            {

            }

            return null;
        }
    }
}
