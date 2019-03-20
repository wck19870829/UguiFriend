using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 可以序列化的哈希集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SerializableHashSet<T> : HashSet<T>,
        ISerializationCallbackReceiver
    {
        [SerializeField] List<T> values;

        public SerializableHashSet()
        {
            values = new List<T>();
        }

        public void OnAfterDeserialize()
        {
            Clear();
            foreach (var value in values)
            {
                if (!Contains(value))
                {
                    Add(value);
                }
            }
        }

        public void OnBeforeSerialize()
        {
            values.Clear();
            foreach (var value in this)
            {
                values.Add(value);
            }
        }
    }
}