using System;

namespace HT.Framework
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    [InternalModule(HTFrameworkModule.Event)]
    public sealed class EventManager : InternalModuleBase<IEventHelper>
    {
        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Subscribe<T>(HTFAction<EventHandlerBase> handler) where T : EventHandlerBase
        {
            _helper.Subscribe(typeof(T), handler);
        }
        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Subscribe(Type type, HTFAction<EventHandlerBase> handler)
        {
            _helper.Subscribe(type, handler);
        }
        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe<T>(HTFAction<EventHandlerBase> handler) where T : EventHandlerBase
        {
            _helper.Unsubscribe(typeof(T), handler);
        }
        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe(Type type, HTFAction<EventHandlerBase> handler)
        {
            _helper.Unsubscribe(type, handler);
        }
        /// <summary>
        /// 清空已订阅的事件
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        public void ClearSubscribe<T>() where T : EventHandlerBase
        {
            _helper.ClearSubscribe(typeof(T));
        }
        /// <summary>
        /// 清空已订阅的事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        public void ClearSubscribe(Type type)
        {
            _helper.ClearSubscribe(type);
        }
        /// <summary>
        /// 抛出事件
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        public void Throw<T>() where T : EventHandlerBase
        {
            _helper.Throw(typeof(T));
        }
        /// <summary>
        /// 抛出事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        public void Throw(Type type)
        {
            _helper.Throw(type);
        }
        /// <summary>
        /// 抛出事件（抛出事件时，请使用引用池生成事件处理者实例）
        /// </summary>
        /// <param name="handler">事件处理类实例</param>
        public void Throw(EventHandlerBase handler)
        {
            _helper.Throw(handler);
        }
    }
}