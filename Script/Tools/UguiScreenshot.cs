using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 截图
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
        /// 截取UI元素
        /// </summary>
        /// <param name="tex">位图</param>
        /// <param name="target">渲染物体</param>
        public void CaptureGraphic(ref Texture2D tex,Graphic target)
        {
            if (target == null)
            {
                throw new Exception("目标为空.");
            }

            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            var targetRectTrans = target.rectTransform;
            var targetWidth = (int)target.rectTransform.rect.width;
            var targetHeight = (int)targetRectTrans.rect.height;
            var maxTextureSize = (float)SystemInfo.maxTextureSize;
            var texSizeRotio = Mathf.Min(
                                Mathf.Min(maxTextureSize / targetWidth, 1),
                                Mathf.Min(maxTextureSize / targetHeight, 1)
                                );
            var texWidth = (int)(targetWidth * texSizeRotio);
            var texHeight = (int)(targetHeight * texSizeRotio);
            var rootCanvas = target.canvas.rootCanvas;
            var rootScaler = rootCanvas.GetComponent<CanvasScaler>();

            m_Canvas.renderMode = RenderMode.ScreenSpaceCamera;
            m_Canvas.worldCamera = m_Camera;
            var dist = screenHeight * 0.5f / Mathf.Tan(m_Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            m_Canvas.planeDistance = dist;
            m_Camera.transform.localPosition = new Vector3(0, 0, -dist / m_Canvas.scaleFactor);
            m_Camera.farClipPlane = 10000;
            gameObject.SetActive(true);

            //缓存状态
            var targetParent = targetRectTrans.parent;
            var cachePos= targetRectTrans.position;
            var cacheScale = targetRectTrans.localScale;
            var cacheRotation = targetRectTrans.rotation;
            var cachePivot = targetRectTrans.pivot;
            var children = target.GetComponentsInChildren<Transform>(false);
            var layerDict = new Dictionary<GameObject, int>(children.Length);
            foreach (var child in children)
            {
                layerDict.Add(child.gameObject,child.gameObject.layer);
            }

            //置入渲染画布
            targetRectTrans.SetParent(transform);
            targetRectTrans.pivot = Vector2.one*0.5f;
            targetRectTrans.rotation = Quaternion.identity;
            targetRectTrans.localPosition = Vector3.zero;
            targetRectTrans.localScale = new Vector3(texSizeRotio, texSizeRotio, texSizeRotio);
            foreach (var child in children)
            {
                child.gameObject.layer = gameObject.layer;
            }

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
            foreach (var child in children)
            {
                child.gameObject.layer = layerDict[child.gameObject];
            }
            gameObject.SetActive(false);

            //拷贝图像
            var cacheRT = RenderTexture.active;
            RenderTexture.active = rt;
            if (tex == null)
                tex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);
            if (tex.width != texWidth || tex.height != texHeight)
                tex.Resize(texWidth, texHeight, TextureFormat.ARGB32, false);
            var sourceRect = new Rect(
                (screenWidth - texWidth) * 0.5f,
                (screenHeight - texHeight) * 0.5f,
                texWidth,
                texHeight
                );
            tex.ReadPixels(sourceRect, 0, 0);
            tex.Apply();
            RenderTexture.active = cacheRT;

            RenderTexture.ReleaseTemporary(rt);
        }
    }
}