namespace HT.Framework
{
    /// <summary>
    /// 流程基类
    /// </summary>
    public abstract class Procedure
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

        /// <summary>
        /// 切换流程
        /// </summary>
        protected void SwitchProcedure<T>() where T : Procedure
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
