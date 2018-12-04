using UnityEngine;
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
        Dictionary<string, IUguiHistoryElement> elementDict;
        Dictionary<string, List<object>> stateDict;
        int stateIndex;

        public void OnSingletonCreate(UguiHistoryManager instance)
        {
            stateDict = new Dictionary<string, List<object>>();
            elementDict = new Dictionary<string, IUguiHistoryElement>();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="element"></param>
        public void Register(IUguiHistoryElement element)
        {
            var guid = element.GUID;
            if (!stateDict.ContainsKey(guid))
            {
                stateDict.Add(guid, new List<object>());
            }
            else
            {
                stateDict[guid].Clear();
            }

            if (!elementDict.ContainsKey(guid))
            {
                elementDict.Add(guid,element);
            }
            else
            {
                elementDict[guid] = element;
            }
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="element"></param>
        public void Unregister(IUguiHistoryElement element)
        {
            var guid = element.GUID;
            stateDict.Remove(guid);
            elementDict.Remove(guid);
        }

        /// <summary>
        /// 上一步
        /// </summary>
        public void GotoPrevState()
        {

        }

        /// <summary>
        /// 下一步
        /// </summary>
        public void GotoNextState()
        {

        }

        /// <summary>
        /// 快照,保存当前状态
        /// </summary>
        public void Snapshoot()
        {

        }
    }
}