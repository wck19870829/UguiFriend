using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 滚动广告栏
    /// </summary>
    public class UguiFlowBanner : UguiBanner
    {
        [SerializeField] protected UguiWrapContent content;

        public Vector3 normal;
        public Transform target;
        public Transform plane;
        public Transform refObject;

        private void Update()
        {

        }
    }
}