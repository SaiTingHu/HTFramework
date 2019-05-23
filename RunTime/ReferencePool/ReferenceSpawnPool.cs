using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 引用池
    /// </summary>
    public sealed class ReferenceSpawnPool : Object
    {
        private int _limit = 100;
        private Queue<IReference> _referenceQueue = new Queue<IReference>();

        public ReferenceSpawnPool(int limit)
        {
            _limit = limit;
        }

        /// <summary>
        /// 引用数量
        /// </summary>
        public int Count
        {
            get
            {
                return _referenceQueue.Count;
            }
        }

        /// <summary>
        /// 生成引用
        /// </summary>
        public T Spawn<T>() where T : class, IReference, new()
        {
            T refe;
            if (_referenceQueue.Count > 0)
            {
                refe = _referenceQueue.Dequeue() as T;
            }
            else
            {
                refe = new T();
            }

            return refe;
        }

        /// <summary>
        /// 回收引用
        /// </summary>
        public void Despawn(IReference refe)
        {
            if (_referenceQueue.Count >= _limit)
            {
                refe = null;
            }
            else
            {
                refe.Reset();
                _referenceQueue.Enqueue(refe);
            }
        }

        /// <summary>
        /// 销毁所有引用
        /// </summary>
        public void Clear(bool destruct)
        {
            _referenceQueue.Clear();

            if (destruct)
            {
                Destroy(this);
            }
        }
    }
}
