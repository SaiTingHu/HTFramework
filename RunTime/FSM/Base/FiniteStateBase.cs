namespace HT.Framework
{
    /// <summary>
    /// 有限状态基类
    /// </summary>
    public abstract class FiniteStateBase
    {
        /// <summary>
        /// 所属状态机
        /// </summary>
        public FSM StateMachine;

        /// <summary>
        /// 状态初始化
        /// </summary>
        public abstract void OnInit();
        /// <summary>
        /// 进入状态
        /// </summary>
        public abstract void OnEnter();
        /// <summary>
        /// 离开状态
        /// </summary>
        public abstract void OnLeave();
        /// <summary>
        /// 切换状态的动机
        /// </summary>
        public abstract void OnReason();
        /// <summary>
        /// 状态帧刷新
        /// </summary>
        public abstract void OnUpdate();
        /// <summary>
        /// 终止状态
        /// </summary>
        public abstract void OnTermination();
    }
}
