﻿namespace HT.Framework
{
    /// <summary>
    /// 任务流程开始事件【调用 TaskMaster.Begin 开始整个流程后触发】（二型事件）
    /// </summary>
    public sealed class EventTaskBegin : EventHandlerBase
    {
        /// <summary>
        /// 填充数据，所有属性、字段的初始化工作可以在这里完成
        /// </summary>
        public EventTaskBegin Fill()
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