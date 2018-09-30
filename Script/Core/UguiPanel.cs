using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(Canvas))]
    /// <summary>
    /// 显示面板基类
    /// </summary>
    public abstract class UguiPanel : UguiObject
    {
        protected Canvas m_Canvas;

        protected override void Awake()
        {
            base.Awake();

            m_Canvas = GetComponent<Canvas>();
            if (m_Canvas == null)
            {
                m_Canvas = gameObject.AddComponent<Canvas>();
            }
        }
    }

    public abstract class UguiPanelData : UguiObjectData
    {
        public int layer;
    }
}