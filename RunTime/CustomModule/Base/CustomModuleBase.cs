namespace HT.Framework
{
    /// <summary>
    /// 自定义模块基类
    /// </summary>
    public abstract class CustomModuleBase : IModuleManager
    {
        /// <summary>
        /// 是否运行中
        /// </summary>
        public virtual bool IsRunning { get; set; }

        /// <summary>
        /// 初始化模块
        /// </summary>
        public virtual void OnInitialization()
        { }
        /// <summary>
        /// 模块准备工作
        /// </summary>
        public virtual void OnPreparatory()
        { }
        /// <summary>
        /// 刷新模块
        /// </summary>
        public virtual void OnRefresh()
        { }
        /// <summary>
        /// 终结模块
        /// </summary>
        public virtual void OnTermination()
        { }
        /// <summary>
        /// 暂停模块
        /// </summary>
        public virtual void OnPause()
        { }
        /// <summary>
        /// 恢复模块
        /// </summary>
        public virtual void OnResume()
        { }
    }
}