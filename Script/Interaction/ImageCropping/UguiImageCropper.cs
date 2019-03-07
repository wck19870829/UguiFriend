using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 图片编辑器
    /// </summary>
    public class UguiImageCropper : UIBehaviour
    {
        protected const int safeFrameDragWidth = 40;                        //拖拽框宽度
        protected const int defaultSafeFrameMinWidth = 100;                 //默认最小宽高
        protected const int defaultSafeFrameMinHeight = 100;

        public RawImage target;

        [SerializeField] protected RectTransform imageEditorArea;   //图片编辑区域
        [SerializeField] protected RectTransform srcImageContent;   //待处理图容器
        [SerializeField] protected RawImage srcImage;
        [SerializeField] protected Image safeFrame;                 //安全框
        [SerializeField] protected RawImage maskImage;              //遮罩
        [SerializeField] protected Slider scaleSlider;              //缩放滑块
        [SerializeField] protected Slider rotationSlider;           //旋转滑块
        [SerializeField] protected Button confirmButton;            //确认按钮
        [SerializeField] protected Button cancelButton;             //取消按钮
        [SerializeField] protected Color maskColor;                 //遮罩颜色
        [SerializeField] protected bool showBisectrix;              //安全框显示等分线
        [SerializeField] protected int bisectrixColumn = 2;         //安全框等分线行列数量
        [SerializeField] protected int bisectrixRow = 2;
        [SerializeField] protected Color bisectrixColor;            //安全框等分线颜色
        [SerializeField] protected int safeFrameMinWidth;                       //最小宽高
        [SerializeField] protected int safeFrameMinHeight;

        [Header("Step")]
        [SerializeField] protected Button scaleStepButton;          //调整缩放步骤按钮
        [SerializeField] protected Button rotationStepButton;       //调整旋转步骤按钮
        [SerializeField] protected Button cropStepButton;           //调整裁切步骤按钮
        [SerializeField] protected GameObject scaleStep;
        [SerializeField] protected GameObject rotationStep;
        [SerializeField] protected GameObject cropStep;

        [Header("Crop Ratio")]
        [SerializeField] protected Button originalCropRatioButton;      //比例重置按钮
        [SerializeField] protected Button cropRatioButton1_1;
        [SerializeField] protected Button cropRatioButton4_3;
        [SerializeField] protected Button cropRatioButton3_2;
        [SerializeField] protected Button cropRatioButton16_9;

        protected Texture2D screenShot;
        protected List<RawImage> bisectrixColumnList;
        protected List<RawImage> bisectrixRowList;
        protected List<EventTrigger> safeFrameDragList;
        protected Canvas m_Canvas;
        protected List<GameObject> stepList;
        protected Vector2 srcImageOffset;

        public Action<Texture2D> OnConfirm;
        public Action OnCancel;

        protected UguiImageCropper()
        {
            safeFrameMinWidth = defaultSafeFrameMinWidth;
            safeFrameMinHeight = defaultSafeFrameMinHeight;
            showBisectrix = true;
            maskColor = new Color(0, 0, 0, 0.7f);
            bisectrixColor = new Color(1, 1, 1, 0.3f);
            bisectrixColumnList = new List<RawImage>();
            bisectrixRowList = new List<RawImage>();
            safeFrameDragList = new List<EventTrigger>();
        }

        protected override void Awake()
        {
            base.Awake();

            safeFrame.type = Image.Type.Sliced;
            safeFrame.raycastTarget = false;

            //创建拖拽区域
            var dragButtonList = new List<UguiDragResize>();
            for (var i = 0; i < 8; i++)
            {
                var dragButtonImage = UguiTools.AddChild<RawImage>("", safeFrame.transform);
                var dragButton = dragButtonImage.gameObject.AddComponent<UguiDragResize>();
                dragButton.Graphic.color = Color.clear;
                dragButton.Target = safeFrame.rectTransform;
                dragButton.RectTransform.sizeDelta = new Vector2(safeFrameDragWidth, safeFrameDragWidth);
                dragButton.OnResize += OnResizeHandle;
                dragButtonList.Add(dragButton);
            }
            var dragButtonTop= dragButtonList[0];
            var dragButtonBottom = dragButtonList[1];
            var dragButtonLeft = dragButtonList[2];
            var dragButtonRight = dragButtonList[3];
            var dragButtonTopLeft = dragButtonList[4];
            var dragButtonTopRight = dragButtonList[5];
            var dragButtonBottomLeft = dragButtonList[6];
            var dragButtonBottomRight = dragButtonList[7];
            dragButtonTop.Pivot = UguiPivot.Top;
            dragButtonBottom.Pivot = UguiPivot.Bottom;
            dragButtonLeft.Pivot = UguiPivot.Left;
            dragButtonRight.Pivot = UguiPivot.Right;
            dragButtonTopLeft.Pivot = UguiPivot.TopLeft;
            dragButtonTopRight.Pivot = UguiPivot.TopRight;
            dragButtonBottomLeft.Pivot = UguiPivot.BottomLeft;
            dragButtonBottomRight.Pivot = UguiPivot.BottomRight;
            UguiTools.SetAnchor(dragButtonTop.RectTransform, AnchorPresets.HorStretchTop);
            UguiTools.SetAnchor(dragButtonBottom.RectTransform, AnchorPresets.HorStretchBottom);
            UguiTools.SetAnchor(dragButtonLeft.RectTransform, AnchorPresets.VertStretchLeft);
            UguiTools.SetAnchor(dragButtonRight.RectTransform, AnchorPresets.VertStretchRight);
            UguiTools.SetAnchor(dragButtonTopLeft.RectTransform, AnchorPresets.TopLeft);
            UguiTools.SetAnchor(dragButtonTopRight.RectTransform, AnchorPresets.TopRight);
            UguiTools.SetAnchor(dragButtonBottomLeft.RectTransform, AnchorPresets.BottomLeft);
            UguiTools.SetAnchor(dragButtonBottomRight.RectTransform, AnchorPresets.BottomRight);
            foreach (var dragButton in dragButtonList)
            {
                dragButton.name = "Drag_"+dragButton.Pivot;
            }

            srcImageContent.SetParent(imageEditorArea);
            maskImage.rectTransform.SetParent(imageEditorArea);
            safeFrame.rectTransform.SetParent(imageEditorArea);
            UguiTools.SetAnchor(maskImage.rectTransform, AnchorPresets.StretchAll);

            //限制安全框
            UguiTools.SetAnchor(safeFrame.rectTransform, AnchorPresets.StretchAll);

            if(!srcImage)
                srcImage = UguiTools.AddChild<RawImage>("SrcImage",srcImageContent);
            srcImage.transform.SetParent(srcImageContent,true);
            var srcTrigger = srcImage.gameObject.AddComponent<EventTrigger>();
            var srcEntry = new EventTrigger.Entry();
            srcEntry.eventID = EventTriggerType.Drag;
            srcEntry.callback.AddListener(OnSrcImageDrag);
            srcTrigger.triggers.Add(srcEntry);
            var srcBeginDragEntry = new EventTrigger.Entry();
            srcBeginDragEntry.eventID = EventTriggerType.BeginDrag;
            srcBeginDragEntry.callback.AddListener(OnSrcImageBeginDrag);
            srcTrigger.triggers.Add(srcBeginDragEntry);

            //创建安全框等分线
            for (var i = 0; i < bisectrixColumn; i++)
            {
                var line = UguiTools.AddChild<RawImage>("Bisectrix", safeFrame.transform);
                line.color = bisectrixColor;
                line.raycastTarget = false;
                line.rectTransform.sizeDelta = Vector2.one;
                UguiTools.SetAnchor(line.rectTransform, AnchorPresets.HorStretchBottom);
                bisectrixColumnList.Add(line);
            }
            for (var i = 0; i < bisectrixRow; i++)
            {
                var line = UguiTools.AddChild<RawImage>("Bisectrix", safeFrame.transform);
                line.color = bisectrixColor;
                line.raycastTarget = false;
                line.rectTransform.sizeDelta = Vector2.one;
                UguiTools.SetAnchor(line.rectTransform, AnchorPresets.VertStretchLeft);
                bisectrixRowList.Add(line);
            }

            m_Canvas = GetComponentInParent<Canvas>();

            rotationSlider.minValue = 0;
            rotationSlider.maxValue = 360;
            scaleSlider.minValue = 0.01f;
            scaleSlider.maxValue = 1f;
            confirmButton.onClick.AddListener(OnConfirmButtonClick);
            cancelButton.onClick.AddListener(OnCancelButtonClick);
            scaleStepButton.onClick.AddListener(OnScaleStepButtonClick);
            rotationStepButton.onClick.AddListener(OnRotationStepButtonClick);
            cropStepButton.onClick.AddListener(OnCropStepButtonClick);
            scaleSlider.onValueChanged.AddListener(OnScaleChange);
            rotationSlider.onValueChanged.AddListener(OnRotationChange);

            stepList = new List<GameObject>()
            {
                scaleStep,
                rotationStep,
                cropStep
            };
            originalCropRatioButton.onClick.AddListener(OriginalCropRatioButtonClick);
            cropRatioButton1_1.onClick.AddListener(CropRatioButton1_1Click);
            cropRatioButton4_3.onClick.AddListener(CropRatioButton4_3Click);
            cropRatioButton3_2.onClick.AddListener(CropRatioButton3_2Click);
            cropRatioButton16_9.onClick.AddListener(CropRatioButton16_9Click);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            GotoStep(cropStep);
        }

        protected override void OnValidate()
        {

        }

        protected virtual void LateUpdate()
        {
            RefreshDisplay();
        }

        protected virtual void OnScaleStepButtonClick()
        {
            GotoStep(scaleStep);
        }

        protected virtual void OnRotationStepButtonClick()
        {
            GotoStep(rotationStep);
        }

        protected virtual void OnCropStepButtonClick()
        {
            GotoStep(cropStep);
        }

        /// <summary>
        /// 跳转到步骤
        /// </summary>
        /// <param name="step"></param>
        public virtual void GotoStep(GameObject step)
        {
            foreach (var item in stepList)
            {
                item.SetActive(false);
            }
            step.SetActive(true);
        }

        /// <summary>
        /// 设置安全框尺寸
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public virtual void SetSafeFrameSize(int width,int height)
        {

        }

        protected virtual void OriginalCropRatioButtonClick()
        {
            if (srcImage && srcImage.texture)
            {

            }
        }

        protected virtual void CropRatioButton1_1Click()
        {

        }

        protected virtual void CropRatioButton4_3Click()
        {

        }

        protected virtual void CropRatioButton3_2Click()
        {

        }

        protected virtual void CropRatioButton16_9Click()
        {

        }

        protected virtual void OnResizeHandle(UguiDragResize dragButton)
        {
            LimitSafeFrame();
        }

        protected virtual void SetSafeFrame()
        {
            if (safeFrame)
            {
                UguiTools.SetAnchor(safeFrame.rectTransform, AnchorPresets.MiddleCenter);
                safeFrame.rectTransform.localPosition = Vector3.zero;
            }
        }

        protected virtual void OnScaleChange(float value)
        {
            if (srcImage)
            {
                srcImage.transform.localScale = new Vector3(value,value,1);
            }
        }

        protected virtual void OnRotationChange(float value)
        {
            if (srcImage)
            {
                srcImage.transform.localEulerAngles = new Vector3(0,0,value);
            }
        }

        protected virtual void OnConfirmButtonClick()
        {
            var destImage = GetDestImage();
            target.texture = destImage;
            if (OnConfirm != null)
            {
                OnConfirm.Invoke(destImage);
            }
        }

        protected virtual void OnCancelButtonClick()
        {
            if (OnCancel != null)
            {
                OnCancel.Invoke();
            }
        }

        protected virtual void RefreshDisplay()
        {
            RefreshMask();
            RefreshBisectrix();
            LimitSafeFrame();
        }

        /// <summary>
        /// 更新遮罩显示
        /// </summary>
        protected virtual void RefreshMask()
        {
            if (safeFrame&& maskImage&&imageEditorArea)
            {
                var safeFrameRect = (Rect)UguiTools.GetLocalRect(imageEditorArea, safeFrame.rectTransform);
                var maskRect = (Rect)UguiTools.GetLocalRect(imageEditorArea, maskImage.rectTransform);
                var xMin = (safeFrameRect.xMin - maskRect.xMin) / Mathf.Abs(maskRect.width);
                var xMax = (safeFrameRect.xMax - maskRect.xMin) / Mathf.Abs(maskRect.width);
                var yMin = (safeFrameRect.yMin - maskRect.yMin) / Mathf.Abs(maskRect.height);
                var yMax = (safeFrameRect.yMax - maskRect.yMin) / Mathf.Abs(maskRect.height);
                var safeFrameValue = new Vector4(xMin, xMax, yMin, yMax);
                maskImage.material.SetVector("_SafeFrame", safeFrameValue);
            }
        }

        protected virtual void LimitSafeFrame()
        {
            if (safeFrame)
            {
                UguiMathf.LimitRecrTransform(
                    safeFrame.rectTransform, 
                    safeFrameDragWidth,
                    safeFrameDragWidth,
                    safeFrameDragWidth, 
                    safeFrameDragWidth);
            }
        }

        /// <summary>
        /// 更新等分线
        /// </summary>
        protected virtual void RefreshBisectrix()
        {
            if (safeFrame)
            {
                var cornerOffset = 2;
                var rect = safeFrame.rectTransform.rect;
                var offsetX = rect.width / (bisectrixColumn + 1);
                var offsetY = rect.height / (bisectrixRow + 1);
                for (var i = 0; i < bisectrixColumn; i++)
                {
                    var line = bisectrixColumnList[i];
                    line.rectTransform.anchoredPosition = new Vector3(0, (i + 1) * offsetY);
                    line.rectTransform.offsetMin = new Vector2(cornerOffset, line.rectTransform.offsetMin.y);
                    line.rectTransform.offsetMax = new Vector2(-cornerOffset, line.rectTransform.offsetMax.y);
                }
                for (var i = 0; i < bisectrixRow; i++)
                {
                    var line = bisectrixRowList[i];
                    line.rectTransform.anchoredPosition = new Vector3((i+1) * offsetX, 0);
                    line.rectTransform.offsetMin = new Vector2(line.rectTransform.offsetMin.x, cornerOffset);
                    line.rectTransform.offsetMax = new Vector2(line.rectTransform.offsetMax.x,-cornerOffset);
                }
            }
        }

        protected virtual void OnSrcImageBeginDrag(BaseEventData eventData)
        {
            PointerEventData pe = eventData as PointerEventData;
            if (pe != null)
            {
                var screenPoint=RectTransformUtility.WorldToScreenPoint(m_Canvas.rootCanvas.worldCamera, srcImage.rectTransform.position);
                srcImageOffset = screenPoint- pe.position;
            }
        }

        protected virtual void OnSrcImageDrag(BaseEventData eventData)
        {
            PointerEventData pe = eventData as PointerEventData;
            if (pe != null)
            {
                Vector3 worldPoint;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(srcImage.rectTransform, pe.position + srcImageOffset, m_Canvas.rootCanvas.worldCamera, out worldPoint))
                {
                    srcImage.transform.position = worldPoint;
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
                srcImage.transform.localEulerAngles = new Vector3(0, 0, angle);
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
                srcImage.transform.localScale = new Vector3(scale.x, scale.y, 1);
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
            if (srcImage && srcImage.texture)
            {
                var cachePos = srcImage.transform.position;
                srcImageContent.position = safeFrame.transform.position;
                srcImageContent.sizeDelta = safeFrame.rectTransform.sizeDelta;
                srcImage.transform.position=cachePos;
                UguiScreenshot.Instance.Capture(ref screenShot, srcImageContent);

                return screenShot;
            }

            return null;
        }
    }
}
