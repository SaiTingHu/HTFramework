namespace HT.Framework
{
    /// <summary>
    /// 游戏恢复事件
    /// </summary>
    public sealed class EventUnPauseGame : EventHandlerBase
    {
        /// <summary>
        /// 填充数据，所有属性、字段的初始化工作可以在这里完成
        /// </summary>
        public EventUnPauseGame Fill()
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