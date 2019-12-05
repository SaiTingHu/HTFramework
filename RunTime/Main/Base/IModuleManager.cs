namespace HT.Framework
{
    /// <summary>
    /// 模块管理者接口
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// 初始化模块
        /// </summary>
        void OnInitialization();
        /// <summary>
        /// 模块准备工作
        /// </summary>
        void OnPreparatory();
        /// <summary>
        /// 刷新模块
        /// </summary>
        void OnRefresh();
        /// <summary>
        /// 终结模块
        /// </summary>
        void OnTermination();
        /// <summary>
        /// 暂停模块
        /// </summary>
        void OnPause();
        /// <summary>
        /// 恢复模块
        /// </summary>
        void OnUnPause();
    }
}