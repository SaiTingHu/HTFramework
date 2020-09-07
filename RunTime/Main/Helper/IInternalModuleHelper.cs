namespace HT.Framework
{
    /// <summary>
    /// 内置模块的助手接口
    /// </summary>
    public interface IInternalModuleHelper
    {
        /// <summary>
        /// 所属的内置模块
        /// </summary>
        InternalModuleBase Module { get; set; }

        /// <summary>
        /// 初始化助手
        /// </summary>
        void OnInitialization();
        /// <summary>
        /// 助手准备工作
        /// </summary>
        void OnPreparatory();
        /// <summary>
        /// 刷新助手
        /// </summary>
        void OnRefresh();
        /// <summary>
        /// 终结助手
        /// </summary>
        void OnTermination();
        /// <summary>
        /// 暂停助手
        /// </summary>
        void OnPause();
        /// <summary>
        /// 恢复助手
        /// </summary>
        void OnUnPause();
    }
}