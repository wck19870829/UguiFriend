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
        Dictionary<string, LinkedList<StateKey>> stateDict;
        int stateFrame;

        public void OnSingletonCreate(UguiHistoryManager instance)
        {
            stateDict = new Dictionary<string, LinkedList<StateKey>>();
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
                stateDict.Add(guid,new LinkedList<StateKey>());
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
        /// 直接跳转到帧
        /// </summary>
        /// <param name="frame"></param>
        public void GotoState(int frame)
        {

        }

        /// <summary>
        /// 快照,保存当前状态
        /// </summary>
        public void Snapshoot()
        {

        }

        [System.Serializable]
        private class StateLine
        {


            /// <summary>
            /// 关键帧信息
            /// </summary>
            private class StateKey
            {
                public int frame;
                public object state;
            }
        }
    }
}