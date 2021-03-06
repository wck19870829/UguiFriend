﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 历史记录管理器
    /// </summary>
    public sealed class UguiHistoryManager
    {
        static Dictionary<string, List<UguiObjectData>> stateDict;
        static int m_CurrentStep;
        static int m_StepCount;

        static UguiHistoryManager()
        {
            stateDict = new Dictionary<string, List<UguiObjectData>>();
        }

        /// <summary>
        /// 上一步
        /// </summary>
        public static void GotoPrevStep()
        {
            if (m_CurrentStep <= 0) return;

            m_CurrentStep--;
            m_CurrentStep = Mathf.Clamp(m_CurrentStep, 0, m_StepCount - 1);
            foreach (var item in UguiObjectManager.ObjectDict)
            {
                var state = stateDict[item.Key][m_CurrentStep];
                item.Value.GotoPrevStep(state);
            }
        }

        /// <summary>
        /// 下一步
        /// </summary>
        public static void GotoNextStep()
        {
            if (m_CurrentStep >= m_StepCount - 1) return;

            m_CurrentStep++;
            m_CurrentStep = Mathf.Clamp(m_CurrentStep, 0, m_StepCount - 1);
            foreach (var item in UguiObjectManager.ObjectDict)
            {
                var state = stateDict[item.Key][m_CurrentStep];
                item.Value.GotoNextStep(state);
            }
        }

        /// <summary>
        /// 跳转到
        /// </summary>
        /// <param name="step"></param>
        public static void GotoStep(int step)
        {
            if (step < 0 || step >= m_StepCount) return;

            m_CurrentStep = step;
            m_CurrentStep = Mathf.Clamp(m_CurrentStep, 0, m_StepCount - 1);
            foreach (var item in UguiObjectManager.ObjectDict)
            {
                var state = stateDict[item.Key][m_CurrentStep];
                item.Value.GotoStep(state);
            }
        }

        /// <summary>
        /// 快照
        /// </summary>
        /// <param name="step"></param>
        public static void Snapshoot(int step)
        {
            if (step < 0 || step > m_StepCount)
            {
                return;
            }

            m_CurrentStep = step;
            foreach (var item in UguiObjectManager.ObjectDict)
            {
                var state=item.Value.Snapshoot();
                stateDict[item.Key].RemoveRange(m_CurrentStep, Mathf.Max(0,stateDict[item.Key].Count- m_CurrentStep));
                stateDict[item.Key].Add(state);
            }
            m_StepCount = m_CurrentStep + 1;
        }

        /// <summary>
        /// 清除历史记录
        /// </summary>
        public static void Clear()
        {
            foreach (var item in stateDict.Values)
            {
                item.Clear();
            }
            m_CurrentStep = 0;
            m_StepCount = 0;
        }

        /// <summary>
        /// 当前步骤索引
        /// </summary>
        public static int CurrentStep
        {
            get
            {
                return m_CurrentStep;
            }
            set
            {
                m_CurrentStep = Mathf.Clamp(value,0,m_StepCount-1);
            }
        }

        /// <summary>
        /// 总步骤数
        /// </summary>
        public static int StepCount
        {
            get
            {
                return m_StepCount;
            }
        }
    }
}