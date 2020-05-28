using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 自定义模块管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.CustomModule)]
    public sealed class CustomModuleManager : InternalModuleBase
    {
        private ICustomModuleHelper _helper;

        internal override void OnInitialization()
        {
            base.OnInitialization();

            _helper = Helper as ICustomModuleHelper;
            _helper.OnInitialization();
        }
        internal override void OnPreparatory()
        {
            base.OnPreparatory();

            _helper.OnPreparatory();
        }
        internal override void OnRefresh()
        {
            base.OnRefresh();

            _helper.OnRefresh();
        }
        internal override void OnTermination()
        {
            base.OnTermination();

            _helper.OnTermination();
        }
        internal override void OnPause()
        {
            base.OnPause();

            _helper.OnPause();
        }
        internal override void OnUnPause()
        {
            base.OnUnPause();

            _helper.OnUnPause();
        }

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