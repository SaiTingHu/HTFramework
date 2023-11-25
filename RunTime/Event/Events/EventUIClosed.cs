namespace HT.Framework
{
    /// <summary>
    /// 任意UI关闭事件
    /// </summary>
    public sealed class EventUIClosed : EventHandlerBase
    {
        /// <summary>
        /// 关闭的UI
        /// </summary>
        public UILogicBase UILogic { get; private set; }

        /// <summary>
        /// 填充数据，所有属性、字段的初始化工作可以在这里完成
        /// </summary>
        public EventUIClosed Fill(UILogicBase uILogic)
        {
            UILogic = uILogic;
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