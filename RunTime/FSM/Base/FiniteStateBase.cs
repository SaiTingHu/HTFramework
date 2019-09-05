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
        /// <param name="lastState">上一个离开的状态</param>
        public abstract void OnEnter(FiniteStateBase lastState);
        /// <summary>
        /// 离开状态
        /// </summary>
        /// <param name="nextState">下一个进入的状态</param>
        public abstract void OnLeave(FiniteStateBase nextState);
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
