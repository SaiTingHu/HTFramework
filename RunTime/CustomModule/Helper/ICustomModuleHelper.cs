using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 自定义模块管理器的助手接口
    /// </summary>
    public interface ICustomModuleHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 获取指定的自定义模块
        /// </summary>
        /// <param name="name">模块名称</param>
        /// <returns>模块实例</returns>
        CustomModuleBase GetCustomModule(string name);
        /// <summary>
        /// 获取所有的自定义模块
        /// </summary>
        List<CustomModuleBase> GetAllCustomModule();
        /// <summary>
        /// 终止指定的自定义模块
        /// </summary>
        /// <param name="name">模块名称</param>
        void TerminationModule(string name);
    }
}