using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 吸入特效
    /// </summary>
    public class UguiSuckEffect : RawImage
    {
        [SerializeField]protected Vector2 m_BlackHolePoint;
        [SerializeField]protected float m_Percent;
        [SerializeField]protected int m_SimpleDist=5;
        protected List<UIVertex> m_Verts;
        protected List<int> m_Indices;

        protected UguiSuckEffect()
        {
            m_Verts = new List<UIVertex>(1024);
            m_Indices = new List<int>(4096);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            m_Verts.Clear();
            m_Indices.Clear();

            //创建初始网格
            var rect = rectTransform.rect;
            var wCount = Mathf.FloorToInt(rect.width / m_SimpleDist);
            var hCount = Mathf.FloorToInt(rect.height / m_SimpleDist);
            var wInterval = rect.width / wCount;
            var hInterval = rect.height / hCount;
            for (var i = 0; i <= hCount; i++)
            {
                for (var j = 0; j <= wCount; j++)
                {
                    var xOffset = j * wInterval;
                    var yOffset = i * hInterval;
                    var currentPoint = new Vector2(xOffset + rect.x, yOffset + rect.y);

                    var uv = new Vector2(xOffset / rect.width, yOffset / rect.height);
                    var vertex = new UIVertex();
                    vertex.position = currentPoint;
                    vertex.color = color;
                    vertex.uv0 = vertex.uv1 = uv;

                    m_Verts.Add(vertex);
                }
            }
            for (var i=1;i<=hCount;i++)
            {
                for (var j=1;j<=wCount;j++)
                {
                    var current = i + j * wCount;
                    var left = current - 1;
                    var up = current - wCount;
                    var upLeft = up - 1;

                    m_Indices.Add(current);
                    m_Indices.Add(left);
                    m_Indices.Add(up);

                    m_Indices.Add(upLeft);
                    m_Indices.Add(left);
                    m_Indices.Add(up);
                }
            }

            vh.AddUIVertexStream(m_Verts, m_Indices);

            var t = Mathf.Max(m_Percent,0.00001f);
            var p1 = new Vector2(rect.xMin,rect.yMin);
            var p2 = new Vector2(rect.xMax,rect.yMin);
            var p3 = new Vector2(rect.xMax,rect.yMax);
            var p4 = new Vector2(rect.xMin,rect.yMax);
            var effectRange = Mathf.Max(
                                Vector2.Distance(p1, m_BlackHolePoint),
                                Vector2.Distance(p2, m_BlackHolePoint),
                                Vector2.Distance(p3, m_BlackHolePoint),
                                Vector2.Distance(p4, m_BlackHolePoint));

        }
    }
}