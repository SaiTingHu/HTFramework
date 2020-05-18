using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 热更新管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.Hotfix)]
    public sealed class HotfixManager : InternalModuleBase
    {
        /// <summary>
        /// 是否启用热更新【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsEnableHotfix = false;
        /// <summary>
        /// 热更新库文件AB包名称【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string HotfixDllAssetBundleName = "hotfix";
        /// <summary>
        /// 热更新库文件路径【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string HotfixDllAssetsPath = "Assets/Hotfix/Hotfix.dll.bytes";
        /// <summary>
        /// 执行热更新逻辑事件
        /// </summary>
        internal event HTFAction UpdateHotfixLogicEvent;

        private TextAsset _hotfixDll;
        private Assembly _hotfixAssembly;
        private object _hotfixEnvironment;
        private Dictionary<HotfixMethodType, Dictionary<string, MethodInfo>> _fixedMethods = new Dictionary<HotfixMethodType, Dictionary<string, MethodInfo>>();
        private Dictionary<HotfixMethodType, Dictionary<string, Delegate>> _fixedDelegates = new Dictionary<HotfixMethodType, Dictionary<string, Delegate>>();

        /// <summary>
        /// 当前已加载的热更新程序集
        /// </summary>
        public Assembly HotfixAssembly
        {
            get
            {
                return _hotfixAssembly;
            }
        }
        /// <summary>
        /// 当前的热更新环境
        /// </summary>
        public object HotfixEnvironment
        {
            get
            {
                return _hotfixEnvironment;
            }
        }

        internal override void OnInitialization()
        {
            base.OnInitialization();

            if (IsEnableHotfix)
            {
                foreach (Enum e in Enum.GetValues(typeof(HotfixMethodType)))
                {
                    _fixedMethods.Add((HotfixMethodType)e, new Dictionary<string, MethodInfo>());
                    _fixedDelegates.Add((HotfixMethodType)e, new Dictionary<string, Delegate>());
                }
            }
        }
        internal override void OnPreparatory()
        {
            base.OnPreparatory();

            if (IsEnableHotfix)
            {
                if (Main.m_Resource.Mode == ResourceLoadMode.Resource)
                {
                    throw new HTFrameworkException(HTFrameworkModule.Hotfix, "热更新初始化失败：热更新库不支持使用Resource加载模式！");
                }

                AssetInfo info = new AssetInfo(HotfixDllAssetBundleName, HotfixDllAssetsPath, "");
                Main.m_Resource.LoadAsset<TextAsset>(info, null, HotfixDllLoadDone);
            }
        }
        internal override void OnRefresh()
        {
            base.OnRefresh();

            if (IsEnableHotfix)
            {
                UpdateHotfixLogicEvent?.Invoke();
            }
        }
        internal override void OnTermination()
        {
            base.OnTermination();

            _hotfixDll = null;
            _hotfixAssembly = null;
            _hotfixEnvironment = null;
            _fixedMethods.Clear();
            _fixedDelegates.Clear();
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
            if (!IsEnableHotfix)
            {
                return null;
            }

            if (_fixedDelegates[methodType].ContainsKey(targetName))
            {
                return _fixedDelegates[methodType][targetName];
            }
            else
            {
                if (_fixedMethods[methodType].ContainsKey(targetName))
                {
                    Delegate del = Delegate.CreateDelegate(type, _fixedMethods[methodType][targetName]);
                    _fixedDelegates[methodType].Add(targetName, del);
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
            _hotfixDll = asset;
            _hotfixAssembly = Assembly.Load(_hotfixDll.bytes, null);
            _hotfixEnvironment = _hotfixAssembly.CreateInstance("HotfixEnvironment");

            if (_hotfixEnvironment == null)
            {
                throw new HTFrameworkException(HTFrameworkModule.Hotfix, "热更新初始化失败：热更新库中不存在热更新环境 HotfixEnvironment！");
            }

            SearchHotfixMethod();

            Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventHotfixReady>());
        }
        private void SearchHotfixMethod()
        {
            Type[] types = _hotfixAssembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                MethodInfo[] methods = types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                for (int j = 0; j < methods.Length; j++)
                {
                    HotfixMethodAttribute att = methods[j].GetCustomAttribute<HotfixMethodAttribute>();
                    if (att != null)
                    {
                        HotfixMethodType methodType = GetHotfixMethodType(methods[j]);
                        if (!_fixedMethods[methodType].ContainsKey(att.TargetName))
                        {
                            _fixedMethods[methodType].Add(att.TargetName, methods[j]);
                        }
                    }
                }
            }

            if (_fixedMethods[HotfixMethodType.Invalid].Count > 0)
            {
                foreach (var item in _fixedMethods[HotfixMethodType.Invalid])
                {
                    GlobalTools.LogError("发现无效的热修复方法：" + item.Value.Name);
                }
                _fixedMethods[HotfixMethodType.Invalid].Clear();
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

        /// <summary>
        /// 热修复方法的类型
        /// </summary>
        internal enum HotfixMethodType
        {
            Invalid,
            HTFAction,
            HTFAction_1Arg,
            HTFAction_2Arg,
            HTFAction_3Arg,
            HTFFunc,
            HTFFunc_1Arg,
            HTFFunc_2Arg,
            HTFFunc_3Arg
        }
    }
}