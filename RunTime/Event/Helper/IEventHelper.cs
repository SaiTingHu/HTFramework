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
        /// 一型事件
        /// </summary>
        Dictionary<Type, HTFAction<object, EventHandlerBase>> EventHandlerList1 { get; }
        /// <summary>
        /// 二型事件
        /// </summary>
        Dictionary<Type, HTFAction> EventHandlerList2 { get; }
        /// <summary>
        /// 三型事件
        /// </summary>
        Dictionary<Type, HTFAction<EventHandlerBase>> EventHandlerList3 { get; }

        /// <summary>
        /// 订阅一型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        void Subscribe(Type type, HTFAction<object, EventHandlerBase> handler);
        /// <summary>
        /// 订阅二型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        void Subscribe(Type type, HTFAction handler);
        /// <summary>
        /// 订阅三型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        void Subscribe(Type type, HTFAction<EventHandlerBase> handler);
        /// <summary>
        /// 取消订阅一型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        void Unsubscribe(Type type, HTFAction<object, EventHandlerBase> handler);
        /// <summary>
        /// 取消订阅二型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        void Unsubscribe(Type type, HTFAction handler);
        /// <summary>
        /// 取消订阅三型事件
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
        /// 抛出一型事件（抛出事件时，请使用引用池生成事件处理者实例）
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="handler">事件处理类实例</param>
        void Throw(object sender, EventHandlerBase handler);
        /// <summary>
        /// 抛出二型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        void Throw(Type type);
        /// <summary>
        /// 抛出三型事件（抛出事件时，请使用引用池生成事件处理者实例）
        /// </summary>
        /// <param name="handler">事件处理类实例</param>
        void Throw(EventHandlerBase handler);
    }
}