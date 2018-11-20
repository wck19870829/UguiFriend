using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 折线图
    /// </summary>
    public class UguiLineChart : UguiChart
    {
        protected const string defaultAreaMat="UguiFriend/Material/AreaChart";

        protected const int simpleDistance = 2;
        protected const int singleTexWidth = 1024;
        protected static readonly Color[] clearColors=new Color[singleTexWidth];

        [SerializeField] protected Material m_AreaMat;
        [SerializeField] protected Mask m_Mask;
        [SerializeField] protected List<Vector2> valueList;
        [SerializeField]protected Vector2 m_ScrollPos;
        [SerializeField] protected List<Texture2D> texList;
        [SerializeField] protected List<RawImage> imageList;
        protected UguiMathf.Bezier[] m_Beziers;
        protected List<Vector2> displayValueList;

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                var maskRect = m_Mask.rectTransform.rect;
                var singleSize= new Vector2(singleTexWidth, maskRect.height);
                FillValueTextures(ref texList, ref displayValueList, valueList, maskRect, m_ScrollPos);
                var addImageCount = texList.Count-imageList.Count;
                for (var i=0;i<addImageCount;i++)
                {
                    var image = UguiTools.AddChild<RawImage>("Image_" + i, m_Mask.transform);
                    imageList.Add(image);
                }
                if (m_AreaMat == null)
                {
                    var sourceMat = Resources.Load<Material>(defaultAreaMat);
                    m_AreaMat = new Material(sourceMat);
                }
                for (var i=0;i< imageList.Count;i++)
                {
                    var image = imageList[i];
                    if(image.material==null)
                        image.material = new Material(m_AreaMat);
                    image.texture = texList[i];
                    image.rectTransform.pivot = new Vector2(0, 1);
                    image.transform.localPosition = new Vector2(i*singleTexWidth,0);
                    image.rectTransform.sizeDelta = singleSize;
                }
            }
        }

        public override void Rebuild()
        {

        }

        /// <summary>
        /// 填充面积图遮罩
        /// </summary>
        /// <param name="texList"></param>
        /// <param name="valueList"></param>
        /// <param name="maskRect"></param>
        /// <param name="scrollPos"></param>
        protected void FillValueTextures(ref List<Texture2D>texList,ref List<Vector2>displayValueList,List<Vector2>valueList,Rect maskRect,Vector2 scrollPos)
        {
            if (valueList == null)
                throw new Exception("值数组为空");

            if (displayValueList == null)
                displayValueList = new List<Vector2>(valueList.Count);
            displayValueList.Clear();
            displayValueList.AddRange(valueList);

            var texCount = Mathf.CeilToInt(maskRect.width / singleTexWidth);
            if (texList == null)
                texList = new List<Texture2D>(texCount);
            var addCount = texCount - texList.Count;
            for (var i=0;i< addCount; i++)
            {
                var tex = new Texture2D(singleTexWidth, 1);
                texList.Add(tex);
            }

            displayValueList.Sort((a,b)=> {
                if (a.x == b.x) return 0;
                return a.x > b.x ? 1 : -1;
            });

            var startIndex = -1;
            var endIndex = 0;
            var widthTotal = texList.Count * singleTexWidth;
            for (var i=0;i< widthTotal;i++)
            {
                var texIndex = i / singleTexWidth;
                var colorIndex = i % singleTexWidth;
                if (displayValueList[endIndex].x < i)
                {
                    startIndex = endIndex;
                    endIndex++;
                    if (endIndex >= displayValueList.Count)
                    {
                        endIndex = -1;
                    }
                    if (startIndex >= displayValueList.Count)
                    {
                        startIndex = -1;
                    }
                }
                var startValue = (startIndex<0)?0:displayValueList[startIndex].y/maskRect.height;
                var endValue = (endIndex < 0)?0:displayValueList[endIndex].y / maskRect.height;
                var startX= (startIndex < 0) ? -1 : displayValueList[startIndex].x;
                var endX = (endIndex < 0) ? -1 : displayValueList[endIndex].x;
                if (i >= startX && i <= endX)
                {
                    var lerpValue = Mathf.Lerp(startValue, endValue,(i-startX)/(endX-startX));
                    var lerpColor = new Color(lerpValue, lerpValue, lerpValue);
                    texList[texIndex].SetPixel(colorIndex, 0, lerpColor);
                }
            }

            foreach (var tex in texList)
            {
                tex.Apply();
            }
        }
    }
}