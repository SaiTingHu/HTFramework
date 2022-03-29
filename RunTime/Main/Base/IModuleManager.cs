namespace HT.Framework
{
    /// <summary>
    /// 模块管理器接口
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// 模块的优先级（越小越优先）
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 初始化模块
        /// </summary>
        void OnInit();
        /// <summary>
        /// 模块准备工作
        /// </summary>
        void OnReady();
        /// <summary>
        /// 更新模块
        /// </summary>
        void OnUpdate();
        /// <summary>
        /// 终结模块
        /// </summary>
        void OnTerminate();
        /// <summary>
        /// 暂停模块
        /// </summary>
        void OnPause();
        /// <summary>
        /// 恢复模块
        /// </summary>
        void OnResume();
    }
}