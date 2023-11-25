using System;
using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 事件管理器的助手接口
    /// </summary>
    public interface IEventHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 事件列表
        /// </summary>
        Dictionary<Type, HTFAction<EventHandlerBase>> EventHandlerList { get; }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        void Subscribe(Type type, HTFAction<EventHandlerBase> handler);
        /// <summary>
        /// 取消订阅事件
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
        /// 抛出事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        void Throw(Type type);
        /// <summary>
        /// 抛出事件（抛出事件时，请使用引用池生成事件处理者实例）
        /// </summary>
        /// <param name="handler">事件处理类实例</param>
        void Throw(EventHandlerBase handler);
    }
}