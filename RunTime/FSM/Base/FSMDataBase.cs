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
        public FSM StateMachine { get; internal set; }
        /// <summary>
        /// 是否支持数据驱动
        /// </summary>
        public bool IsSupportedDataDriver
        {
            get
            {
                return this is IDataDriver;
            }
        }
        /// <summary>
        /// 是否启用自动化，这将造成反射的性能消耗
        /// </summary>
        public virtual bool IsAutomate => false;

        /// <summary>
        /// 所属状态机初始化，数据初始化
        /// </summary>
        public virtual void OnInit()
        { }
        /// <summary>
        /// 所属状态机重生
        /// </summary>
        public virtual void OnRenewal()
        { }
        /// <summary>
        /// 所属状态机完结
        /// </summary>
        public virtual void OnFinal()
        { }
        /// <summary>
        /// 所属状态机销毁，数据销毁
        /// </summary>
        public virtual void OnTerminate()
        { }
    }
}