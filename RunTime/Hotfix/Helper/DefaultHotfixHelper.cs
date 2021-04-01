using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static HT.Framework.HotfixManager;

namespace HT.Framework
{
    /// <summary>
    /// 默认的热更新管理器助手
    /// </summary>
    public sealed class DefaultHotfixHelper : IHotfixHelper
    {
        private HotfixManager _module;

        /// <summary>
        /// 热更新管理器
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 热更新DLL
        /// </summary>
        public TextAsset HotfixDll { get; private set; }
        /// <summary>
        /// 热更新程序集
        /// </summary>
        public Assembly HotfixAssembly { get; private set; }
        /// <summary>
        /// 热更新环境
        /// </summary>
        public object HotfixEnvironment { get; private set; }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        public Dictionary<HotfixMethodType, Dictionary<string, MethodInfo>> FixedMethods { get; private set; } = new Dictionary<HotfixMethodType, Dictionary<string, MethodInfo>>();
        /// <summary>
        /// 热修复后的方法
        /// </summary>
        public Dictionary<HotfixMethodType, Dictionary<string, Delegate>> FixedDelegates { get; private set; } = new Dictionary<HotfixMethodType, Dictionary<string, Delegate>>();
        
        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInitialization()
        {
            _module = Module as HotfixManager;

            if (_module.IsEnableHotfix)
            {
                foreach (Enum e in Enum.GetValues(typeof(HotfixMethodType)))
                {
                    FixedMethods.Add((HotfixMethodType)e, new Dictionary<string, MethodInfo>());
                    FixedDelegates.Add((HotfixMethodType)e, new Dictionary<string, Delegate>());
                }
            }
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnPreparatory()
        {
            if (_module.IsEnableHotfix)
            {
                if (Main.m_Resource.Mode == ResourceLoadMode.Resource)
                {
                    throw new HTFrameworkException(HTFrameworkModule.Hotfix, "热更新初始化失败：热更新库不支持使用Resource加载模式！");
                }

                AssetInfo info = new AssetInfo(_module.HotfixDllAssetBundleName, _module.HotfixDllAssetsPath, "");
                Main.m_Resource.LoadAsset<TextAsset>(info, null, HotfixDllLoadDone);
            }
        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnRefresh()
        {

        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTermination()
        {
            HotfixDll = null;
            HotfixAssembly = null;
            HotfixEnvironment = null;
            FixedMethods.Clear();
            FixedDelegates.Clear();
        }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {

        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume()
        {

        }

        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFAction FixMethod(HTFAction action)
        {
            Delegate del = FixMethod(HotfixMethodType.HTFAction, action.Target.GetType().FullName + "." + action.Method.Name, typeof(HTFAction));
            if (del != null) return del as HTFAction;
            else return action;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFAction<T> FixMethod<T>(HTFAction<T> action)
        {
            Delegate del = FixMethod(HotfixMethodType.HTFAction_1Arg, action.Target.GetType().FullName + "." + action.Method.Name, typeof(HTFAction<T>));
            if (del != null) return del as HTFAction<T>;
            else return action;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFAction<T1, T2> FixMethod<T1, T2>(HTFAction<T1, T2> action)
        {
            Delegate del = FixMethod(HotfixMethodType.HTFAction_2Arg, action.Target.GetType().FullName + "." + action.Method.Name, typeof(HTFAction<T1, T2>));
            if (del != null) return del as HTFAction<T1, T2>;
            else return action;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFAction<T1, T2, T3> FixMethod<T1, T2, T3>(HTFAction<T1, T2, T3> action)
        {
            Delegate del = FixMethod(HotfixMethodType.HTFAction_3Arg, action.Target.GetType().FullName + "." + action.Method.Name, typeof(HTFAction<T1, T2, T3>));
            if (del != null) return del as HTFAction<T1, T2, T3>;
            else return action;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFFunc<TResult> FixMethod<TResult>(HTFFunc<TResult> action)
        {
            Delegate del = FixMethod(HotfixMethodType.HTFFunc, action.Target.GetType().FullName + "." + action.Method.Name, typeof(HTFFunc<TResult>));
            if (del != null) return del as HTFFunc<TResult>;
            else return action;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFFunc<T, TResult> FixMethod<T, TResult>(HTFFunc<T, TResult> action)
        {
            Delegate del = FixMethod(HotfixMethodType.HTFFunc_1Arg, action.Target.GetType().FullName + "." + action.Method.Name, typeof(HTFFunc<T, TResult>));
            if (del != null) return del as HTFFunc<T, TResult>;
            else return action;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFFunc<T1, T2, TResult> FixMethod<T1, T2, TResult>(HTFFunc<T1, T2, TResult> action)
        {
            Delegate del = FixMethod(HotfixMethodType.HTFFunc_2Arg, action.Target.GetType().FullName + "." + action.Method.Name, typeof(HTFFunc<T1, T2, TResult>));
            if (del != null) return del as HTFFunc<T1, T2, TResult>;
            else return action;
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFFunc<T1, T2, T3, TResult> FixMethod<T1, T2, T3, TResult>(HTFFunc<T1, T2, T3, TResult> action)
        {
            Delegate del = FixMethod(HotfixMethodType.HTFFunc_3Arg, action.Target.GetType().FullName + "." + action.Method.Name, typeof(HTFFunc<T1, T2, T3, TResult>));
            if (del != null) return del as HTFFunc<T1, T2, T3, TResult>;
            else return action;
        }

        private Delegate FixMethod(HotfixMethodType methodType, string targetName, Type type)
        {
            if (!_module.IsEnableHotfix)
            {
                return null;
            }

            if (FixedDelegates[methodType].ContainsKey(targetName))
            {
                return FixedDelegates[methodType][targetName];
            }
            else
            {
                if (FixedMethods[methodType].ContainsKey(targetName))
                {
                    Delegate del = Delegate.CreateDelegate(type, FixedMethods[methodType][targetName]);
                    FixedDelegates[methodType].Add(targetName, del);
                    return del;
                }
                else
                {
                    return null;
                }
            }
        }
        private void HotfixDllLoadDone(TextAsset asset)
        {
            HotfixDll = asset;
            HotfixAssembly = Assembly.Load(HotfixDll.bytes, null);
            HotfixEnvironment = HotfixAssembly.CreateInstance("HotfixEnvironment");

            if (HotfixEnvironment == null)
            {
                throw new HTFrameworkException(HTFrameworkModule.Hotfix, "热更新初始化失败：热更新库中不存在热更新环境 HotfixEnvironment！");
            }

            SearchHotfixMethod();

            Main.m_Event.Throw<EventHotfixReady>();
        }
        private void SearchHotfixMethod()
        {
            Type[] types = HotfixAssembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                MethodInfo[] methods = types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                for (int j = 0; j < methods.Length; j++)
                {
                    HotfixMethodAttribute att = methods[j].GetCustomAttribute<HotfixMethodAttribute>();
                    if (att != null)
                    {
                        HotfixMethodType methodType = GetHotfixMethodType(methods[j]);
                        if (!FixedMethods[methodType].ContainsKey(att.TargetName))
                        {
                            FixedMethods[methodType].Add(att.TargetName, methods[j]);
                        }
                    }
                }
            }

            if (FixedMethods[HotfixMethodType.Invalid].Count > 0)
            {
                foreach (var item in FixedMethods[HotfixMethodType.Invalid])
                {
                    Log.Error("发现无效的热修复方法：" + item.Value.Name);
                }
                FixedMethods[HotfixMethodType.Invalid].Clear();
            }
        }
        private HotfixMethodType GetHotfixMethodType(MethodInfo method)
        {
            bool isVoid = method.ReturnType.Name == "Void";
            ParameterInfo[] pis = method.GetParameters();

            if (isVoid)
            {
                switch (pis.Length)
                {
                    case 0:
                        return HotfixMethodType.HTFAction;
                    case 1:
                        return HotfixMethodType.HTFAction_1Arg;
                    case 2:
                        return HotfixMethodType.HTFAction_2Arg;
                    case 3:
                        return HotfixMethodType.HTFAction_3Arg;
                    default:
                        return HotfixMethodType.Invalid;
                }
            }
            else
            {
                switch (pis.Length)
                {
                    case 0:
                        return HotfixMethodType.HTFFunc;
                    case 1:
                        return HotfixMethodType.HTFFunc_1Arg;
                    case 2:
                        return HotfixMethodType.HTFFunc_2Arg;
                    case 3:
                        return HotfixMethodType.HTFFunc_3Arg;
                    default:
                        return HotfixMethodType.Invalid;
                }
            }
        }
    }
}