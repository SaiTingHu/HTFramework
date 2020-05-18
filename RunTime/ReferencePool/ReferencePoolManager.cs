using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 引用池管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.ReferencePool)]
    public sealed class ReferencePoolManager : InternalModuleBase
    {
        /// <summary>
        /// 单个引用池上限【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal int Limit = 100;

        /// <summary>
        /// 所有引用池
        /// </summary>
        internal Dictionary<Type, ReferenceSpawnPool> SpawnPools { get; private set; } = new Dictionary<Type, ReferenceSpawnPool>();

        internal override void OnTermination()
        {
            base.OnTermination();

            ClearAll();
        }
        
        /// <summary>
        /// 获取引用池中引用数量
        /// </summary>
        /// <param name="name">引用类型</param>
        /// <returns>对象数量</returns>
        public int GetPoolCount<T>() where T : class, IReference, new()
        {
            if (SpawnPools.ContainsKey(typeof(T)))
            {
                return SpawnPools[typeof(T)].Count;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.ReferencePool, "获取引用数量失败：不存在引用池 " + name + " ！");
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
            if (!SpawnPools.ContainsKey(type))
            {
                SpawnPools.Add(type, new ReferenceSpawnPool(Limit));
            }
            return SpawnPools[type].Spawn(type);
        }
        /// <summary>
        /// 回收引用
        /// </summary>
        /// <param name="refe">对象</param>
        public void Despawn(IReference refe)
        {
            Type type = refe.GetType();
            if (!SpawnPools.ContainsKey(type))
            {
                SpawnPools.Add(type, new ReferenceSpawnPool(Limit));
            }

            SpawnPools[type].Despawn(refe);
        }
        /// <summary>
        /// 批量回收引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="refes">对象集合</param>
        public void Despawns<T>(List<T> refes) where T : class, IReference, new()
        {
            Type type = typeof(T);
            if (!SpawnPools.ContainsKey(type))
            {
                SpawnPools.Add(type, new ReferenceSpawnPool(Limit));
            }

            for (int i = 0; i < refes.Count; i++)
            {
                SpawnPools[type].Despawn(refes[i]);
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
            if (!SpawnPools.ContainsKey(type))
            {
                SpawnPools.Add(type, new ReferenceSpawnPool(Limit));
            }

            for (int i = 0; i < refes.Length; i++)
            {
                SpawnPools[type].Despawn(refes[i]);
            }
        }
        /// <summary>
        /// 清空指定的引用池
        /// </summary>
        /// <param name="type">引用类型</param>
        public void Clear(Type type)
        {
            if (SpawnPools.ContainsKey(type))
            {
                SpawnPools[type].Clear();
            }
        }
        /// <summary>
        /// 清空所有引用池
        /// </summary>
        public void ClearAll()
        {
            foreach (var spawnPool in SpawnPools)
            {
                spawnPool.Value.Clear();
            }
        }
    }
}