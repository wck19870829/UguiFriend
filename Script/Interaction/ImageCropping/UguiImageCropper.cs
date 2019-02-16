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
        const int safeFrameMinWidthValue = 100;
        const int safeFrameMinHeightValue = 100;

        [SerializeField] protected RawImage srcImage;               //待处理的图
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
        [SerializeField] protected Button mattingStepButton;        //抠像步骤按钮
        [SerializeField] protected GameObject scaleStep;
        [SerializeField] protected GameObject rotationStep;
        [SerializeField] protected GameObject cropStep;
        [SerializeField] protected GameObject mattingStep;

        [Header("Scale Ratio")]
        [SerializeField] protected Button one2OneRatioButton;
        [SerializeField] protected Button four2ThreeRatioButton;
        [SerializeField] protected Button originalRatioButton;
        [SerializeField] protected Button three2TwoRatioButton;
        [SerializeField] protected Button sixteen2NineRatioButton;

        protected List<RawImage> bisectrixColumnList;
        protected List<RawImage> bisectrixRowList;
        protected List<EventTrigger> safeFrameDragList;
        protected Canvas m_Canvas;
        protected List<GameObject> stepList;

        public Action<Texture2D> OnConfirm;
        public Action OnCancel;

        protected UguiImageCropper()
        {
            safeFrameMinWidth = safeFrameMinWidthValue;
            safeFrameMinHeight = safeFrameMinHeightValue;
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
            for (var i = 0; i < 4; i++)
            {
                var dragButton = UguiTools.AddChild<RawImage>("DragButton", safeFrame.transform);
                dragButton.rectTransform.sizeDelta = new Vector2(60, 60);
                var dragButtonTigger = dragButton.gameObject.AddComponent<EventTrigger>();
                var dragEntry = new EventTrigger.Entry();
                dragEntry.eventID = EventTriggerType.Drag;
                dragEntry.callback.AddListener(OnSafeFrameDrag);
                dragButtonTigger.triggers.Add(dragEntry);
            }

            var srcTrigger = srcImage.gameObject.AddComponent<EventTrigger>();
            var srcEntry = new EventTrigger.Entry();
            srcEntry.eventID = EventTriggerType.Drag;
            srcEntry.callback.AddListener(OnSrcImageDrag);
            srcTrigger.triggers.Add(srcEntry);

            //创建安全框等分线
            for (var i = 0; i < bisectrixColumn; i++)
            {
                var line = UguiTools.AddChild<RawImage>("Bisectrix", safeFrame.transform);
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

            rotationSlider.minValue = 0;
            rotationSlider.maxValue = 360;
            scaleSlider.minValue = 0.01f;
            scaleSlider.maxValue = 1f;
            confirmButton.onClick.AddListener(OnConfirmButtonClick);
            cancelButton.onClick.AddListener(OnCancelButtonClick);
            scaleStepButton.onClick.AddListener(OnScaleStepButtonClick);
            rotationStepButton.onClick.AddListener(OnRotationStepButtonClick);
            mattingStepButton.onClick.AddListener(OnMattingStepButtonClick);
            cropStepButton.onClick.AddListener(OnCropStepButtonClick);
            scaleSlider.onValueChanged.AddListener(OnScaleChange);
            rotationSlider.onValueChanged.AddListener(OnRotationChange);

            stepList = new List<GameObject>()
            {
                scaleStep,
                rotationStep,
                cropStep,
                mattingStep
            };
            originalRatioButton.onClick.AddListener(OriginalButtonClick);
            one2OneRatioButton.onClick.AddListener(One2OneButtonClick);
            four2ThreeRatioButton.onClick.AddListener(Four2ThreeButtonClick);
            three2TwoRatioButton.onClick.AddListener(Three2TwoButtonClick);
            sixteen2NineRatioButton.onClick.AddListener(Sixteen2NineButtonClick);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            GotoStep(cropStep);
        }

        protected override void OnValidate()
        {
            LimitSafeFrame();
        }

        protected virtual void Update()
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

        protected virtual void OnMattingStepButtonClick()
        {
            GotoStep(mattingStep);
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

        protected virtual void OriginalButtonClick()
        {

        }

        protected virtual void One2OneButtonClick()
        {

        }

        protected virtual void Four2ThreeButtonClick()
        {

        }

        protected virtual void Three2TwoButtonClick()
        {

        }

        protected virtual void Sixteen2NineButtonClick()
        {

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
            if (OnConfirm != null)
            {
                OnConfirm.Invoke(GetDestImage());
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
            var rectTransform = transform as RectTransform;
            if (safeFrame && maskImage)
            {
                //更新遮罩显示
                LimitSafeFrame();
                var safeFrameRect = (Rect)UguiTools.GetLocalRectIncludeChildren(safeFrame.rectTransform, rectTransform, true);
                var maskRect = (Rect)UguiTools.GetLocalRectIncludeChildren(maskImage.rectTransform, rectTransform, true);
                var xMin = (safeFrameRect.xMin - maskRect.xMin) / Mathf.Abs(maskRect.width);
                var xMax = (safeFrameRect.xMax - maskRect.xMin) / Mathf.Abs(maskRect.width);
                var yMin = (safeFrameRect.yMin - maskRect.yMin) / Mathf.Abs(maskRect.height);
                var yMax = (safeFrameRect.yMax - maskRect.yMin) / Mathf.Abs(maskRect.height);
                var safeFrameValue = new Vector4(xMin, xMax, yMin, yMax);
                maskImage.material.SetVector("_SafeFrame", safeFrameValue);
            }

            UpdateBisectrix();
        }

        protected virtual void LimitSafeFrame()
        {
            if (safeFrame)
            {
                var frameSize = safeFrame.rectTransform.rect.size;
                safeFrameMinWidth = Mathf.Max(safeFrameMinWidth, safeFrameMinWidthValue);
                safeFrameMinHeight = Mathf.Max(safeFrameMinHeight, safeFrameMinHeightValue);
                frameSize.x = Mathf.Max(frameSize.x, safeFrameMinWidth);
                frameSize.y = Mathf.Max(frameSize.y, safeFrameMinHeight);
                safeFrame.rectTransform.sizeDelta = frameSize;
            }
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
            if (pe != null)
            {
                Vector3 worldPoint;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(safeFrame.rectTransform, pe.position, m_Canvas.rootCanvas.worldCamera, out worldPoint))
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

            }

            return null;
        }
    }
}
