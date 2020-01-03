using System;
using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 引用池
    /// </summary>
    public sealed class ReferenceSpawnPool
    {
        private int _limit = 100;
        private Queue<IReference> _referenceQueue = new Queue<IReference>();

        internal ReferenceSpawnPool(int limit)
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
        /// <param name="type">引用类型</param>
        /// <returns>对象</returns>
        public IReference Spawn(Type type)
        {
            IReference refe;
            if (_referenceQueue.Count > 0)
            {
                refe = _referenceQueue.Dequeue();
            }
            else
            {
                refe = Activator.CreateInstance(type) as IReference;
            }

            return refe;
        }

        /// <summary>
        /// 回收引用
        /// </summary>
        /// <param name="refe">对象</param>
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
        /// 清空所有引用
        /// </summary>
        public void Clear()
        {
            _referenceQueue.Clear();
        }
    }
}