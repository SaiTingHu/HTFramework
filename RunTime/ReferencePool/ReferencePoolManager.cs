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
        /// 单个引用池上限【请勿在代码中修改】
        /// </summary>
        public int Limit = 100;

        private Dictionary<Type, ReferenceSpawnPool> _spawnPools = new Dictionary<Type, ReferenceSpawnPool>();

        public override void OnTermination()
        {
            base.OnTermination();

            Clear();
        }

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
        /// 获取引用池中引用数量
        /// </summary>
        /// <param name="name">引用类型</param>
        /// <returns>对象数量</returns>
        public int GetPoolCount<T>() where T : class, IReference, new()
        {
            if (_spawnPools.ContainsKey(typeof(T)))
            {
                return _spawnPools[typeof(T)].Count;
            }
            else
            {
                GlobalTools.LogError(string.Format("获取引用数量失败：不存在引用池 {0} ！", name));
                return -1;
            }
        }
        /// <summary>
        /// 生成引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <returns>对象</returns>
        public T Spawn<T>() where T : class, IReference, new()
        {
            return Spawn(typeof(T)) as T;
        }
        /// <summary>
        /// 生成引用
        /// </summary>
        /// <param name="type">引用类型</param>
        /// <returns>对象</returns>
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
        /// <param name="refe">对象</param>
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
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="refes">对象集合</param>
        public void Despawns<T>(List<T> refes) where T : class, IReference, new()
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
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="refes">对象数组</param>
        public void Despawns<T>(T[] refes) where T : class, IReference, new()
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
        /// 清空指定的引用池
        /// </summary>
        /// <param name="type">引用类型</param>
        public void Clear(Type type)
        {
            if (_spawnPools.ContainsKey(type))
            {
                _spawnPools[type].Clear();
            }
        }
        /// <summary>
        /// 清空所有引用池
        /// </summary>
        public void Clear()
        {
            foreach (var spawnPool in _spawnPools)
            {
                spawnPool.Value.Clear();
            }
        }
    }
}