using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    [ExecuteInEditMode]
    public abstract class UguiEffectControlByPosition : UIBehaviour
    {
        [SerializeField] protected Transform content;                   //容器
        [SerializeField] protected AnimationCurve weightCurve;          //采样曲线
        [SerializeField] protected Transform startPoint;                //边界开始
        [SerializeField] protected Transform endPoint;                  //边界结束
        [SerializeField] protected bool isCenterMirror;                 //是否中心镜像

        protected UguiEffectControlByPosition()
        {
            weightCurve = AnimationCurve.EaseInOut(0,0,1,1);
        }

        protected override void Start()
        {
            base.Start();

            if (content == null)
                throw new System.Exception("Content is null.");
            if (startPoint == null)
                throw new System.Exception("Start point is null.");
            if (endPoint == null)
                throw new System.Exception("End point is null.");

            CacheItems();
        }

        protected virtual void Update()
        {
            MidifyChildren();
        }
        
        /// <summary>
        /// 更新子元素
        /// </summary>
        protected abstract void MidifyChildren();

        /// <summary>
        /// 缓存子物体
        /// </summary>
        public abstract void CacheItems();

        /// <summary>
        /// 获得权重
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual float GetWeight(Transform item)
        {
            if (startPoint == null || endPoint == null || item == null) return 0;

            var projectPoint = UguiTools.ProjectPointLine(item.position, startPoint.position, endPoint.position);
            if (isCenterMirror)
            {
                var center = Vector3.Lerp(startPoint.position, endPoint.position, 0.5f);
                var weight = Vector3.Distance(projectPoint, center) / Vector3.Distance(startPoint.position, center);
                return weightCurve.Evaluate(weight);
            }
            else
            {
                var dist = Vector3.Distance(startPoint.position,endPoint.position);
                var dirStart2End = endPoint.position - startPoint.position;
                var dirStart2Item = projectPoint - startPoint.position;
                var sign=Mathf.Sign(Vector3.Dot(dirStart2End, dirStart2Item));
                var weight = sign * Vector3.Distance(projectPoint, startPoint.position) / dist;
                return weightCurve.Evaluate(weight);
            }
        }
    }

    /// <summary>
    /// 通过位置权重控制物体变化
    /// </summary>
    public abstract class UguiEffectControlByPosition<Item,Value>: UguiEffectControlByPosition
        where Item : Component
    {
        [SerializeField] protected Value valueFrom;
        [SerializeField] protected Value valueTo;
        protected List<Item> cacheItmeList;

        protected UguiEffectControlByPosition()
        {
            cacheItmeList = new List<Item>();
        }

        public override void CacheItems()
        {
            if (content == null) return;

            cacheItmeList.Clear();
            foreach (Transform t in content)
            {
                var component = t.GetComponent<Item>();
                if (component != null)
                {
                    cacheItmeList.Add(component);
                }
            }
        }

        protected override void MidifyChildren()
        {
            if (startPoint == null || endPoint == null) return;

            foreach (var item in cacheItmeList)
            {
                UpdateItem(item,GetWeight(item.transform));
            }
        }

        protected abstract void UpdateItem(Item item, float weight);
    }
}