using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using System.IO;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 搜索框
    /// </summary>
    public class UguiSearchBox : UIBehaviour
    {
        protected const string cacheKey = "UguiSearchHistory";

        [SerializeField] protected Button m_SubmitButton;                                       //搜索按钮
        [SerializeField] protected Button m_ClearAllHistoryButton;                              //清除所有历史记录
        [SerializeField] protected InputField m_InputField;
        [SerializeField] protected RectTransform m_HistoryContent;
        [SerializeField] protected GridLayoutGroup m_HistoryGrid;
        [SerializeField] protected UguiSearchBoxHistoryItem m_HistoryItemPrefabSource;
        [SerializeField] protected bool m_CacheHistory = true;                                  //缓存历史记录到本地
        [SerializeField] protected int m_CacheCount = 10;                                       //缓存几条历史记录

        protected List<string> m_HistoryList;

        public Action<string> OnSearch;

        protected UguiSearchBox()
        {
            m_HistoryList = new List<string>();
        }

        protected override void Awake()
        {
            base.Awake();

            m_InputField.onEndEdit.AddListener(OnEndEdit);
            m_SubmitButton.onClick.AddListener(OnSubmitButtonClick);
            if (m_ClearAllHistoryButton)
                m_ClearAllHistoryButton.onClick.AddListener(OnClearAllHistoryButtonClick);
        }

        protected virtual void OnEndEdit(string text)
        {
            Search(m_InputField.text);
        }

        protected virtual void OnSubmitButtonClick()
        {
            Search(m_InputField.text);
        }

        protected virtual void OnClearAllHistoryButtonClick()
        {
            ClearHistoryCache();
        }

        /// <summary>
        /// 删除单条历史记录
        /// </summary>
        /// <param name="text"></param>
        public virtual void DeleteHistory(string text)
        {
            m_HistoryList.Remove(text);
            SetHistory(m_HistoryList);
        } 

        /// <summary>
        /// 设置历史记录信息
        /// </summary>
        /// <param name="historyList"></param>
        public virtual void SetHistory(List<string> historyList)
        {
            if (historyList == null)
                throw new Exception("List is null!");

            m_HistoryList = historyList;
            RefreshView();
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="text"></param>
        public virtual void Search(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            RefreshView();
            CloseHistory();

            if (OnSearch != null)
            {
                OnSearch.Invoke(text);
            }
        }

        /// <summary>
        /// 缓存历史记录
        /// </summary>
        protected virtual void CacheHistory()
        {
            var cacheHistory = PlayerPrefs.GetString(cacheKey);
            var sr = new StringReader(cacheHistory);
            m_HistoryList.Clear();
            while (true)
            {
                var line = sr.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                m_HistoryList.Add(line);
            }

            //删除无效搜索
            m_HistoryList.RemoveAll(
            (x) =>
            {
                return string.IsNullOrEmpty(x);
            });

            ////如果历史记录中有相同搜索,那么删除旧搜索
            //var findIndex = m_HistoryList.IndexOf(text);
            //if (findIndex>=0)
            //{
            //    m_HistoryList.RemoveAt(findIndex);
            //}

            ////新搜索置顶
            //m_HistoryList.Insert(0, text);

            //删除超过缓存数量上限的搜索
            var removeCount = m_HistoryList.Count - m_CacheCount;
            if (removeCount > 0)
            {
                m_HistoryList.RemoveRange(m_CacheCount, removeCount);
            }

            var sw = new StringWriter();
            foreach (var history in m_HistoryList)
            {
                sw.WriteLine(history);
            }
            PlayerPrefs.SetString(cacheKey, sw.ToString());
        }

        protected virtual void RefreshView()
        {
            m_HistoryList.RemoveAll(
                (x) =>
                {
                    return string.IsNullOrEmpty(x);
                });
            if (m_CacheHistory)
                CacheHistory();

            if (m_HistoryContent && m_HistoryItemPrefabSource&& m_HistoryGrid)
            {
                var historyItems = m_HistoryGrid.GetComponentsInChildren<UguiSearchBoxHistoryItem>(true);
                var createCount = m_HistoryList.Count - historyItems.Length;
                for (var i = 0; i < createCount; i++)
                {
                    UguiTools.AddChild<UguiSearchBoxHistoryItem>(m_HistoryItemPrefabSource, m_HistoryGrid.transform);
                }
                historyItems = m_HistoryGrid.GetComponentsInChildren<UguiSearchBoxHistoryItem>(true);
                for (var i=0;i< historyItems.Length;i++)
                {
                    var isActive = i < m_HistoryList.Count ? true : false;
                    historyItems[i].gameObject.SetActive(isActive);
                    if (isActive)
                    {
                        historyItems[i].SetText(m_HistoryList[i]);
                    }
                }
                var cellSize= (m_HistoryItemPrefabSource.transform as RectTransform).rect.size;
                cellSize.x = (transform as RectTransform).rect.width;
                m_HistoryGrid.cellSize = cellSize;

                var scrollRect = m_HistoryGrid.GetComponentInParent<ScrollRect>();
                if (scrollRect)
                {

                }
            }
        }

        /// <summary>
        /// 清除历史记录缓存
        /// </summary>
        public virtual void ClearHistoryCache()
        {
            PlayerPrefs.DeleteKey(cacheKey);
            m_HistoryList.Clear();
            RefreshView();
        }

        /// <summary>
        /// 打开历史记录容器
        /// </summary>
        public virtual void OpenHistory()
        {
            if (m_HistoryContent)
            {
                m_HistoryContent.gameObject.SetActive(true);

            }
        }

        /// <summary>
        /// 关闭历史记录容器
        /// </summary>
        public virtual void CloseHistory()
        {
            if (m_HistoryContent)
            {
                m_HistoryContent.gameObject.SetActive(false);
            }
        }
    }
}