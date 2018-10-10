using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 显示对象基类
    /// </summary>
    public abstract class UguiObject : MonoBehaviour
    {
        const string openAnim = "Open";
        const string closeAnim = "Close";
        const string refreshViewFunc = "RefreshViewDelay";
        static readonly Dictionary<Type, UguiBindingAttribute> bindingDict;           //绑定信息

        protected UguiObjectData m_Data;
        internal bool isOpen;
        Animator m_Anim;

        static UguiObject()
        {
            bindingDict = new Dictionary<Type, UguiBindingAttribute>();

            var bindingType = typeof(UguiBindingAttribute);
            var baseType = typeof(UguiObjectData);
            foreach (var type in baseType.Assembly.GetTypes())
            {
                if (type.IsAbstract) continue;

                if (baseType.IsAssignableFrom(type))
                {
                    var customAtt = type.GetCustomAttributes(bindingType, false);
                    if (customAtt.Length == 0)
                    {
                        Debug.LogErrorFormat("ui数据未绑定元素！   {0}", type);
                    }
                    else if (customAtt.Length == 1)
                    {
                        bindingDict.Add(type, (UguiBindingAttribute)customAtt[0]);
                    }
                    else
                    {
                        Debug.LogErrorFormat("ui数据绑定元素数量错误！  {0}", type);
                    }
                }
            }
        }

        #region Mono运行时序
        protected virtual void Awake()
        {

        }

        protected virtual void OnEnable()
        {
            isOpen = true;
        }

        protected virtual void Start()
        {

        }

        protected virtual void OnDisable()
        {
            isOpen = false;
        }

        protected virtual void OnDestroy()
        {

        }
        #endregion

        public void Open()
        {
            isOpen = true;
            PlayAnim(openAnim);
        }

        public void Close()
        {
            isOpen = false;
            PlayAnim(closeAnim);
        }

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="state"></param>
        public virtual void Open(UguiObjectData state)
        {
            Data = state;
            Open();
        }

        public void PlayAnim(string anim)
        {
            if (m_Anim == null) m_Anim = GetComponent<Animator>();
            if (m_Anim == null) return;

            m_Anim.Play(anim, 0, 0);
        }

        public UguiObjectData Data
        {
            get
            {
                return m_Data;
            }
            set
            {
                SetData(value);
            }
        }

        protected virtual bool SetData(UguiObjectData value)
        {
            if (value == null)
            {
                Debug.LogErrorFormat("不支持Data为空!");
                return false;
            }
            if (GetBindingInfo(value).uiType != this.GetType())
            {
                Debug.LogErrorFormat("赋值的数据类型应为ui元素绑定的数据类型! UI:{0}  Data:{1}", this, value);
                return false;
            }

            m_Data = value;
            SetDirty();

            return true;
        }

        public T GetData<T>() where T : UguiObjectData
        {
            if (m_Data == null) return null;

            return m_Data as T;
        }

        /// <summary>
        /// 获取快照
        /// </summary>
        /// <returns></returns>
        public virtual UguiObjectData StateSnapshot()
        {
            if (m_Data != null)
            {
                var data = m_Data.DeepClone();
                return data;
            }

            return null;
        }

        /// <summary>
        /// 刷新视图
        /// </summary>
        /// <param name="immediately"></param>
        public void SetDirty(bool immediatelyRefresh = false)
        {
            if (immediatelyRefresh)
            {
                RefreshView();
            }
            else
            {
                StopCoroutine(refreshViewFunc);
                StartCoroutine(refreshViewFunc);
            }
        }

        IEnumerator RefreshViewDelay()
        {
            yield return new WaitForEndOfFrame();
            RefreshView();
        }

        /// <summary>
        /// 子类重写此方法刷新视图
        /// </summary>
        protected abstract void RefreshView();

        #region 静态方法

        /// <summary>
        /// 由数据获取对应元素的实例化预设
        /// </summary>
        /// <param name="data"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static UguiObject GetInstanceElementByData(UguiObjectData data, Transform parent = null)
        {
            if (data == null)
            {
                Debug.LogError("Data为null");
                return null;
            }

            if (bindingDict.ContainsKey(data.GetType()))
            {
                var bindingInfo = bindingDict[data.GetType()];
                if (!string.IsNullOrEmpty(bindingInfo.prefabPath))
                {
                    var source = data.GetUIPrefabSource();
                    var clone = GameObject.Instantiate<UguiObject>(source);
                    if (parent != null) clone.transform.SetParent(parent);
                    clone.Data = data;
                    clone.transform.localPosition = Vector3.zero;
                    clone.transform.localRotation = Quaternion.identity;
                    clone.transform.localScale = Vector3.one;

                    return clone;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取Data上绑定数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static UguiBindingAttribute GetBindingInfo(UguiObjectData data)
        {
            if (bindingDict.ContainsKey(data.GetType()))
            {
                return bindingDict[data.GetType()];
            }

            return null;
        }

        #endregion
    }

    [Serializable]
    /// <summary>
    /// UI元素数据基类
    /// </summary>
    public abstract class UguiObjectData
    {
        public long id;

        public UguiObjectData()
        {

        }

        public UguiObjectData(long id)
            : this()
        {
            this.id = id;
        }

        public virtual UguiObjectData DeepClone()
        {
            return this.MemberwiseClone() as UguiObjectData;
        }

        /// <summary>
        /// 获取UI元素
        /// 默认使用Resources加载，如使用其他加载方式重写此方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual UguiObject GetUIPrefabSource()
        {
            var bindInfo = UguiObject.GetBindingInfo(this);

            return Resources.Load<UguiObject>(bindInfo.prefabPath);
        }
    }
}