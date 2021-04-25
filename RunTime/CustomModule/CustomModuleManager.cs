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
                if (_helper.CustomModules.ContainsKey(moduleName))
                {
                    return _helper.CustomModules[moduleName];
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 终止指定的自定义模块
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        public void TerminationModule(string moduleName)
        {
            if (_helper.CustomModules.ContainsKey(moduleName))
            {
                _helper.CustomModules[moduleName].OnTermination();
                _helper.CustomModules.Remove(moduleName);
            }
        }
    }
}