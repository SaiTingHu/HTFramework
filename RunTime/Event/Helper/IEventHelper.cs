using System;

namespace HT.Framework
{
    /// <summary>
    /// 事件管理器的助手接口
    /// </summary>
    public interface IEventHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void OnInitialization();
        /// <summary>
        /// 终结
        /// </summary>
        void OnTermination();

        /// <summary>
        /// 订阅I型事件 ------ HTFAction(object, EventHandlerBase)
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        void Subscribe(Type type, HTFAction<object, EventHandlerBase> handler);
        /// <summary>
        /// 订阅II型事件 ------ HTFAction()
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        void Subscribe(Type type, HTFAction handler);
        /// <summary>
        /// 订阅III型事件 ------ HTFAction(EventHandlerBase)
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        void Subscribe(Type type, HTFAction<EventHandlerBase> handler);
        /// <summary>
        /// 取消订阅I型事件 ------ HTFAction(object, EventHandlerBase)
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        void Unsubscribe(Type type, HTFAction<object, EventHandlerBase> handler);
        /// <summary>
        /// 取消订阅II型事件 ------ HTFAction()
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        void Unsubscribe(Type type, HTFAction handler);
        /// <summary>
        /// 取消订阅III型事件 ------ HTFAction(EventHandlerBase)
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        void Unsubscribe(Type type, HTFAction<EventHandlerBase> handler);
        /// <summary>
        /// 清空已订阅的事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        void ClearSubscribe(Type type);

        /// <summary>
        /// 抛出I型事件（抛出事件时，请使用引用池生成事件处理者实例）
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="handler">事件处理类实例</param>
        void Throw(object sender, EventHandlerBase handler);
        /// <summary>
        /// 抛出II型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        void Throw(Type type);
        /// <summary>
        /// 抛出III型事件（抛出事件时，请使用引用池生成事件处理者实例）
        /// </summary>
        /// <param name="handler">事件处理类实例</param>
        void Throw(EventHandlerBase handler);
    }
}