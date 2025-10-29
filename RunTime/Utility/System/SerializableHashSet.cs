using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 可序列化的哈希集
    /// </summary>
    [Serializable]
    public sealed class SerializableHashSet<T> : ISet<T>
    {
        [SerializeField] private List<T> _values = new List<T>();
        [NonSerialized] private bool _isFillHashSet = false;
        private HashSet<T> _hashSet = new HashSet<T>();

        /// <summary>
        /// 哈希集中元素的数量
        /// </summary>
        public int Count
        {
            get
            {
                if (IsRuntimeMode)
                {
                    FillHashSet();
                    return _hashSet.Count;
                }
                else
                {
                    return _values.Count;
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
        /// <returns>添加数据是否成功</returns>
        public bool Add(T item)
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

#if UNITY_EDITOR
                if (!_values.Contains(item)) _values.Add(item);
#endif
                return _hashSet.Add(item);
            }
            else
            {
                if (!_values.Contains(item))
                {
                    _values.Add(item);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        void ICollection<T>.Add(T item)
        {
            Add(item);
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <returns>移除数据是否成功</returns>
        public bool Remove(T item)
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

#if UNITY_EDITOR
                if (_values.Contains(item)) _values.Remove(item);
#endif
                return _hashSet.Remove(item);
            }
            else
            {
                if (_values.Contains(item))
                {
                    _values.Remove(item);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 是否存在指定的数据
        /// </summary>
        public bool Contains(T item)
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

                return _hashSet.Contains(item);
            }
            else
            {
                return _values.Contains(item);
            }
        }
        /// <summary>
        /// 拷贝数据
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

                _hashSet.CopyTo(array, arrayIndex);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的哈希集：CopyTo 方法只支持在运行时调用。");
            }
        }
        /// <summary>
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            _values.Clear();
            _hashSet.Clear();
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

                _hashSet.ExceptWith(other);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的哈希集：ExceptWith 方法只支持在运行时调用。");
            }
        }
        public void IntersectWith(IEnumerable<T> other)
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

                _hashSet.IntersectWith(other);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的哈希集：IntersectWith 方法只支持在运行时调用。");
            }
        }
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

                return _hashSet.IsProperSubsetOf(other);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的哈希集：IsProperSubsetOf 方法只支持在运行时调用。");
            }
        }
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

                return _hashSet.IsProperSupersetOf(other);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的哈希集：IsProperSupersetOf 方法只支持在运行时调用。");
            }
        }
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

                return _hashSet.IsSubsetOf(other);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的哈希集：IsSubsetOf 方法只支持在运行时调用。");
            }
        }
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

                return _hashSet.IsSupersetOf(other);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的哈希集：IsSupersetOf 方法只支持在运行时调用。");
            }
        }
        public bool Overlaps(IEnumerable<T> other)
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

                return _hashSet.Overlaps(other);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的哈希集：Overlaps 方法只支持在运行时调用。");
            }
        }
        public bool SetEquals(IEnumerable<T> other)
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

                return _hashSet.SetEquals(other);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的哈希集：SetEquals 方法只支持在运行时调用。");
            }
        }
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

                _hashSet.SymmetricExceptWith(other);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的哈希集：SymmetricExceptWith 方法只支持在运行时调用。");
            }
        }
        public void UnionWith(IEnumerable<T> other)
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

                _hashSet.UnionWith(other);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的哈希集：UnionWith 方法只支持在运行时调用。");
            }
        }
        public IEnumerator<T> GetEnumerator()
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

                return _hashSet.GetEnumerator();
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的哈希集：GetEnumerator 方法只支持在运行时调用。");
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (IsRuntimeMode)
            {
                FillHashSet();

                return _hashSet.GetEnumerator();
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, "可序列化的哈希集：GetEnumerator 方法只支持在运行时调用。");
            }
        }

        private void FillHashSet()
        {
            if (_isFillHashSet)
                return;

            _hashSet.Clear();
            for (int i = 0; i < _values.Count; i++)
            {
                _hashSet.Add(_values[i]);
            }
            _isFillHashSet = true;
        }
    }
}