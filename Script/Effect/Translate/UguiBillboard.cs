using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 广告牌
    /// </summary>
    public class UguiBillboard : UIBehaviour
    {
        public Camera m_Camera;
        protected Canvas m_Canvas;

        protected virtual void Update()
        {
            if (m_Camera != null)
            {
                SetRotation(m_Camera);
            }
            else
            {
                if (m_Canvas == null)
                    m_Canvas = GetComponentInParent<Canvas>();
                if (m_Canvas != null)
                {
                    SetRotation(m_Canvas.rootCanvas.worldCamera);
                }
            }
        }

        protected void SetRotation(Camera cam)
        {
            var camBack = -cam.transform.forward;
            var plane = new Plane(cam.transform.position, -camBack);
            var ray = new Ray(transform.position, camBack);
            float dist;
            plane.Raycast(ray, out dist);
            var point = transform.position + camBack * dist;
            transform.LookAt(point, Vector3.up);
        }
    }
}