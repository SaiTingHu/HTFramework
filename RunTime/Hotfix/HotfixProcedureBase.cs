namespace HT.Framework
{
    /// <summary>
    /// 热更新流程基类
    /// </summary>
    public abstract class HotfixProcedureBase
    {
        /// <summary>
        /// 流程初始化
        /// </summary>
        public virtual void OnInit()
        { }
        /// <summary>
        /// 进入流程
        /// </summary>
        public virtual void OnEnter()
        { }
        /// <summary>
        /// 离开流程
        /// </summary>
        public virtual void OnLeave()
        { }
        /// <summary>
        /// 流程帧更新
        /// </summary>
        public virtual void OnUpdate()
        { }
        /// <summary>
        /// 流程秒更新
        /// </summary>
        public virtual void OnUpdateSecond()
        { }
    }
}