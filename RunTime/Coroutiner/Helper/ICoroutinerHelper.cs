using System;
using System.Collections.Generic;
using static HT.Framework.Coroutiner;

namespace HT.Framework
{
    /// <summary>
    /// 协程调度器的助手接口
    /// </summary>
    public interface ICoroutinerHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 所有协程迭代器
        /// </summary>
        Dictionary<string, CoroutineEnumerator> CoroutineEnumerators { get; }
        /// <summary>
        /// 迭代器仓库
        /// </summary>
        Dictionary<Delegate, List<CoroutineEnumerator>> Warehouse { get; }

        /// <summary>
        /// 运行协程
        /// </summary>
        /// <param name="action">协程方法</param>
        /// <returns>协程迭代器ID</returns>
        string Run(CoroutineAction action);
        /// <summary>
        /// 运行协程
        /// </summary>
        /// <typeparam name="T">协程方法的参数类型</typeparam>
        /// <param name="action">协程方法</param>
        /// <param name="arg">协程方法的参数</param>
        /// <returns>协程迭代器ID</returns>
        string Run<T>(CoroutineAction<T> action, T arg);
        /// <summary>
        /// 运行协程
        /// </summary>
        /// <typeparam name="T1">协程方法的参数类型</typeparam>
        /// <typeparam name="T2">协程方法的参数类型</typeparam>
        /// <param name="action">协程方法</param>
        /// <param name="arg1">协程方法的参数</param>
        /// <param name="arg2">协程方法的参数</param>
        /// <returns>协程迭代器ID</returns>
        string Run<T1, T2>(CoroutineAction<T1, T2> action, T1 arg1, T2 arg2);
        /// <summary>
        /// 重启协程
        /// </summary>
        /// <param name="id">协程迭代器ID</param>
        void Rerun(string id);
        /// <summary>
        /// 终止指定ID的协程
        /// </summary>
        /// <param name="id">协程迭代器ID</param>
        void Stop(string id);
        /// <summary>
        /// 终止指定类型的所有协程
        /// </summary>
        /// <param name="action">协程方法</param>
        void Stop(Delegate action);
        /// <summary>
        /// 是否存在指定ID的协程
        /// </summary>
        /// <param name="id">协程迭代器ID</param>
        /// <returns>是否存在</returns>
        bool IsExist(string id);
        /// <summary>
        /// 指定ID的协程是否运行中
        /// </summary>
        /// <param name="id">协程迭代器ID</param>
        /// <returns>是否运行中</returns>
        bool IsRunning(string id);
        /// <summary>
        /// 清理所有未运行的协程
        /// </summary>
        void ClearNotRunning();
    }
}