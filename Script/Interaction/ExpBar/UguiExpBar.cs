using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 经验条
    /// </summary>
    public class UguiExpBar : UIBehaviour
    {
        [SerializeField] protected Image m_Bar;
        [SerializeField] protected float m_Duration = 0.2f;

        protected int m_MaxLevel;
        protected int m_MinLevel;
        protected int m_CurrentLevel;
        protected int m_CurrentExp;
        protected int m_CurrentTotalExp;
        protected int m_TotalExp;
        protected float m_FillAmount;
        protected bool m_IsUp;
        protected List<ExpRange> rangeList;

        protected int m_AnimCurrentTotalExp;

        public Action<UguiExpBar> OnReachMaxLevel;              //达到等级上限
        public Action<UguiExpBar> OnReachMinLevel;              //达到等级下限
        public Action<UguiExpBar> OnLevelUp;                    //升级
        public Action<UguiExpBar> OnLevelDown;                  //降级

        protected override void Awake()
        {
            base.Awake();


        }

        protected virtual void Update()
        {
            var level = -1;
            foreach (var range in rangeList)
            {
                if(range.from<= m_AnimCurrentTotalExp && range.to>= m_AnimCurrentTotalExp)
                {
                    level = range.level;
                    break;
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="currentLevel"></param>
        /// <param name="currentExp"></param>
        /// <param name="levelStepExpList">每一级对应的经验值数组</param>
        /// <param name="minLevel"></param>
        /// <param name="maxLevel"></param>
        public virtual void Init(int currentLevel,int currentExp,List<int>levelStepExpList,int minLevel,int maxLevel)
        {
            if (levelStepExpList == null)
                throw new Exception("数组为空.");
            var step = maxLevel - minLevel+1;
            if (levelStepExpList.Count != step)
                throw new Exception("数组长度应等于："+step);

            m_CurrentLevel = currentLevel;
            m_CurrentExp = currentExp;
            m_MinLevel = minLevel;
            m_MaxLevel = maxLevel;
            m_TotalExp = 0;

            if (rangeList == null)
                rangeList = new List<ExpRange>();
            rangeList.Clear();
            var level = minLevel;
            foreach (var exp in levelStepExpList)
            {
                var from = m_TotalExp;
                var to = from + exp;
                var range = new ExpRange(level, from, to);
                rangeList.Add(range);

                m_TotalExp = to;
                level++;
            }
        }

        /// <summary>
        /// 增加经验，支持负数
        /// </summary>
        /// <param name="exp"></param>
        public virtual void AddExp(int exp)
        {
            if (exp == 0) return;

            m_IsUp = exp > 0 ? true : false;
            m_CurrentTotalExp += exp;
            m_CurrentTotalExp = Mathf.Clamp(m_CurrentTotalExp, 0, m_TotalExp);
        }

        /// <summary>
        /// 最大等级
        /// </summary>
        public int MaxLevel { get { return m_MaxLevel; } }

        /// <summary>
        /// 最小等级
        /// </summary>
        public int MinLevel { get { return m_MinLevel; } }

        /// <summary>
        /// 当前等级
        /// </summary>
        public int CurrentLevel { get { return m_CurrentLevel; } }

        /// <summary>
        /// 当前等级的经验值
        /// </summary>
        public int CurrentExp { get { return m_CurrentExp; } }

        /// <summary>
        /// 累计总经验值
        /// </summary>
        public int CurrentTotalExp { get { return m_CurrentTotalExp; } }

        /// <summary>
        /// 从最低等级到最高等级经验总值
        /// </summary>
        public int TotalExp { get { return m_TotalExp; } }

        protected struct ExpRange
        {
            public int level;
            public int from;
            public int to;

            public ExpRange(int level,int from,int to)
            {
                this.level = level;
                this.from = from;
                this.to = to;
            }
        }
    }
}