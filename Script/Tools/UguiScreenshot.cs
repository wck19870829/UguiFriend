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
        const int defaultSize = 2048;
        static UguiScreenshot s_Current;

        Camera m_Camera;

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
            var screenRect=UguiTools.GetScreenRect(target);

            var rt = RenderTexture.GetTemporary(defaultSize, defaultSize,32,RenderTextureFormat.ARGB32);
            if (tex == null)
                tex = new Texture2D((int)screenRect.width, (int)screenRect.height, TextureFormat.ARGB32, false);
            var cacheRT = RenderTexture.active;

            RenderTexture.active = cacheRT;
            RenderTexture.ReleaseTemporary(rt);
        }

        ///// <summary>
        ///// 快照
        ///// </summary>
        ///// <param name="tex">位图</param>
        ///// <param name="target">渲染的物体</param>
        ///// <param name="targetCam">渲染的物体摄像机</param>
        //public void Capture(ref Texture2D tex,Rect screenRect,GameObject target,Camera targetCam)
        //{
        //    var cam = targetCam == null
        //            ? m_Camera
        //            : targetCam;

        //    var width = Screen.width;
        //    var height = Screen.height;

        //    var screenShot = new Texture2D((int)screenRect.width, (int)screenRect.height, TextureFormat.ARGB32, false);
        //    screenShot.ReadPixels(screenRect, 0, 0);
        //    screenShot.Apply();
        //}
    }
}