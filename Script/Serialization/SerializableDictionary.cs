using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 可以序列化的字典
    /// </summary>
    public class SerializableDictionary<TKey,TValue> :Dictionary<TKey,TValue>,
        ISerializationCallbackReceiver
    {
        [SerializeField] List<TKey> keys;
        [SerializeField] List<TValue> values;

        public SerializableDictionary()
        {
            keys = new List<TKey>();
            values = new List<TValue>();
        }

        public void OnAfterDeserialize()
        {
            Clear();
            var count = Mathf.Min(keys.Count,values.Count);
            for (var i=0;i<count;i++)
            {
                Add(keys[i],values[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            foreach (var key in Keys)
            {
                keys.Add(key);
            }
            values.Clear();
            foreach (var value in Values)
            {
                values.Add(value);
            }
        }
    }
}
