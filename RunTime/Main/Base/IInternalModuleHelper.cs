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
        IModuleManager Module { get; set; }

        /// <summary>
        /// 初始化助手
        /// </summary>
        void OnInit();
        /// <summary>
        /// 助手准备工作
        /// </summary>
        void OnReady();
        /// <summary>
        /// 更新助手
        /// </summary>
        void OnUpdate();
        /// <summary>
        /// 终结助手
        /// </summary>
        void OnTerminate();
        /// <summary>
        /// 暂停助手
        /// </summary>
        void OnPause();
        /// <summary>
        /// 恢复助手
        /// </summary>
        void OnResume();
    }
}