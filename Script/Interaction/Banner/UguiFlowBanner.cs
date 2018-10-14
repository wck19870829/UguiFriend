using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 滚动广告栏
    /// </summary>
    public class UguiFlowBanner : UguiBanner
    {
        public float sideBorder=300;                    //侧边距
        public float bannerBorder=400;                  //广告边距，此值总是大于sideBorder
        public int sideItemCount = 2;                   //侧边元素最大数量

        protected virtual void Update()
        {

        }

        private void OnValidate()
        {
            sideBorder = Mathf.Max(sideBorder,0);
            bannerBorder = Mathf.Clamp(bannerBorder,sideBorder, bannerBorder);
        }
    }
}