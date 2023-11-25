namespace HT.Framework
{
    /// <summary>
    /// 任务流程结束事件【调用 TaskMaster.End 结束整个流程或任务执行完毕后触发】
    /// </summary>
    public sealed class EventTaskEnd : EventHandlerBase
    {
        /// <summary>
        /// 填充数据，所有属性、字段的初始化工作可以在这里完成
        /// </summary>
        public EventTaskEnd Fill()
        {
            return this;
        }
        /// <summary>
        /// 重置引用，当被引用池回收时调用
        /// </summary>
        public override void Reset()
        {

        }
    }
}