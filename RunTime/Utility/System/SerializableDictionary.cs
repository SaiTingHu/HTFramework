using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 可序列化的字典
    /// </summary>
    [Serializable]
    public sealed class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        [SerializeField] private List<TKey> _keys = new List<TKey>();
        [SerializeField] private List<TValue> _values = new List<TValue>();
        private bool _isFillDictionary = false;
        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        /// <summary>
        /// 通过Key获取Value
        /// </summary>
        public TValue this[TKey key]
        {
            get
            {
                if (IsRuntimeMode)
                {
                    FillDictionary();
                    return _dictionary[key];
                }
                else
                {
                    int index = _keys.IndexOf(key);
                    return _values[index];
                }
            }
            set
            {
                if (IsRuntimeMode)
                {
                    FillDictionary();
                    _dictionary[key] = value;

#if UNITY_EDITOR
                    int index = _keys.IndexOf(key);
                    _values[index] = value;
#endif
                }
                else
                {
                    int index = _keys.IndexOf(key);
                    _values[index] = value;
                }
            }
        }
        /// <summary>
        /// 所有的Key
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                if (IsRuntimeMode)
                {
                    FillDictionary();
                    return _dictionary.Keys;
                }
                else
                {
                    return _keys;
                }
            }
        }
        /// <summary>
        /// 所有的Value
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                if (IsRuntimeMode)
                {
                    FillDictionary();
                    return _dictionary.Values;
                }
                else
                {
                    return _values;
                }
            }
        }
        /// <summary>
        /// 字典中元素的数量
        /// </summary>
        public int Count
        {
            get
            {
                if (IsRuntimeMode)
                {
                    FillDictionary();
                    return _dictionary.Count;
                }
                else
                {
                    return _keys.Count;
                }
            }
        }
        /// <summary>
        /// 是否为只读
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 是否为运行时模式
        /// </summary>
        private bool IsRuntimeMode
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorApplication.isPlaying;
#else
                return true;
#endif
            }
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            if (IsRuntimeMode)
            {
                FillDictionary();

                if (_dictionary.ContainsKey(key))
                {
                    throw new HTFrameworkException(HTFrameworkModule.Utility, $"可序列化的字典：添加数据失败，字典中已存在该 Key：{key}。");
                }

                _dictionary.Add(key, value);
#if UNITY_EDITOR
                _keys.Add(key);
                _values.Add(value);
#endif
            }
            else
            {
                if (_keys.Contains(key))
                {
                    throw new HTFrameworkException(HTFrameworkModule.Utility, $"可序列化的字典：添加数据失败，字典中已存在该 Key：{key}。");
                }

                _keys.Add(key);
                _values.Add(value);
            }
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <returns>移除数据是否成功</returns>
        public bool Remove(TKey key)
        {
            if (IsRuntimeMode)
            {
                FillDictionary();

                if (!_dictionary.ContainsKey(key))
                {
                    throw new HTFrameworkException(HTFrameworkModule.Utility, $"可序列化的字典：移除数据失败，字典中不存在该 Key：{key}。");
                }

#if UNITY_EDITOR
                _keys.Remove(key);
                _values.Remove(_dictionary[key]);
#endif
                return _dictionary.Remove(key);
            }
            else
            {
                if (!_keys.Contains(key))
                {
                    throw new HTFrameworkException(HTFrameworkModule.Utility, $"可序列化的字典：移除数据失败，字典中不存在该 Key：{key}。");
                }

                int index = _keys.IndexOf(key);
                _keys.RemoveAt(index);
                _values.RemoveAt(index);
                return true;
            }
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <returns>移除数据是否成功</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }
        /// <summary>
        /// 尝试获取数据
        /// </summary>
        /// <returns>获取数据是否成功</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (IsRuntimeMode)
            {
                FillDictionary();

                return _dictionary.TryGetValue(key, out value);
            }
            else
            {
                int index = _keys.IndexOf(key);
                if (index != -1)
                {
                    value = _values[index];
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }
        }
        /// <summary>
        /// 是否存在指定的数据Key
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            if (IsRuntimeMode)
            {
                FillDictionary();

                return _dictionary.ContainsKey(key);
            }
            else
            {
                return _keys.Contains(key);
            }
        }
        /// <summary>
        /// 是否存在指定的数据
        /// </summary>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }
        /// <summary>
        /// 拷贝数据
        /// </summary>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (IsRuntimeMode)
            {
                FillDictionary();

                ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的字典：CopyTo 方法只支持在运行时调用。");
            }
        }
        /// <summary>
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            _keys.Clear();
            _values.Clear();
            _dictionary.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (IsRuntimeMode)
            {
                FillDictionary();

                return _dictionary.GetEnumerator();
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的字典：GetEnumerator 方法只支持在运行时调用。");
            }
        }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (IsRuntimeMode)
            {
                FillDictionary();

                return _dictionary.GetEnumerator();
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的字典：GetEnumerator 方法只支持在运行时调用。");
            }
        }

        private void FillDictionary()
        {
            if (_isFillDictionary)
                return;

            if (_keys.Count != _values.Count)
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的字典：字典初始化失败，序列化数据存在异常，请清空数据重新添加！");
            }

            _dictionary.Clear();
            for (int i = 0; i < _keys.Count; i++)
            {
                if (!_dictionary.ContainsKey(_keys[i]))
                {
                    _dictionary.Add(_keys[i], _values[i]);
                }
            }
            _isFillDictionary = true;
        }
    }
}