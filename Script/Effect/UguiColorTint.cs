using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    [ExecuteInEditMode]
    /// <summary>
    /// 颜色着色
    /// </summary>
    public class UguiColorTint : UIBehaviour
    {
        [SerializeField] public float duration;
        [SerializeField] protected Color m_Color = Color.white;
        protected Graphic[] graphics;
        protected Color cacheColor;

        protected override void Awake()
        {
            base.Awake();

            graphics = gameObject.GetComponentsInChildren<Graphic>(true);
        }

        protected virtual void Update()
        {
            if (m_Color != cacheColor)
            {
                cacheColor = m_Color;
                foreach (var graphic in graphics)
                {
                    graphic.color = m_Color;
                }
            }
        }

        public Color Color
        {
            get
            {
                return m_Color;
            }
            set
            {
                m_Color = value;
            }
        }
    }
}