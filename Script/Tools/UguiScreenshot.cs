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
        }

        /// <summary>
        /// 截图
        /// </summary>
        /// <param name="tex">位图</param>
        /// <param name="target">渲染物体</param>
        public void Capture(ref Texture2D tex,Graphic target)
        {
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;

#if UNITY_EDITOR
            var screenSize=UnityEditor.UnityStats.screenRes.Split('x');
            screenWidth = int.Parse(screenSize[0]);
            screenHeight = int.Parse(screenSize[1]);
#endif
            var rootCanvas = target.canvas.rootCanvas;
            if (rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay && rootCanvas.worldCamera == null)
            {
                throw new Exception("请正确设置相机!");
            }

            var targetScreenRect=UguiTools.GetScreenRect(target);
            var targetWidth = (int)targetScreenRect.width;
            var targetHeight=(int)targetScreenRect.height;

            //对齐到目标画布
            var rootScaler = rootCanvas.GetComponent<CanvasScaler>();
            UguiTools.CopyProps(rootCanvas,m_Canvas);
            UguiTools.CopyProps(rootCanvas.transform, m_Canvas.transform);
            UguiTools.CopyProps(rootScaler, m_CanvasScaler);
            m_Canvas.worldCamera = m_Camera;
            if (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                m_Canvas.renderMode = RenderMode.ScreenSpaceCamera;
                var dist = screenHeight * 0.5f / Mathf.Tan(m_Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                m_Canvas.planeDistance = dist;
                m_Camera.transform.localPosition = new Vector3(0, 0, -dist/rootCanvas.scaleFactor);
            }
            else
            {
                UguiTools.CopyProps(rootCanvas.worldCamera, m_Camera);
            }

            //渲染准备
            var unusedLayer = UguiTools.FindUnusedLayer();
            if (unusedLayer < 0) unusedLayer = 31;
            unusedLayer = 5;
            gameObject.layer = unusedLayer;
            gameObject.SetActive(true);
            m_Camera.backgroundColor = Color.clear;
            m_Canvas.gameObject.layer = unusedLayer;
            m_Camera.cullingMask = 1 << unusedLayer;

            //置入
            var parent = target.transform.parent;
            target.transform.SetParent(transform, true);

            //渲染
            var rt = RenderTexture.GetTemporary(screenWidth, screenHeight, 32, RenderTextureFormat.ARGB32);
            m_Camera.targetTexture = rt;
            GL.Clear(true, true, Color.clear);
            m_Camera.RenderDontRestore();

            target.transform.SetParent(parent, true);
            gameObject.SetActive(false);

            //拷贝图像
            var cacheRT = RenderTexture.active;
            RenderTexture.active = rt;
            var sourceRect = new Rect(
                            targetScreenRect.x,
                            screenHeight-targetScreenRect.yMax,
                            targetScreenRect.width,
                            targetScreenRect.height
                            );
            if (tex == null)
                tex = new Texture2D(targetWidth, targetHeight, TextureFormat.ARGB32, false);
            if (tex.width != targetWidth || tex.height != targetHeight)
                tex.Resize(targetWidth, targetHeight, TextureFormat.ARGB32, false);
            tex.ReadPixels(sourceRect, 0, 0);
            tex.Apply();
            RenderTexture.active = cacheRT;

            RenderTexture.ReleaseTemporary(rt);
        }
    }
}