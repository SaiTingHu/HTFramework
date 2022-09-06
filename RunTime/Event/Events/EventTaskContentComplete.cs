namespace HT.Framework
{
    /// <summary>
    /// 任务内容完成事件（三型事件）
    /// </summary>
    public sealed class EventTaskContentComplete : EventHandlerBase
    {
        /// <summary>
        /// 任务内容对象
        /// </summary>
        public TaskContentBase TaskContent { get; private set; }
        /// <summary>
        /// 是否为自动完成
        /// </summary>
        public bool IsAutoComplete { get; private set; }

        /// <summary>
        /// 填充数据，所有属性、字段的初始化工作可以在这里完成
        /// </summary>
        /// <param name="taskContent">任务内容对象</param>
        /// <param name="isAutoComplete">是否为自动完成</param>
        public EventTaskContentComplete Fill(TaskContentBase taskContent, bool isAutoComplete)
        {
            TaskContent = taskContent;
            IsAutoComplete = isAutoComplete;
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