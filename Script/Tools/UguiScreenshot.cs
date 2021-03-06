﻿using UnityEngine;
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
        static public Texture2D tempTex;

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
        /// <param name="resolutionRatio">缩放倍率</param>
        public void Capture(ref Texture2D tex,RectTransform target,float resolutionRatio=1)
        {
            if (target == null)
            {
                throw new Exception("目标为空.");
            }

            resolutionRatio = Mathf.Clamp(resolutionRatio,0.1f,5f);
            var screenWidth = (int)(Screen.width*resolutionRatio);
            var screenHeight = (int)(Screen.height*resolutionRatio);
            var targetRectTrans = target;
            var targetWidth = (int)targetRectTrans.rect.width;
            var targetHeight = (int)targetRectTrans.rect.height;
            var maxTextureSize = (float)SystemInfo.maxTextureSize;
            var texSizeRotio = Mathf.Min(
                                Mathf.Min(maxTextureSize / targetWidth,(float)screenWidth/ targetWidth, resolutionRatio),
                                Mathf.Min(maxTextureSize / targetHeight,(float)screenHeight/targetHeight , resolutionRatio)
                                );
            var texWidth = (int)(targetWidth * texSizeRotio);
            var texHeight = (int)(targetHeight * texSizeRotio);
            var rootCanvas = target.GetComponentInParent<Canvas>().rootCanvas;
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
            var cacheSiblingIndex = targetRectTrans.GetSiblingIndex();
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
            targetRectTrans.SetSiblingIndex(cacheSiblingIndex);
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
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
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