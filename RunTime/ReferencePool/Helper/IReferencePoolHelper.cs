using System;
using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 引用池管理器的助手接口
    /// </summary>
    public interface IReferencePoolHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 所有引用池
        /// </summary>
        Dictionary<Type, ReferenceSpawnPool> SpawnPools { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="limit">对象池上限</param>
        void OnInitialization(int limit);
        /// <summary>
        /// 终结
        /// </summary>
        void OnTermination();

        /// <summary>
        /// 获取引用池中引用数量
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <returns>引用数量</returns>
        int GetPoolCount(Type type);
        /// <summary>
        /// 生成引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <returns>对象</returns>
        T Spawn<T>() where T : class, IReference, new();
        /// <summary>
        /// 生成引用
        /// </summary>
        /// <param name="type">引用类型</param>
        /// <returns>对象</returns>
        IReference Spawn(Type type);
        /// <summary>
        /// 回收引用
        /// </summary>
        /// <param name="refe">对象</param>
        void Despawn(IReference refe);
        /// <summary>
        /// 批量回收引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="refes">对象集合</param>
        void Despawns<T>(List<T> refes) where T : class, IReference, new();
        /// <summary>
        /// 批量回收引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="refes">对象数组</param>
        void Despawns<T>(T[] refes) where T : class, IReference, new();
        /// <summary>
        /// 清空指定的引用池
        /// </summary>
        /// <param name="type">引用类型</param>
        void Clear(Type type);
        /// <summary>
        /// 清空所有引用池
        /// </summary>
        void ClearAll();
    }
}