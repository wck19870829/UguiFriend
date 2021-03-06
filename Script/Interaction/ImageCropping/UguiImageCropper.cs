﻿using UnityEngine;
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
        protected const int safeFrameDragWidth = 100;                       //拖拽框宽度
        protected const int border = (int)(safeFrameDragWidth * 0.5f);      //到边框的距离
        protected const int defaultSafeFrameMinWidth = 100;                 //默认最小宽高
        protected const int defaultSafeFrameMinHeight = 100;
        protected const int defaultBisectrixWidth = 2;

        [SerializeField] protected RectTransform imageEditorArea;   //图片编辑区域
        [SerializeField] protected RectTransform srcImageContent;   //待处理图容器
        [SerializeField] protected RawImage srcImage;
        [SerializeField] protected Image safeFrame;                 //安全框
        [SerializeField] protected RawImage maskImage;              //遮罩
        [SerializeField] protected Button confirmButton;            //确认按钮
        [SerializeField] protected Button cancelButton;             //取消按钮
        [SerializeField] protected Color maskColor;                 //遮罩颜色
        [SerializeField] protected bool showBisectrix;              //安全框显示等分线
        [SerializeField] protected int bisectrixColumn = 2;         //安全框等分线行列数量
        [SerializeField] protected int bisectrixRow = 2;
        [SerializeField] protected Color bisectrixColor;            //安全框等分线颜色
        [SerializeField] protected int safeFrameMinWidth;           //最小宽高
        [SerializeField] protected int safeFrameMinHeight;
        [SerializeField] UguiPivotSet activeDragPivotSet;           //激活的拖拽区域

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

        [Header("Rotate")]
        [SerializeField] protected Button rotate90Button;
        [SerializeField] protected Button cancelRotateButton;
        [SerializeField] protected Text rotationText;
        [SerializeField] protected Slider rotationSlider;           //旋转滑块

        [Header("Scale")]
        [SerializeField] protected Text scaleText;
        [SerializeField] protected Slider scaleSlider;              //缩放滑块

        protected Texture2D screenShot;
        protected List<RawImage> bisectrixColumnList;
        protected List<RawImage> bisectrixRowList;
        protected List<EventTrigger> safeFrameDragList;
        protected Canvas m_Canvas;
        protected List<GameObject> stepList;
        protected Vector2 srcImageOffset;
        protected UguiTweenPosition srcImagePostionTweener;
        protected UguiTweenScale srcImageScaleTweener;
        protected List<UguiDragResize> dragButtonList;

        public Action<Texture2D> OnConfirm;             //确认按钮按下事件
        public Action OnCancel;                         //取消按钮按下事件

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
            dragButtonList = new List<UguiDragResize>();
            activeDragPivotSet = new UguiPivotSet
            {
                UguiPivot.Bottom,
                UguiPivot.BottomLeft,
                UguiPivot.BottomRight,
                UguiPivot.Left,
                UguiPivot.Right,
                UguiPivot.Top,
                UguiPivot.TopLeft,
                UguiPivot.TopRight
            };
        }

        protected override void Awake()
        {
            base.Awake();

            safeFrame.type = Image.Type.Sliced;
            safeFrame.raycastTarget = false;

            //创建拖拽区域
            CreateSafeFrameDragArea();

            srcImageContent.SetParent(imageEditorArea);
            maskImage.rectTransform.SetParent(imageEditorArea);
            safeFrame.rectTransform.SetParent(imageEditorArea);
            UguiMathf.SetAnchor(maskImage.rectTransform, AnchorPresets.StretchAll);
            UguiMathf.SetAnchor(safeFrame.rectTransform, AnchorPresets.StretchAll);

            //遮罩添加拖拽监听
            if(!srcImage)
                srcImage = UguiTools.AddChild<RawImage>("SrcImage",srcImageContent);
            srcImage.transform.SetParent(srcImageContent,true);
            srcImagePostionTweener = UguiTools.GetOrAddComponent<UguiTweenPosition>(srcImage.gameObject);
            srcImagePostionTweener.Space = Space.Self;
            srcImageScaleTweener = UguiTools.GetOrAddComponent<UguiTweenScale>(srcImage.gameObject);
            var maskDrag = UguiTools.GetOrAddComponent<UguiDragObject>(maskImage.gameObject);
            maskDrag.OnEndDragEvent += LimitSrcImage;
            maskDrag.Target = srcImage.transform;
            var maskRotate = UguiTools.GetOrAddComponent<UguiRotateObject>(maskImage.gameObject);
            maskRotate.Target = srcImage.transform;
            var maskZoom = UguiTools.GetOrAddComponent<UguiZoomObject>(maskImage.gameObject);
            maskZoom.Target = srcImage.transform;

            //创建安全框等分线
            for (var i = 0; i < bisectrixColumn; i++)
            {
                var line = UguiTools.AddChild<RawImage>("Bisectrix", safeFrame.transform);
                line.color = bisectrixColor;
                line.raycastTarget = false;
                line.rectTransform.sizeDelta = Vector2.one* defaultBisectrixWidth;
                UguiMathf.SetAnchor(line.rectTransform, AnchorPresets.HorStretchBottom);
                bisectrixColumnList.Add(line);
            }
            for (var i = 0; i < bisectrixRow; i++)
            {
                var line = UguiTools.AddChild<RawImage>("Bisectrix", safeFrame.transform);
                line.color = bisectrixColor;
                line.raycastTarget = false;
                line.rectTransform.sizeDelta = Vector2.one * defaultBisectrixWidth;
                UguiMathf.SetAnchor(line.rectTransform, AnchorPresets.VertStretchLeft);
                bisectrixRowList.Add(line);
            }

            m_Canvas = GetComponentInParent<Canvas>();

            confirmButton.onClick.AddListener(OnConfirmButtonClick);
            cancelButton.onClick.AddListener(OnCancelButtonClick);
            scaleStepButton.onClick.AddListener(OnScaleStepButtonClick);
            rotationStepButton.onClick.AddListener(OnRotationStepButtonClick);
            cropStepButton.onClick.AddListener(OnCropStepButtonClick);

            scaleSlider.minValue = 0.01f;
            scaleSlider.maxValue = 1f;
            scaleSlider.onValueChanged.AddListener(OnScaleChange);
            UguiTools.AddTriger(scaleSlider.gameObject, EventTriggerType.EndDrag, OnScaleSliderEndDrag);

            rotationSlider.minValue = 0;
            rotationSlider.maxValue = 360;
            rotationSlider.onValueChanged.AddListener(OnRotationChange);
            UguiTools.AddTriger(rotationSlider.gameObject, EventTriggerType.EndDrag, OnRotateSlideEndDrag);

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

            rotate90Button.onClick.AddListener(OnRotate90Click);
            cancelRotateButton.onClick.AddListener(OnCancelRotateClick);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            GotoStep(cropStep);
        }

        protected virtual void LateUpdate()
        {
            RefreshDisplay();
        }

        protected virtual void OnRotateSlideEndDrag(BaseEventData eventData)
        {
            LimitSrcImage();
        }

        protected virtual void OnScaleSliderEndDrag(BaseEventData eventData)
        {
            LimitSrcImage();
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
        /// 创建SafeFrame拖拽区域
        /// </summary>
        protected virtual void CreateSafeFrameDragArea()
        {
            for (var i = 0; i < 8; i++)
            {
                var dragButtonImage = UguiTools.AddChild<RawImage>("", safeFrame.transform);
                var dragButton = dragButtonImage.gameObject.AddComponent<UguiDragResize>();
                dragButton.Graphic.color = Color.clear;
                dragButton.Target = safeFrame.rectTransform;
                dragButton.RectTransform.sizeDelta = new Vector2(safeFrameDragWidth, safeFrameDragWidth);
                dragButton.OnResize += OnResizeHandle;
                dragButton.OnEndResize += OnEndResizeHandle;
                dragButtonList.Add(dragButton);
            }
            var dragButtonTop = dragButtonList[0];
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
            var moveIcon = Resources.Load<Texture2D>("UguiFriend/Texture/Move");
            var neResizeIcon = Resources.Load<Texture2D>("UguiFriend/Texture/NeResize");
            var seResizeIcon = Resources.Load<Texture2D>("UguiFriend/Texture/SeResize");
            var sResizeIcon = Resources.Load<Texture2D>("UguiFriend/Texture/SResize");
            var wResizeIcon = Resources.Load<Texture2D>("UguiFriend/Texture/WResize");
            dragButtonTop.CursorIcon = sResizeIcon;
            dragButtonBottom.CursorIcon = sResizeIcon;
            dragButtonLeft.CursorIcon = wResizeIcon;
            dragButtonRight.CursorIcon = wResizeIcon;
            dragButtonTopLeft.CursorIcon = seResizeIcon;
            dragButtonBottomRight.CursorIcon = seResizeIcon;
            dragButtonTopRight.CursorIcon = neResizeIcon;
            dragButtonBottomLeft.CursorIcon = neResizeIcon;
            UguiMathf.SetAnchor(dragButtonTop.RectTransform, AnchorPresets.HorStretchTop);
            UguiMathf.SetAnchor(dragButtonBottom.RectTransform, AnchorPresets.HorStretchBottom);
            UguiMathf.SetAnchor(dragButtonLeft.RectTransform, AnchorPresets.VertStretchLeft);
            UguiMathf.SetAnchor(dragButtonRight.RectTransform, AnchorPresets.VertStretchRight);
            UguiMathf.SetAnchor(dragButtonTopLeft.RectTransform, AnchorPresets.TopLeft);
            UguiMathf.SetAnchor(dragButtonTopRight.RectTransform, AnchorPresets.TopRight);
            UguiMathf.SetAnchor(dragButtonBottomLeft.RectTransform, AnchorPresets.BottomLeft);
            UguiMathf.SetAnchor(dragButtonBottomRight.RectTransform, AnchorPresets.BottomRight);
            foreach (var dragButton in dragButtonList)
            {
                dragButton.name = "Drag_" + dragButton.Pivot;
                var dragButtonActive = activeDragPivotSet.Contains(dragButton.Pivot);
                dragButton.gameObject.SetActive(dragButtonActive);
            }
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

        protected virtual void OriginalCropRatioButtonClick()
        {
            if (srcImage && srcImage.texture)
            {
                var aspectRatio = (float)srcImage.texture.width / (float)srcImage.texture.height;
                UguiMathf.LimitRectTransform(
                            safeFrame.rectTransform,
                            ScaleMode.ScaleToFit,
                            aspectRatio,
                            border,
                            border,
                            border,
                            border);
                LimitSrcImage();
            }
        }

        protected virtual void CropRatioButton1_1Click()
        {
            if (srcImage && srcImage.texture)
            {
                var aspectRatio = 1f / 1f;
                UguiMathf.LimitRectTransform(
                            safeFrame.rectTransform,
                            ScaleMode.ScaleToFit,
                            aspectRatio,
                            border,
                            border,
                            border,
                            border);
                LimitSrcImage();
            }
        }

        protected virtual void CropRatioButton4_3Click()
        {
            if (srcImage && srcImage.texture)
            {
                var aspectRatio = 4f / 3f;
                UguiMathf.LimitRectTransform(
                            safeFrame.rectTransform,
                            ScaleMode.ScaleToFit,
                            aspectRatio,
                            border,
                            border,
                            border,
                            border);
                LimitSrcImage();
            }
        }

        protected virtual void CropRatioButton3_2Click()
        {
            if (srcImage && srcImage.texture)
            {
                var aspectRatio = 3f / 2f;
                UguiMathf.LimitRectTransform(
                            safeFrame.rectTransform,
                            ScaleMode.ScaleToFit,
                            aspectRatio,
                            border,
                            border,
                            border,
                            border);
                LimitSrcImage();
            }
        }

        protected virtual void CropRatioButton16_9Click()
        {
            if (srcImage && srcImage.texture)
            {
                var aspectRatio = 16f / 9f;
                UguiMathf.LimitRectTransform(
                            safeFrame.rectTransform,
                            ScaleMode.ScaleToFit,
                            aspectRatio,
                            border,
                            border,
                            border,
                            border);
                LimitSrcImage();
            }
        }

        protected virtual void OnRotate90Click()
        {
            if (srcImage)
            {
                srcImage.transform.localEulerAngles += new Vector3(0, 0, -90);
                LimitSrcImage();
            }
        }

        protected virtual void OnCancelRotateClick()
        {
            if (srcImage)
            {
                srcImage.transform.localEulerAngles = Vector3.zero;
                LimitSrcImage();
            }
        }

        protected virtual void OnResizeHandle(UguiDragResize dragButton)
        {
            LimitSafeFrame();
        }

        protected virtual void OnEndResizeHandle(UguiDragResize dragButton)
        {
            LimitSrcImage();
        }

        protected virtual void SetSafeFrame()
        {
            if (safeFrame)
            {
                UguiMathf.SetAnchor(safeFrame.rectTransform, AnchorPresets.MiddleCenter);
                safeFrame.rectTransform.localPosition = Vector3.zero;
            }
        }

        protected virtual void OnScaleChange(float value)
        {
            if (srcImage)
            {
                //var cam = UguiTools.GetValidCamera(m_Canvas);
                //var screenPos = RectTransformUtility.WorldToScreenPoint(cam,safeFrame.transform.position);
                //var ray = RectTransformUtility.ScreenPointToRay(cam, center);
                //var plane = new Plane(ray.direction.normalized, srcImage.transform.position);
                //float enter;
                //plane.Raycast(ray, out enter);
                //var worldPos = ray.GetPoint(enter);
                //var newScale = m_Target.localScale.x + scaleDelta;
                //UguiMathf.TransformScaleAround(m_Target, worldPos, new Vector3(newScale, newScale, newScale));
                ////srcImage.transform.localScale = new Vector3(value,value, value);
            }
        }

        protected virtual void OnRotationChange(float value)
        {
            if (srcImage)
            {
                //srcImage.transform.localEulerAngles = new Vector3(0,0,value);
            }
        }

        protected virtual void OnConfirmButtonClick()
        {
            var destImage = GetDestImage();
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
            RefreshScaleStep();
            RefreshRotateStep();
            RefreshCropStep();
        }

        protected virtual void RefreshScaleStep()
        {
            if (srcImage)
            {
                scaleSlider.value = srcImage.transform.localScale.x;
                scaleText.text = Mathf.RoundToInt(srcImage.transform.localScale.x*100)+"%";
            }
        }

        /// <summary>
        /// 设置拖拽按钮状态
        /// </summary>
        /// <param name="dragPivot"></param>
        /// <param name="active"></param>
        public virtual void SetDragState(UguiPivot dragPivot,bool active)
        {
            var dragButton=dragButtonList.Find(
                (x) => {
                    return x.Pivot == dragPivot;
                });
            if (!dragButton) return;

            dragButton.gameObject.SetActive(active);
        }

        protected virtual void RefreshRotateStep()
        {
            if (srcImage)
            {
                var angle = srcImage.transform.localEulerAngles.z%360;
                rotationSlider.value = angle;
                rotationText.text = UguiMathf.VectorSignedAngle(Vector2.up,UguiMathf.Rotation(Vector2.up,angle)).ToString("0.0") + "°";
            }
        }

        protected virtual void RefreshCropStep()
        {

        }

        /// <summary>
        /// 更新遮罩显示
        /// </summary>
        protected virtual void RefreshMask()
        {
            if (safeFrame&& maskImage&&imageEditorArea)
            {
                var safeFrameDist = UguiMathf.GetRectTransformEdgeDistance(safeFrame.rectTransform, maskImage.rectTransform);
                var maskRect = maskImage.rectTransform.rect;
                var safeFrameValue = new Vector4(
                                    safeFrameDist.y / maskRect.height,
                                    1-safeFrameDist.x / maskRect.height,
                                    safeFrameDist.z / maskRect.width,
                                    1 - safeFrameDist.w / maskRect.width);
                maskImage.material.SetVector("_SafeFrame", safeFrameValue);

                //强制刷新,待优化
                maskImage.enabled = false;
                maskImage.enabled = true;
            }
        }

        /// <summary>
        /// 限制图片
        /// </summary>
        protected virtual void LimitSrcImage()
        {
            var localPos = Vector3.zero;
            var localScale = Vector3.zero;
            UguiMathf.LimitRectTransformWithMoveAndScale(srcImage.rectTransform,safeFrame.rectTransform,ref localPos,ref localScale);
            srcImagePostionTweener.Play(srcImage.rectTransform.localPosition, localPos);
            srcImageScaleTweener.Play(srcImage.rectTransform.localScale, localScale);
        }

        /// <summary>
        /// 限制安全框
        /// </summary>
        protected virtual void LimitSafeFrame()
        {
            if (safeFrame)
            {
                UguiMathf.LimitRectTransform(
                    safeFrame.rectTransform,
                    border,
                    border,
                    border,
                    border);
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
