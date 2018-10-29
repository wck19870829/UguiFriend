using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 滚动
    /// </summary>
    public class UguiContentScroll : UIBehaviour
    {
        public Vector2 direction=Vector2.left;
        public float speed = 1;
        [SerializeField] protected GameObject content;

        protected virtual void Update()
        {
            Scroll();
        }

        protected virtual void Scroll()
        {
            if (content == null) return;

            transform.Translate(direction.normalized * speed, Space.Self);
        }
    }
}