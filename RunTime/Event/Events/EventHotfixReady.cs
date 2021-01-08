namespace HT.Framework
{
    /// <summary>
    /// 热更新准备就绪事件（二型事件）
    /// </summary>
    public sealed class EventHotfixReady : EventHandlerBase
    {
        /// <summary>
        /// 填充数据，所有属性、字段的初始化工作可以在这里完成
        /// </summary>
        public EventHotfixReady Fill()
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