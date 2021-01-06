namespace HT.Framework
{
    /// <summary>
    /// 创建实体成功事件
    /// </summary>
    public sealed class EventCreateEntitySucceed : EventHandlerBase
    {
        public EntityLogicBase EntityLogic;

        /// <summary>
        /// 填充数据，所有属性、字段的初始化工作可以在这里完成
        /// </summary>
        public EventCreateEntitySucceed Fill(EntityLogicBase entityLogic)
        {
            EntityLogic = entityLogic;
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