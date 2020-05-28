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
        Dictionary<string, CustomModuleBase> CustomModules { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        void OnInitialization();
        /// <summary>
        /// 准备工作
        /// </summary>
        void OnPreparatory();
        /// <summary>
        /// 刷新
        /// </summary>
        void OnRefresh();
        /// <summary>
        /// 终结
        /// </summary>
        void OnTermination();
        /// <summary>
        /// 暂停
        /// </summary>
        void OnPause();
        /// <summary>
        /// 恢复
        /// </summary>
        void OnUnPause();
    }
}