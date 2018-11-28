using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// MonoBehaviour单例模版
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UguiSingleton<T>: MonoBehaviour
        where T:MonoBehaviour,IUguiSingletonCreate<T>
    {
        static volatile T s_Instance;
        static object syncRoot = new object();

        private void OnDestroy()
        {
            s_Instance = null;
        }

        /// <summary>
        /// 获取单例
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (s_Instance == null)
                    {
                        var instances = FindObjectsOfType<T>();
                        if (instances.Length > 0)
                        {
                            s_Instance = instances[0];
                            for (var i = 1; i < instances.Length; i++)
                            {
                                Destroy(instances[i].gameObject);
                            }
                        }
                        if (s_Instance == null)
                        {
                            var go = new GameObject("[" + typeof(T).Name + "]");
                            s_Instance = go.AddComponent<T>();
                            s_Instance.OnSingletonCreate(s_Instance);
                        }
                    }

                    return s_Instance;
                }
            }
        }
    }

    /// <summary>
    /// 单例创建接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUguiSingletonCreate<T>
        where T : MonoBehaviour
    {
        void OnSingletonCreate(T instance);
    }
}