namespace HT.Framework
{
    /// <summary>
    /// 流程基类
    /// </summary>
    public abstract class ProcedureBase
    {
        /// <summary>
        /// 流程初始化
        /// </summary>
        public virtual void OnInit()
        { }
        /// <summary>
        /// 进入流程
        /// </summary>
        /// <param name="lastProcedure">上一个离开的流程</param>
        public virtual void OnEnter(ProcedureBase lastProcedure)
        { }
        /// <summary>
        /// 离开流程
        /// </summary>
        /// <param name="nextProcedure">下一个进入的流程</param>
        public virtual void OnLeave(ProcedureBase nextProcedure)
        { }
        /// <summary>
        /// 流程帧刷新
        /// </summary>
        public virtual void OnUpdate()
        { }
        /// <summary>
        /// 流程帧刷新（秒）
        /// </summary>
        public virtual void OnUpdateSecond()
        { }

        /// <summary>
        /// 切换流程
        /// </summary>
        protected void SwitchProcedure<T>() where T : ProcedureBase
        {
            Main.m_Procedure.SwitchProcedure<T>();
        }
        /// <summary>
        /// 切换至下一流程
        /// </summary>
        protected void SwitchNextProcedure()
        {
            Main.m_Procedure.SwitchNextProcedure();
        }
        /// <summary>
        /// 切换至上一流程
        /// </summary>
        protected void SwitchLastProcedure()
        {
            Main.m_Procedure.SwitchLastProcedure();
        }
    }
}