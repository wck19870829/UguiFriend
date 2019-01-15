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
    public class UguiSearchBox : Selectable
    {
        protected const string cacheKey = "UguiSearchHistory";

        [SerializeField] protected Button m_SubmitButton;                                       //搜索按钮
        [SerializeField] protected Button m_ClearAllHistoryButton;                              //清除所有历史记录
        [SerializeField] protected Button m_CloseHistoryButton;                                 //关闭历史记录按钮
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

        protected override void Start()
        {
            base.Start();

            var inputTrigger = m_InputField.GetComponent<EventTrigger>();
            if (inputTrigger == null)
                m_InputField.gameObject.AddComponent<EventTrigger>();
            var selectEntry = new EventTrigger.Entry();
            selectEntry.eventID = EventTriggerType.Select;
            selectEntry.callback.AddListener(SelectInputField);
            inputTrigger.triggers.Add(selectEntry);
            var deselectEntry = new EventTrigger.Entry();
            deselectEntry.eventID = EventTriggerType.Deselect;
            deselectEntry.callback.AddListener(DeselectInputField);
            inputTrigger.triggers.Add(deselectEntry);

            m_SubmitButton.onClick.AddListener(OnSubmitButtonClick);
            if (m_ClearAllHistoryButton)
                m_ClearAllHistoryButton.onClick.AddListener(OnClearAllHistoryButtonClick);
            if (m_CloseHistoryButton)
                m_CloseHistoryButton.onClick.AddListener(OnCloseHistory);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_InputField.text = "";
            CloseHistory();
        }

        protected virtual void OnCloseHistory()
        {
            CloseHistory();
        }

        protected virtual void SelectInputField(BaseEventData eventData)
        {
            OpenHistory();
        }

        protected virtual void DeselectInputField(BaseEventData eventData)
        {
            Debug.Log(EventSystem.current.currentSelectedGameObject);

            return;

            var pointerEventData = eventData as PointerEventData;
            if (pointerEventData!=null)
            {
                Debug.Log(pointerEventData.pointerPressRaycast.gameObject);
            }
            if (eventData.selectedObject)
            {
                Debug.Log(eventData.selectedObject.name);
                var selectParent=eventData.selectedObject.GetComponentInParent<UguiSearchBox>();
                if (selectParent==null||selectParent!=this)
                {
                    CloseHistory();
                }
            }
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

            //插入新数据
            m_HistoryList.Insert(0,text);

            //删除无效搜索
            m_HistoryList.RemoveAll(
            (x) =>
            {
                return string.IsNullOrEmpty(x);
            });

            //删除重复搜索
            for (var i = m_HistoryList.Count - 1; i >= 0; i--)
            {
                var findIndex = m_HistoryList.IndexOf(m_HistoryList[i]);
                if (findIndex >= 0 && findIndex < i)
                {
                    m_HistoryList.RemoveAt(i);
                }
            }

            //删除超过缓存数量上限的搜索
            var removeCount = m_HistoryList.Count - m_CacheCount;
            if (removeCount > 0)
            {
                m_HistoryList.RemoveRange(m_CacheCount, removeCount);
            }

            m_InputField.text = text;
            RefreshView();
            CloseHistory();

            Debug.LogFormat("Search:{0}",text);

            if (OnSearch != null)
            {
                OnSearch.Invoke(text);
            }
        }

        /// <summary>
        /// 加载历史记录
        /// </summary>
        protected virtual void LoadHistory()
        {
            if (m_CacheHistory)
            {
                m_HistoryList.Clear();
                var cacheHistory = PlayerPrefs.GetString(cacheKey);
                var sr = new StringReader(cacheHistory);
                while (true)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        break;

                    m_HistoryList.Add(line);
                }
            }
        }

        /// <summary>
        /// 缓存历史记录
        /// </summary>
        protected virtual void CacheHistory()
        {
            var sw = new StringWriter();
            foreach (var history in m_HistoryList)
            {
                sw.WriteLine(history);
            }
            PlayerPrefs.SetString(cacheKey, sw.ToString());
        }

        protected virtual void RefreshView()
        {
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
            CloseHistory();
        }

        /// <summary>
        /// 打开历史记录容器
        /// </summary>
        public virtual void OpenHistory()
        {
            if (m_HistoryContent)
            {
                if (!m_HistoryContent.gameObject.activeSelf)
                {
                    m_HistoryContent.gameObject.SetActive(true);

                    if (m_HistoryGrid)
                    {
                        var scrollRect = m_HistoryGrid.GetComponentInParent<ScrollRect>();
                        if (scrollRect)
                        {
                            scrollRect.normalizedPosition = new Vector2(0, 1);
                        }
                    }
                }
            }
            LoadHistory();
            RefreshView();
        }

        /// <summary>
        /// 关闭历史记录容器
        /// </summary>
        public virtual void CloseHistory()
        {
            if (m_HistoryContent)
            {
                if(m_HistoryContent.gameObject.activeSelf)
                    m_HistoryContent.gameObject.SetActive(false);
            }
        }
    }
}