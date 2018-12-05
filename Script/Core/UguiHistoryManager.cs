﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 历史记录管理器
    /// </summary>
    public sealed class UguiHistoryManager : UguiSingleton<UguiHistoryManager>,
        IUguiSingletonCreate<UguiHistoryManager>
    {
        Dictionary<string, List<object>> stateDict;
        Dictionary<string, IUguiHistoryElement> elementDict;
        int m_CurrentStep;
        int m_StepCount;

        public void OnSingletonCreate(UguiHistoryManager instance)
        {
            elementDict = new Dictionary<string, IUguiHistoryElement>();
            stateDict = new Dictionary<string, List<object>>();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="element"></param>
        public void Register(IUguiHistoryElement element)
        {
            var guid = element.GUID;

            if (!elementDict.ContainsKey(guid))
            {
                elementDict.Add(guid, element);
            }
            else
            {
                elementDict[guid] = element;
            }

            if (!stateDict.ContainsKey(guid))
            {
                stateDict.Add(guid, new List<object>());
            }
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="element"></param>
        public void Unregister(IUguiHistoryElement element)
        {
            var guid = element.GUID;
            elementDict.Remove(guid);
            stateDict.Remove(guid);
        }

        /// <summary>
        /// 上一步
        /// </summary>
        public void GotoPrevStep()
        {
            if (m_CurrentStep <= 0) return;

            m_CurrentStep--;
            m_CurrentStep = Mathf.Clamp(m_CurrentStep, 0, m_StepCount - 1);
            foreach (var item in elementDict)
            {
                var state = stateDict[item.Key][m_CurrentStep];
                item.Value.GotoPrevStep(state);
            }
        }

        /// <summary>
        /// 下一步
        /// </summary>
        public void GotoNextStep()
        {
            if (m_CurrentStep >= m_StepCount - 1) return;

            m_CurrentStep++;
            m_CurrentStep = Mathf.Clamp(m_CurrentStep, 0, m_StepCount - 1);
            foreach (var item in elementDict)
            {
                var state = stateDict[item.Key][m_CurrentStep];
                item.Value.GotoNextStep(state);
            }
        }

        /// <summary>
        /// 跳转到
        /// </summary>
        /// <param name="frame"></param>
        public void GotoStep(int step)
        {
            if (step < 0 || step >= m_StepCount) return;

            m_CurrentStep = step;
            m_CurrentStep = Mathf.Clamp(m_CurrentStep, 0, m_StepCount - 1);
            foreach (var item in elementDict)
            {
                var state = stateDict[item.Key][m_CurrentStep];
                item.Value.GotoStep(state);
            }
        }

        /// <summary>
        /// 快照
        /// </summary>
        /// <param name="step"></param>
        public void Snapshoot(int step)
        {
            if (step < 0 || step > m_StepCount)
            {
                return;
            }

            m_CurrentStep = step;
            foreach (var item in elementDict)
            {
                var state=item.Value.DeepCloneState();
                stateDict[item.Key].RemoveRange(m_CurrentStep, Mathf.Max(0,stateDict[item.Key].Count- m_CurrentStep));
                stateDict[item.Key].Add(state);
            }
            m_StepCount = m_CurrentStep + 1;
        }

        /// <summary>
        /// 清除历史记录
        /// </summary>
        public void Clear()
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
        public int CurrentStep
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
        public int StepCount
        {
            get
            {
                return m_StepCount;
            }
        }
    }
}