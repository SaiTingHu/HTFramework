namespace HT.Framework
{
    /// <summary>
    /// 任务内容执行事件
    /// </summary>
    public sealed class EventTaskContentExecute : EventHandlerBase
    {
        /// <summary>
        /// 任务内容对象
        /// </summary>
        public TaskContentBase TaskContent;

        /// <summary>
        /// 填充数据，所有属性、字段的初始化工作可以在这里完成
        /// </summary>
        public EventTaskContentExecute Fill(TaskContentBase taskContent)
        {
            TaskContent = taskContent;
            return this;
        }

        /// <summary>
        /// 重置引用，当被引用池回收时调用
        /// </summary>
        public override void Reset()
        {
            TaskContent = null;
        }
    }
}