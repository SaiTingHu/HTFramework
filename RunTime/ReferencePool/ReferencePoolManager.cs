using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 引用池管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ReferencePoolManager : ModuleManager
    {
        /// <summary>
        /// 单个引用池上限
        /// </summary>
        public int Limit = 100;

        private Dictionary<Type, ReferenceSpawnPool> _spawnPools = new Dictionary<Type, ReferenceSpawnPool>();

        /// <summary>
        /// 所有引用池
        /// </summary>
        public Dictionary<Type, ReferenceSpawnPool> SpawnPools
        {
            get
            {
                return _spawnPools;
            }
        }
        
        /// <summary>
        /// 生成引用
        /// </summary>
        public T Spawn<T>() where T : class, IReference, new()
        {
            if (!_spawnPools.ContainsKey(typeof(T)))
            {
                _spawnPools.Add(typeof(T), new ReferenceSpawnPool(Limit));
            }

            return _spawnPools[typeof(T)].Spawn<T>();
        }

        /// <summary>
        /// 生成引用
        /// </summary>
        public IReference Spawn(Type type)
        {
            if (!_spawnPools.ContainsKey(type))
            {
                _spawnPools.Add(type, new ReferenceSpawnPool(Limit));
            }

            return _spawnPools[type].Spawn(type);
        }

        /// <summary>
        /// 回收引用
        /// </summary>
        public void Despawn(IReference refe)
        {
            Type type = refe.GetType();
            if (!_spawnPools.ContainsKey(type))
            {
                _spawnPools.Add(type, new ReferenceSpawnPool(Limit));
            }

            _spawnPools[type].Despawn(refe);
        }

        /// <summary>
        /// 批量回收引用
        /// </summary>
        public void Despawns<T>(List<T> refes) where T : IReference
        {
            Type type = typeof(T);
            if (!_spawnPools.ContainsKey(type))
            {
                _spawnPools.Add(type, new ReferenceSpawnPool(Limit));
            }

            for (int i = 0; i < refes.Count; i++)
            {
                _spawnPools[type].Despawn(refes[i]);
            }
            refes.Clear();
        }

        /// <summary>
        /// 批量回收引用
        /// </summary>
        public void Despawns<T>(T[] refes) where T : IReference
        {
            Type type = typeof(T);
            if (!_spawnPools.ContainsKey(type))
            {
                _spawnPools.Add(type, new ReferenceSpawnPool(Limit));
            }

            for (int i = 0; i < refes.Length; i++)
            {
                _spawnPools[type].Despawn(refes[i]);
            }
        }

        /// <summary>
        /// 销毁所有引用池
        /// </summary>
        public void Clear()
        {
            foreach (KeyValuePair<Type, ReferenceSpawnPool> spawnPool in _spawnPools)
            {
                spawnPool.Value.Clear(true);
            }
            _spawnPools.Clear();
        }

        public override void Termination()
        {
            base.Termination();

            Clear();
        }
    }
}
