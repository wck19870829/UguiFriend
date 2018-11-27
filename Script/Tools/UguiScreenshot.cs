using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 截屏
    /// </summary>
    public sealed class UguiScreenshot : MonoBehaviour
    {
        static UguiScreenshot s_Current;

        Camera m_Camera;
        Canvas m_Canvas;

        public static UguiScreenshot GetInstance()
        {
            if (s_Current == null)
            {
                var go = UguiTools.AddChild<Transform>("Screenshot").gameObject;
                go.SetActive(false);
                s_Current = go.AddComponent<UguiScreenshot>();

                var cam = UguiTools.AddChild<Camera>("Camera", go.transform);
                cam.backgroundColor = Color.clear;
                cam.clearFlags = CameraClearFlags.SolidColor;
                s_Current.m_Camera = cam;

                var canvas = go.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = cam;
                s_Current.m_Canvas = canvas;
            }

            return s_Current;
        }

        private void OnDestroy()
        {
            s_Current = null;
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

            if (tex == null)
                tex = new Texture2D(targetWidth, targetHeight, TextureFormat.ARGB32, false);
            if (tex.width != targetWidth || tex.height != targetHeight)
                tex.Resize(targetWidth, targetHeight, TextureFormat.ARGB32, false);

            var rt = RenderTexture.GetTemporary(screenWidth, screenHeight, 32, RenderTextureFormat.ARGB32);

            if(rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                var dist = screenHeight*0.5f/Mathf.Tan(m_Camera.fieldOfView * 0.5f*Mathf.Deg2Rad);
                m_Canvas.planeDistance = dist;
                m_Camera.transform.localPosition = new Vector3(0,0,-dist);
                gameObject.SetActive(true);

                //置入
                var cacheParent = target.transform.parent;
                target.transform.SetParent(transform);

                var camRT = m_Camera.targetTexture;
                m_Camera.targetTexture = rt;
                m_Camera.RenderDontRestore();
                m_Camera.targetTexture = camRT;

                target.transform.SetParent(cacheParent);
                gameObject.SetActive(false);
            }
            else
            {
                var camRT = rootCanvas.worldCamera.targetTexture;
                rootCanvas.worldCamera.targetTexture = rt;
                rootCanvas.worldCamera.RenderDontRestore();
                rootCanvas.worldCamera.targetTexture = camRT;
            }

            var cacheRT = RenderTexture.active;
            RenderTexture.active = rt;
            var sourceRect = new Rect(
                            targetScreenRect.x,
                            screenHeight-targetScreenRect.yMax,
                            targetScreenRect.width,
                            targetScreenRect.height
                            );
            tex.ReadPixels(sourceRect, 0, 0);
            tex.Apply();
            RenderTexture.active = cacheRT;

            RenderTexture.ReleaseTemporary(rt);
        }
    }
}