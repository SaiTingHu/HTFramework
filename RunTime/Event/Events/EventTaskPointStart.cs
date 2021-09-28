namespace HT.Framework
{
    /// <summary>
    /// 任务点开始事件（三型事件）
    /// </summary>
    public sealed class EventTaskPointStart : EventHandlerBase
    {
        /// <summary>
        /// 任务点对象
        /// </summary>
        public TaskPointBase TaskPoint { get; private set; }

        /// <summary>
        /// 填充数据，所有属性、字段的初始化工作可以在这里完成
        /// </summary>
        /// <param name="taskPoint">任务点对象</param>
        public EventTaskPointStart Fill(TaskPointBase taskPoint)
        {
            TaskPoint = taskPoint;
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