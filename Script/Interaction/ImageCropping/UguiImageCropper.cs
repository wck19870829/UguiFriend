using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 图片编辑器
    /// </summary>
    public class UguiImageCropper : UIBehaviour
    {
        [SerializeField] protected RawImage srcImage;               //待处理的图
        [SerializeField] protected Image safeFrame;                 //安全框
        [SerializeField] protected RawImage maskImage;              //黑色遮罩
        [SerializeField] protected bool showBisectrix;              //安全框显示等分线

        protected UguiImageCropper()
        {
            showBisectrix = true;
        }

        protected override void Awake()
        {
            base.Awake();

            safeFrame.type = Image.Type.Sliced;
            safeFrame.raycastTarget = false;
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

                //更新等分线

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
