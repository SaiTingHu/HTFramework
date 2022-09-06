using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 自定义模块管理器
    /// </summary>
    [InternalModule(HTFrameworkModule.CustomModule)]
    public sealed class CustomModuleManager : InternalModuleBase<ICustomModuleHelper>
    {
        /// <summary>
        /// 自定义模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns>自定义模块</returns>
        public CustomModuleBase this[string moduleName]
        {
            get
            {
                return _helper.GetCustomModule(moduleName);
            }
        }
        /// <summary>
        /// 获取所有的自定义模块
        /// </summary>
        public List<CustomModuleBase> GetAllCustomModule()
        {
            return _helper.GetAllCustomModule();
        }
        /// <summary>
        /// 终止指定的自定义模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        public void TerminationModule(string moduleName)
        {
            _helper.TerminationModule(moduleName);
        }
    }
}