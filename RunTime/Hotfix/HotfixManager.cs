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

        private IHotfixHelper _helper;
        
        /// <summary>
        /// 当前已加载的热更新程序集
        /// </summary>
        public Assembly HotfixAssembly
        {
            get
            {
                return _helper.HotfixAssembly;
            }
        }
        /// <summary>
        /// 当前的热更新环境
        /// </summary>
        public object HotfixEnvironment
        {
            get
            {
                return _helper.HotfixEnvironment;
            }
        }

        private HotfixManager()
        {

        }
        internal override void OnInitialization()
        {
            base.OnInitialization();

            _helper = Helper as IHotfixHelper;
        }
        internal override void OnRefresh()
        {
            base.OnRefresh();

            if (IsEnableHotfix)
            {
                UpdateHotfixLogicEvent?.Invoke();
            }
        }

        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFAction FixMethod(HTFAction action)
        {
            return _helper.FixMethod(action);
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFAction<T> FixMethod<T>(HTFAction<T> action)
        {
            return _helper.FixMethod(action);
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFAction<T1, T2> FixMethod<T1, T2>(HTFAction<T1, T2> action)
        {
            return _helper.FixMethod(action);
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFAction<T1, T2, T3> FixMethod<T1, T2, T3>(HTFAction<T1, T2, T3> action)
        {
            return _helper.FixMethod(action);
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFFunc<TResult> FixMethod<TResult>(HTFFunc<TResult> action)
        {
            return _helper.FixMethod(action);
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFFunc<T, TResult> FixMethod<T, TResult>(HTFFunc<T, TResult> action)
        {
            return _helper.FixMethod(action);
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFFunc<T1, T2, TResult> FixMethod<T1, T2, TResult>(HTFFunc<T1, T2, TResult> action)
        {
            return _helper.FixMethod(action);
        }
        /// <summary>
        /// 热修复目标方法
        /// </summary>
        /// <param name="action">目标方法</param>
        /// <returns>修复后的方法</returns>
        public HTFFunc<T1, T2, T3, TResult> FixMethod<T1, T2, T3, TResult>(HTFFunc<T1, T2, T3, TResult> action)
        {
            return _helper.FixMethod(action);
        }
        
        /// <summary>
        /// 热修复方法的类型
        /// </summary>
        public enum HotfixMethodType
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