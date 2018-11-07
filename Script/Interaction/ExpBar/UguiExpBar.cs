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
        public int addExp;

        [SerializeField] protected Image m_ProgressBar;
        [SerializeField] protected Text m_LevelText;
        [SerializeField] protected Text m_CurrentLevelExpText;
        [SerializeField] protected Text m_CurrentLevelTotalExpText;

        [Range(0.01f,1f)]
        [SerializeField] protected float m_Speed = 0.1f;
        protected bool m_Init;

        protected int m_MaxLevel;
        protected int m_MinLevel;
        protected int m_CurrentLevel;
        protected int m_CurrentExp;
        protected int m_CurrentTotalExp;
        protected int m_TotalExp;
        protected float m_FillAmount;
        protected List<ExpRange> rangeList;

        protected int m_AnimCurrentTotalExp;
        protected int m_AnimCurrentLevel;

        public Action<UguiExpBar> OnReachMaxLevel;              //达到等级上限
        public Action<UguiExpBar> OnReachMinLevel;              //达到等级下限
        public Action<UguiExpBar> OnLevelUp;                    //升级
        public Action<UguiExpBar> OnLevelDown;                  //降级

        protected override void Awake()
        {
            base.Awake();

            if (m_ProgressBar == null)
                throw new Exception("进度条不能为null.");
            m_ProgressBar.type = Image.Type.Filled;

            var maxLevel = 10;
            var minLevel = 0;
            var list = new List<int>();
            for (var i=0;i<=maxLevel-minLevel;i++)
            {
                list.Add(UnityEngine.Random.Range(1,1000));
            }
            Init(1, 0, list, minLevel, maxLevel);
        }

        protected virtual void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                AddExp(addExp);
            }

            UpdateState();
        }

        protected virtual void UpdateState()
        {
            if (!m_Init) return;

            var cahceAnimTotalExp = m_AnimCurrentTotalExp;
            m_AnimCurrentTotalExp = (int)Mathf.Lerp(m_AnimCurrentTotalExp, m_CurrentTotalExp,m_Speed);

            if(cahceAnimTotalExp!= m_AnimCurrentTotalExp)
            {
                var animLevel = -1;
                var animCurrentExp = -1;
                var currentLevelTotalExp = -1;
                if (m_AnimCurrentTotalExp == m_TotalExp)
                {
                    m_AnimCurrentLevel = m_MaxLevel;
                    animLevel = m_AnimCurrentLevel;
                }
                else
                {
                    foreach (var range in rangeList)
                    {
                        if (range.from <= m_AnimCurrentTotalExp && m_AnimCurrentTotalExp < range.to)
                        {
                            animLevel = range.level;
                            animCurrentExp = m_AnimCurrentTotalExp - range.from;
                            currentLevelTotalExp = range.exp;
                            break;
                        }
                    }
                }
                if (animLevel < m_MinLevel || currentLevelTotalExp <= 0)
                {
                    Debug.LogError("数据错误");
                    return;
                }

                if (m_ProgressBar != null)
                {
                    m_ProgressBar.fillAmount = (float)(animCurrentExp / (double)currentLevelTotalExp);
                }
                if (m_LevelText != null)
                {
                    m_LevelText.text = animLevel.ToString();
                }
                if (m_CurrentLevelExpText != null)
                {
                    m_CurrentLevelExpText.text = animCurrentExp.ToString();
                }
                if (m_CurrentLevelTotalExpText != null)
                {
                    m_CurrentLevelTotalExpText.text = currentLevelTotalExp.ToString();
                }

                if (m_AnimCurrentTotalExp >= m_TotalExp)
                {
                    if (OnReachMaxLevel != null)
                    {
                        OnReachMaxLevel.Invoke(this);
                    }
                }
                else if (m_AnimCurrentTotalExp <= 0)
                {
                    if (OnReachMinLevel != null)
                    {
                        OnReachMinLevel.Invoke(this);
                    }
                }
                else if (m_AnimCurrentLevel < animLevel)
                {
                    if (OnLevelUp != null)
                    {
                        OnLevelUp.Invoke(this);
                    }
                }
                else if (m_AnimCurrentLevel > animLevel)
                {
                    if (OnLevelDown != null)
                    {
                        OnLevelDown.Invoke(this);
                    }
                }
                m_AnimCurrentLevel = animLevel;
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
                throw new Exception("数组长度应为："+step);
            if (minLevel < 0||maxLevel<0)
                throw new Exception("等级需大于等于0");

            m_CurrentLevel = currentLevel;
            m_CurrentExp = currentExp;
            m_MinLevel = minLevel;
            m_MaxLevel = maxLevel;
            m_TotalExp = 0;
            m_CurrentTotalExp = 0;

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
            var currentLevelExp = 0;
            foreach (var range in rangeList)
            {
                if (range.level == currentLevel)
                {
                    currentLevelExp = range.exp;
                    m_CurrentTotalExp = range.from + currentExp;
                    break;
                }
            }

            m_AnimCurrentTotalExp = m_CurrentTotalExp;
            m_AnimCurrentLevel = currentLevel;
            if (m_ProgressBar != null)
            {
                m_ProgressBar.fillAmount = (float)(currentExp / (double)currentLevelExp);
            }
            if (m_LevelText != null)
            {
                m_LevelText.text = currentLevel.ToString();
            }
            if (m_CurrentLevelExpText != null)
            {
                m_CurrentLevelExpText.text = currentExp.ToString();
            }
            if (m_CurrentLevelTotalExpText != null)
            {
                m_CurrentLevelTotalExpText.text = currentLevelExp.ToString();
            }

            m_Init = true;
        }

        /// <summary>
        /// 增加经验
        /// 正数增加,负数减少
        /// </summary>
        /// <param name="exp"></param>
        public virtual void AddExp(int exp)
        {
            CheckInit();

            if (exp == 0) return;

            m_CurrentTotalExp += exp;
            m_CurrentTotalExp = Mathf.Clamp(m_CurrentTotalExp, 0, m_TotalExp);
            if (m_CurrentTotalExp == m_TotalExp)
            {
                m_CurrentLevel = m_MaxLevel;
            }
            else
            {
                foreach (var range in rangeList)
                {
                    if (m_CurrentTotalExp >= range.from && m_CurrentTotalExp < range.to)
                    {
                        m_CurrentLevel = range.level;
                        break;
                    }
                }
            }
        }

        protected virtual void CheckInit()
        {
            if (!m_Init)
            {
                throw new Exception("需先执行初始化Init()方法");
            }
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
            public int exp;

            public ExpRange(int level,int from,int to)
            {
                this.level = level;
                this.from = from;
                this.to = to;
                this.exp = to - from;
            }
        }
    }
}