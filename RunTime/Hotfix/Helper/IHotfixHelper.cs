using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static HT.Framework.HotfixManager;

namespace HT.Framework
{
    /// <summary>
    /// 热更新管理器的助手接口
    /// </summary>
    public interface IHotfixHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 热更新DLL
        /// </summary>
        TextAsset HotfixDll { get; }
        /// <summary>
        /// 热更新程序集
        /// </summary>
        Assembly HotfixAssembly { get; }
        /// <summary>
        /// 热更新环境
        /// </summary>
        object HotfixEnvironment { get; }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        Dictionary<HotfixMethodType, Dictionary<string, MethodInfo>> FixedMethods { get; }
        /// <summary>
        /// 热修复后的方法
        /// </summary>
        Dictionary<HotfixMethodType, Dictionary<string, Delegate>> FixedDelegates { get; }
        
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        HTFAction FixMethod(HTFAction action);
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        HTFAction<T> FixMethod<T>(HTFAction<T> action);
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        HTFAction<T1, T2> FixMethod<T1, T2>(HTFAction<T1, T2> action);
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        HTFAction<T1, T2, T3> FixMethod<T1, T2, T3>(HTFAction<T1, T2, T3> action);
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        HTFFunc<TResult> FixMethod<TResult>(HTFFunc<TResult> action);
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        HTFFunc<T, TResult> FixMethod<T, TResult>(HTFFunc<T, TResult> action);
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        HTFFunc<T1, T2, TResult> FixMethod<T1, T2, TResult>(HTFFunc<T1, T2, TResult> action);
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        HTFFunc<T1, T2, T3, TResult> FixMethod<T1, T2, T3, TResult>(HTFFunc<T1, T2, T3, TResult> action);
    }
}