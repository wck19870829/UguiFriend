using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 截屏
    /// </summary>
    public sealed class UguiScreenshot : UguiSingleton<UguiScreenshot>,IUguiSingletonCreate<UguiScreenshot>
    {
        Camera m_Camera;
        Canvas m_Canvas;
        CanvasScaler m_CanvasScaler;

        public void OnSingletonCreate(UguiScreenshot instance)
        {
            var go = instance.gameObject;
            go.SetActive(false);

            m_Camera = UguiTools.AddChild<Camera>("Camera", go.transform);
            m_Camera.backgroundColor = Color.clear;
            m_Camera.clearFlags = CameraClearFlags.SolidColor;
            m_Canvas = go.AddComponent<Canvas>();
            m_CanvasScaler = go.AddComponent<CanvasScaler>();

            //设置渲染图层
            var unusedLayer = UguiTools.FindUnusedLayer();
            if (unusedLayer < 0) unusedLayer = 31;
            gameObject.layer = unusedLayer;
            m_Camera.cullingMask = 1 << unusedLayer;
        }

        /// <summary>
        /// 截图
        /// </summary>
        /// <param name="tex">位图</param>
        /// <param name="target">渲染物体</param>
        public void CaptureGraphic(ref Texture2D tex,Graphic target)
        {
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            var targetRectTrans = target.rectTransform;
            var targetWidth = (int)target.rectTransform.rect.width;
            var targetHeight = (int)targetRectTrans.rect.height;
            var rootCanvas = target.canvas.rootCanvas;
            var rootScaler = rootCanvas.GetComponent<CanvasScaler>();

            //UguiTools.CopyProps(rootScaler, m_CanvasScaler);
            m_Canvas.renderMode = RenderMode.ScreenSpaceCamera;
            m_Canvas.worldCamera = m_Camera;
            var dist = screenHeight * 0.5f / Mathf.Tan(m_Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            m_Canvas.planeDistance = dist;
            m_Camera.transform.localPosition = new Vector3(0, 0, -dist / rootCanvas.scaleFactor);
            m_Camera.farClipPlane = 10000;
            gameObject.SetActive(true);

            //缓存状态
            var targetParent = targetRectTrans.parent;
            var cachePos= targetRectTrans.position;
            var cacheScale = targetRectTrans.localScale;
            var cacheRotation = targetRectTrans.rotation;
            var cachePivot = targetRectTrans.pivot;

            //置入
            targetRectTrans.SetParent(transform);
            targetRectTrans.pivot = Vector2.one*0.5f;
            targetRectTrans.rotation = Quaternion.identity;
            targetRectTrans.localPosition = Vector3.zero;
            targetRectTrans.localScale = Vector3.one;

            var rt = RenderTexture.GetTemporary(screenWidth, screenHeight, 32, RenderTextureFormat.ARGB32);
            m_Camera.targetTexture = rt;
            m_Camera.RenderDontRestore();
            m_Camera.targetTexture = null;

            //还原状态
            targetRectTrans.SetParent(targetParent);
            targetRectTrans.pivot = cachePivot;
            targetRectTrans.position = cachePos;
            targetRectTrans.localScale = cacheScale;
            targetRectTrans.rotation = cacheRotation;
            gameObject.SetActive(false);

            ////拷贝图像
            var cacheRT = RenderTexture.active;
            RenderTexture.active = rt;
            var sourceRect = new Rect(
                            (screenWidth - targetWidth) * 0.5f,
                            (screenHeight - targetHeight) * 0.5f,
                            targetWidth,
                            targetHeight
                            );
            var maxTextureSize = (float)SystemInfo.maxTextureSize;//256f;// 
            var texSizeRotio = Mathf.Min(
                                Mathf.Min(maxTextureSize / targetWidth, 1),
                                Mathf.Min(maxTextureSize / targetHeight, 1)
                                );
            var texWidth = (int)(targetWidth * texSizeRotio);
            var texHeight = (int)(targetHeight * texSizeRotio);
            if (tex == null)
                tex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);
            if (tex.width != texWidth || tex.height != texHeight)
                tex.Resize(texWidth, texHeight, TextureFormat.ARGB32, false);
            tex.ReadPixels(sourceRect, 0, 0);
            tex.Apply();
            RenderTexture.active = cacheRT;

            RenderTexture.ReleaseTemporary(rt);

            #region 旧代码

            //var screenWidth = Screen.width;
            //var screenHeight = Screen.height;
            //var rootCanvas = target.canvas.rootCanvas;
            //if (rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay && rootCanvas.worldCamera == null)
            //{
            //    throw new Exception("请正确设置相机!");
            //}

            //var targetScreenRect=UguiTools.GetScreenRect(target);
            //var targetWidth = (int)targetScreenRect.width;
            //var targetHeight=(int)targetScreenRect.height;

            ////对齐到目标画布
            //var rootScaler = rootCanvas.GetComponent<CanvasScaler>();
            //UguiTools.CopyProps(rootCanvas,m_Canvas);
            //UguiTools.CopyProps(rootCanvas.transform, m_Canvas.transform);
            //m_Canvas.worldCamera = m_Camera;
            //if (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            //{
            //    m_Canvas.renderMode = RenderMode.ScreenSpaceCamera;
            //    var dist = screenHeight * 0.5f / Mathf.Tan(m_Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            //    m_Canvas.planeDistance = dist;
            //    m_Camera.transform.localPosition = new Vector3(0, 0, -dist/rootCanvas.scaleFactor);
            //    m_Camera.farClipPlane = 10000;
            //}
            //else
            //{
            //    UguiTools.CopyProps(rootCanvas.worldCamera, m_Camera);
            //}

            ////渲染准备
            //var unusedLayer = UguiTools.FindUnusedLayer();
            //if (unusedLayer < 0) unusedLayer = 31;
            //unusedLayer = 5;
            //gameObject.layer = unusedLayer;
            //gameObject.SetActive(true);
            //m_Camera.backgroundColor = Color.clear;
            //m_Camera.cullingMask = 1 << unusedLayer;

            ////置入
            //Debug.LogFormat("{0}  {1}", targetRectTrans.position, targetRectTrans.localScale);

            //var targetParent = targetRectTrans.parent;
            //targetRectTrans.SetParent(transform, true);

            //Debug.LogFormat("{0}  {1}", targetRectTrans.position, targetRectTrans.localScale);

            ////渲染
            //var rt = RenderTexture.GetTemporary(screenWidth, screenHeight, 32, RenderTextureFormat.ARGB32);
            //m_Camera.targetTexture = rt;
            //m_Camera.RenderDontRestore();
            //m_Camera.targetTexture = null;

            //Debug.LogFormat("{0}  {1}",targetRectTrans.position, targetRectTrans.localScale);

            //targetRectTrans.SetParent(targetParent, true);

            //Debug.LogFormat("{0}  {1}", targetRectTrans.position, targetRectTrans.localScale);

            //gameObject.SetActive(false);

            ////拷贝图像
            //var cacheRT = RenderTexture.active;
            //RenderTexture.active = rt;
            //var sourceRect = new Rect(
            //                targetScreenRect.x,
            //                screenHeight-targetScreenRect.yMax,
            //                targetScreenRect.width,
            //                targetScreenRect.height
            //                );
            //var maxTextureSize = (float)SystemInfo.maxTextureSize;//256f;// 
            //var texSizeRotio = Mathf.Min(
            //                    Mathf.Min(maxTextureSize /targetWidth, 1),
            //                    Mathf.Min(maxTextureSize /targetHeight, 1)
            //                    );
            //var texWidth = (int)(targetWidth* texSizeRotio);
            //var texHeight = (int)(targetHeight* texSizeRotio);
            //if (tex == null)
            //    tex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);
            //if (tex.width != texWidth || tex.height != texHeight)
            //    tex.Resize(texWidth, texHeight, TextureFormat.ARGB32, false);
            //tex.ReadPixels(sourceRect, 0, 0);
            //tex.Apply();
            //RenderTexture.active = cacheRT;

            //RenderTexture.ReleaseTemporary(rt);

            #endregion
        }
    }
}