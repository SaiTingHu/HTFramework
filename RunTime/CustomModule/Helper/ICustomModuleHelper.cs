using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 自定义模块管理器的助手接口
    /// </summary>
    public interface ICustomModuleHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 所有自定义模块
        /// </summary>
        Dictionary<string, CustomModuleBase> CustomModules { get; }
    }
}