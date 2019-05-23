namespace HT.Framework
{
    /// <summary>
    /// 热更新流程基类
    /// </summary>
    public abstract class HotfixProcedure
    {
        /// <summary>
        /// 流程初始化
        /// </summary>
        public abstract void OnInit();
        /// <summary>
        /// 进入流程
        /// </summary>
        public abstract void OnEnter();
        /// <summary>
        /// 离开流程
        /// </summary>
        public abstract void OnLeave();
        /// <summary>
        /// 流程帧刷新
        /// </summary>
        public abstract void OnUpdate();
        /// <summary>
        /// 流程帧刷新（秒）
        /// </summary>
        public abstract void OnUpdateSecond();
    }
}
