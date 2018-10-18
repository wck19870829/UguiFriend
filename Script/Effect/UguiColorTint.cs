using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    [ExecuteInEditMode]
    /// <summary>
    /// 颜色着色
    /// </summary>
    public class UguiColorTint : MonoBehaviour
    {
        public Color color = Color.white;
        Graphic[] graphics;
        Color cacheColor;

        protected virtual void Awake()
        {
            graphics = gameObject.GetComponentsInChildren<Graphic>(true);
        }

        protected virtual void Update()
        {
            if (color!=cacheColor)
            {
                cacheColor = color;
                foreach (var graphic in graphics)
                {
                    graphic.color = color;
                }
            }
        }
    }
}