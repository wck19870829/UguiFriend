using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 元素循环
    /// </summary>
    public class UguiWrapContent : MonoBehaviour
    {
        public float gap = 100;
        [SerializeField]protected Direction direction=Direction.Vertical;
        protected List<Transform> items;

        private void Update()
        {
            
        }

        public enum Direction
        {
            Horizontal,
            Vertical
        }
    }
}