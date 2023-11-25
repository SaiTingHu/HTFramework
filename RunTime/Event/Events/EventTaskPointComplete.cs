namespace HT.Framework
{
    /// <summary>
    /// 任务点完成事件
    /// </summary>
    public sealed class EventTaskPointComplete : EventHandlerBase
    {
        /// <summary>
        /// 任务点对象
        /// </summary>
        public TaskPointBase TaskPoint { get; private set; }
        /// <summary>
        /// 任务点是否启用
        /// </summary>
        public bool IsEnable { get; private set; }
        /// <summary>
        /// 是否为自动完成
        /// </summary>
        public bool IsAutoComplete { get; private set; }

        /// <summary>
        /// 填充数据，所有属性、字段的初始化工作可以在这里完成
        /// </summary>
        /// <param name="taskPoint">任务点对象</param>
        /// <param name="isEnable">任务点是否启用</param>
        /// <param name="isAutoComplete">是否为自动完成</param>
        public EventTaskPointComplete Fill(TaskPointBase taskPoint, bool isEnable, bool isAutoComplete)
        {
            TaskPoint = taskPoint;
            IsEnable = isEnable;
            IsAutoComplete = isAutoComplete;
            return this;
        }
        /// <summary>
        /// 重置引用，当被引用池回收时调用
        /// </summary>
        public override void Reset()
        {
            TaskPoint = null;
        }
    }
}