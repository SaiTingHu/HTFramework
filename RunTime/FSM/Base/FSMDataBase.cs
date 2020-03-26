namespace HT.Framework
{
    /// <summary>
    /// 有限状态机数据基类
    /// </summary>
    public abstract class FSMDataBase
    {
        /// <summary>
        /// 所属状态机
        /// </summary>
        public FSM StateMachine;

        /// <summary>
        /// 所属状态机初始化，数据初始化
        /// </summary>
        public abstract void OnInit();
        /// <summary>
        /// 所属状态机重生
        /// </summary>
        public abstract void OnRenewal();
        /// <summary>
        /// 所属状态机完结
        /// </summary>
        public abstract void OnFinal();
        /// <summary>
        /// 所属状态机销毁，数据销毁
        /// </summary>
        public abstract void OnTermination();
    }
}
