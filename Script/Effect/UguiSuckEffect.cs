using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 吸入特效
    /// </summary>
    public class UguiSuckEffect : RawImage
    {
        [SerializeField]protected Vector2 m_BlackHolePoint;
        [SerializeField]protected float m_Percent;
        [SerializeField] protected int m_SimpleDist;
        protected List<UIVertex> m_Verts;
        protected List<int> m_Indices;

        protected UguiSuckEffect()
        {
            m_Verts = new List<UIVertex>(2048);
            m_Indices = new List<int>();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            m_Verts.Clear();
            m_Indices.Clear();

            var t = Mathf.Max(m_Percent,0.00001f);
            var rect= rectTransform.rect;
            var p1 = new Vector2(rect.xMin,rect.yMin);
            var p2 = new Vector2(rect.xMax,rect.yMin);
            var p3 = new Vector2(rect.xMax,rect.yMax);
            var p4 = new Vector2(rect.xMin,rect.yMax);
            var effectRange = Mathf.Max(
                                Vector2.Distance(p1, m_BlackHolePoint),
                                Vector2.Distance(p2, m_BlackHolePoint),
                                Vector2.Distance(p3, m_BlackHolePoint),
                                Vector2.Distance(p4, m_BlackHolePoint));

            var wCount = (int)(rect.width / m_SimpleDist);
            var hCount = (int)(rect.height/ m_SimpleDist);
            for (var i=0;i<wCount;i++)
            {
                for (var j=0;j<hCount;j++)
                {
                    var xOffset = i * m_SimpleDist;
                    var yOffset = j * m_SimpleDist;
                    var point = new Vector2(xOffset + rect.x, yOffset + rect.y);
                    var dist = Vector2.Distance(m_BlackHolePoint, point);
                    point = Vector2.Lerp(point, m_BlackHolePoint, dist / effectRange);

                    var uv = new Vector2(xOffset / rect.width, yOffset / rect.height);

                    var vertex = new UIVertex();
                    vertex.position = point;
                    vertex.uv0 = vertex.uv1 = uv;

                    m_Verts.Add(vertex);
                }
            }

            for (var i = 1; i < wCount; i++)
            {
                for (var j = 1; j < hCount; j++)
                {

                }
            }

            vh.AddUIVertexStream(m_Verts, m_Indices);
        }
    }
}