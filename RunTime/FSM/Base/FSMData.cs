namespace HT.Framework
{
    /// <summary>
    /// 有限状态机数据基类
    /// </summary>
    public abstract class FSMData
    {
        /// <summary>
        /// 所属状态机
        /// </summary>
        public FSM StateMachine;

        /// <summary>
        /// 数据初始化
        /// </summary>
        public abstract void OnInit();
    }
}
