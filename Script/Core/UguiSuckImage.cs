using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 可吸入的图片
    /// </summary>
    public class UguiSuckImage : RawImage
    {
        public const int minSimpleDist = 10;
        public const int maxSimpleDist = 100;

        [SerializeField] protected Vector2 m_BlackHolePoint;
        [SerializeField] protected float m_Percent;
        [SerializeField] protected int m_SimpleDist = 50;
        [SerializeField] protected float m_Duration=0.25f;
        protected List<UIVertex> m_Verts;
        protected List<int> m_Indices;
        protected State m_State;
        protected Texture2D m_ScreenShot;
        protected Texture m_CacheTex;
        protected Dictionary<GameObject, bool> m_ChildrenStateDict;

        protected UguiSuckImage()
        {
            m_Verts = new List<UIVertex>(1024);
            m_Indices = new List<int>(4096);
            m_ChildrenStateDict = new Dictionary<GameObject, bool>();
        }

        protected virtual void Update()
        {
            var duration = Mathf.Max(m_Duration,0.0001f);
            switch (m_State)
            {
                case State.Storage:
                    m_Percent += Time.deltaTime / duration;
                    m_Percent = Mathf.Clamp01(m_Percent);
                    SetVerticesDirty();
                    if (m_Percent >= 1)
                    {
                        m_State = State.None;
                    }
                    break;

                case State.TakeOut:
                    m_Percent -= Time.deltaTime / duration;
                    m_Percent = Mathf.Clamp01(m_Percent);
                    SetVerticesDirty();
                    if (m_Percent <= 0)
                    {
                        m_State = State.None;

                        //还原状态
                        foreach (var item in m_ChildrenStateDict)
                        {
                            item.Key.SetActive(item.Value);
                        }
                        m_ChildrenStateDict.Clear();
                    }
                    break;
            }
        }

        /// <summary>
        /// 快照
        /// </summary>
        protected virtual void Snapshoot()
        {


            m_CacheTex = this.texture;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            m_Verts.Clear();
            m_Indices.Clear();

            var simpleDist = Mathf.Clamp(m_SimpleDist, minSimpleDist, maxSimpleDist);
            var rect = rectTransform.rect;

            var p1 = new Vector2(rect.xMin, rect.yMin);
            var p2 = new Vector2(rect.xMax, rect.yMin);
            var p3 = new Vector2(rect.xMax, rect.yMax);
            var p4 = new Vector2(rect.xMin, rect.yMax);
            var effectRange = Mathf.Max(
                                Vector2.Distance(p1, m_BlackHolePoint),
                                Vector2.Distance(p2, m_BlackHolePoint),
                                Vector2.Distance(p3, m_BlackHolePoint),
                                Vector2.Distance(p4, m_BlackHolePoint)
                                );
            effectRange = Mathf.Max(effectRange, 0.00001f);

            var wCount = Mathf.Max(2, Mathf.CeilToInt(rect.width / simpleDist) + 1);
            var hCount = Mathf.Max(2, Mathf.CeilToInt(rect.height / simpleDist) + 1);
            var wInterval = rect.width / (wCount - 1);
            var hInterval = rect.height / (hCount - 1);
            for (var i = 0; i < hCount; i++)
            {
                for (var j = 0; j < wCount; j++)
                {
                    var xOffset = j * wInterval;
                    var yOffset = i * hInterval;
                    var point = new Vector2(xOffset + rect.x, yOffset + rect.y);

                    //计算偏移
                    var dist = Vector2.Distance(m_BlackHolePoint, point);
                    point = Vector2.Lerp(point, m_BlackHolePoint, m_Percent+(1-dist/ effectRange)*m_Percent);

                    var uv = new Vector2(xOffset / rect.width, yOffset / rect.height);
                    uv = UguiMathf.UVOffset(uv, uvRect);
                    var vertex = new UIVertex();
                    vertex.position = point;
                    vertex.color = color;
                    vertex.uv0 = vertex.uv1 = uv;

                    m_Verts.Add(vertex);
                }
            }
            for (var i = 1; i < hCount; i++)
            {
                for (var j = 1; j < wCount; j++)
                {
                    var current = j + i * wCount;
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
        }

        /// <summary>
        /// 收缩
        /// </summary>
        public virtual void Storage()
        {
            if (m_State == State.Storage||m_Percent==1) return;

            m_State = State.Storage;

            //缓存子物体状态
            m_ChildrenStateDict.Clear();
            var children = gameObject.GetComponentsInChildren<Transform>(false);
            foreach (var child in children)
            {
                m_ChildrenStateDict.Add(child.gameObject, child.gameObject.activeSelf);
            }
            m_ChildrenStateDict.Remove(gameObject);
            foreach (var item in m_ChildrenStateDict)
            {
                item.Key.SetActive(false);
            }

            UguiScreenshot.GetInstance().Capture(ref m_ScreenShot, this);
        }

        /// <summary>
        /// 显示
        /// </summary>
        public virtual void TakeOut()
        {
            if (m_State == State.TakeOut||m_Percent==0) return;

            m_State = State.TakeOut;
        }

        /// <summary>
        /// 当前进度
        /// </summary>
        public float Percent
        {
            get
            {
                return m_Percent;
            }
            set
            {
                m_Percent = Mathf.Clamp01(value);
                m_State = State.None;
                SetVerticesDirty();
            }
        }

        protected enum State
        {
            None,
            TakeOut,
            Storage
        }
    }
}