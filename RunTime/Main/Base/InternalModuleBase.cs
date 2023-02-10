using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 内置模块基类
    /// </summary>
    /// <typeparam name="H">内置模块的助手类型</typeparam>
    [DisallowMultipleComponent]
    [LockTransform]
    public abstract class InternalModuleBase<H> : HTBehaviour, IModuleManager where H : class, IInternalModuleHelper
    {
        /// <summary>
        /// 内置模块助手类型
        /// </summary>
        [SerializeField] internal string HelperType = "<None>";
        /// <summary>
        /// 模块的优先级（越小越优先）
        /// </summary>
        public virtual int Priority { get; } = 0;
        /// <summary>
        /// 内置模块助手
        /// </summary>
        protected H _helper { get; private set; }

        /// <summary>
        /// 初始化模块
        /// </summary>
        public virtual void OnInit()
        {
            if (HelperType != "<None>")
            {
                Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(HelperType);
                if (type != null)
                {
                    if (typeof(IInternalModuleHelper).IsAssignableFrom(type))
                    {
                        _helper = Activator.CreateInstance(type) as H;
                        _helper.Module = this;
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Main, $"创建内置模块助手失败：内置模块助手类 {HelperType} 必须实现该模块对应的助手接口！");
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Main, $"创建内置模块助手失败：丢失内置模块助手类 {HelperType}！");
                }
            }

            if (_helper != null)
            {
                _helper.OnInit();
            }
        }
        /// <summary>
        /// 模块准备工作
        /// </summary>
        public virtual void OnReady()
        {
            if (_helper != null)
            {
                _helper.OnReady();
            }
        }
        /// <summary>
        /// 刷新模块
        /// </summary>
        public virtual void OnUpdate()
        {
            if (_helper != null)
            {
                _helper.OnUpdate();
            }
        }
        /// <summary>
        /// 终结模块
        /// </summary>
        public virtual void OnTerminate()
        {
            if (_helper != null)
            {
                _helper.OnTerminate();
            }
        }
        /// <summary>
        /// 暂停模块
        /// </summary>
        public virtual void OnPause()
        {
            if (_helper != null)
            {
                _helper.OnPause();
            }
        }
        /// <summary>
        /// 恢复模块
        /// </summary>
        public virtual void OnResume()
        {
            if (_helper != null)
            {
                _helper.OnResume();
            }
        }
    }
}