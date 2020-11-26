using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 内置模块基类
    /// </summary>
    public abstract class InternalModuleBase : HTBehaviour, IModuleManager
    {
        /// <summary>
        /// 内置模块助手类型
        /// </summary>
        [SerializeField] internal string HelperType = "<None>";
        /// <summary>
        /// 内置模块助手
        /// </summary>
        internal IInternalModuleHelper Helper { get; private set; }

        /// <summary>
        /// 初始化模块
        /// </summary>
        internal virtual void OnInitialization()
        {
            if (HelperType != "<None>")
            {
                Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(HelperType);
                if (type != null)
                {
                    if (typeof(IInternalModuleHelper).IsAssignableFrom(type))
                    {
                        Helper = Activator.CreateInstance(type) as IInternalModuleHelper;
                        Helper.Module = this;
                        Helper.OnInitialization();
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Input, "创建内置模块助手失败：内置模块助手类 " + HelperType + " 必须实现该模块对应的助手接口！");
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Input, "创建内置模块助手失败：丢失内置模块助手类 " + HelperType + "！");
                }
            }
        }
        /// <summary>
        /// 模块准备工作
        /// </summary>
        internal virtual void OnPreparatory()
        {
            if (Helper != null)
            {
                Helper.OnPreparatory();
            }
        }
        /// <summary>
        /// 刷新模块
        /// </summary>
        internal virtual void OnRefresh()
        {
            if (Helper != null)
            {
                Helper.OnRefresh();
            }
        }
        /// <summary>
        /// 终结模块
        /// </summary>
        internal virtual void OnTermination()
        {
            if (Helper != null)
            {
                Helper.OnTermination();
            }
        }
        /// <summary>
        /// 暂停模块
        /// </summary>
        internal virtual void OnPause()
        {
            if (Helper != null)
            {
                Helper.OnPause();
            }
        }
        /// <summary>
        /// 恢复模块
        /// </summary>
        internal virtual void OnUnPause()
        {
            if (Helper != null)
            {
                Helper.OnUnPause();
            }
        }
    }
}