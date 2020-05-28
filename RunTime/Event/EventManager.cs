using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.Event)]
    public sealed class EventManager : InternalModuleBase
    {
        private IEventHelper _helper;

        internal override void OnInitialization()
        {
            base.OnInitialization();

            _helper = Helper as IEventHelper;
            _helper.OnInitialization();
        }
        internal override void OnTermination()
        {
            base.OnTermination();

            _helper.OnTermination();
        }

        /// <summary>
        /// 订阅I型事件 ------ HTFAction(object, EventHandlerBase)
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Subscribe<T>(HTFAction<object, EventHandlerBase> handler) where T : EventHandlerBase
        {
            _helper.Subscribe(typeof(T), handler);
        }
        /// <summary>
        /// 订阅I型事件 ------ HTFAction(object, EventHandlerBase)
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Subscribe(Type type, HTFAction<object, EventHandlerBase> handler)
        {
            _helper.Subscribe(type, handler);
        }
        /// <summary>
        /// 订阅II型事件 ------ HTFAction()
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Subscribe<T>(HTFAction handler) where T : EventHandlerBase
        {
            _helper.Subscribe(typeof(T), handler);
        }
        /// <summary>
        /// 订阅II型事件 ------ HTFAction()
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Subscribe(Type type, HTFAction handler)
        {
            _helper.Subscribe(type, handler);
        }
        /// <summary>
        /// 订阅III型事件 ------ HTFAction(EventHandlerBase)
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Subscribe<T>(HTFAction<EventHandlerBase> handler) where T : EventHandlerBase
        {
            _helper.Subscribe(typeof(T), handler);
        }
        /// <summary>
        /// 订阅III型事件 ------ HTFAction(EventHandlerBase)
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Subscribe(Type type, HTFAction<EventHandlerBase> handler)
        {
            _helper.Subscribe(type, handler);
        }
        /// <summary>
        /// 取消订阅I型事件 ------ HTFAction(object, EventHandlerBase)
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe<T>(HTFAction<object, EventHandlerBase> handler) where T : EventHandlerBase
        {
            _helper.Unsubscribe(typeof(T), handler);
        }
        /// <summary>
        /// 取消订阅I型事件 ------ HTFAction(object, EventHandlerBase)
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe(Type type, HTFAction<object, EventHandlerBase> handler)
        {
            _helper.Unsubscribe(type, handler);
        }
        /// <summary>
        /// 取消订阅II型事件 ------ HTFAction()
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe<T>(HTFAction handler) where T : EventHandlerBase
        {
            _helper.Unsubscribe(typeof(T), handler);
        }
        /// <summary>
        /// 取消订阅II型事件 ------ HTFAction()
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe(Type type, HTFAction handler)
        {
            _helper.Unsubscribe(type, handler);
        }
        /// <summary>
        /// 取消订阅III型事件 ------ HTFAction(EventHandlerBase)
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe<T>(HTFAction<EventHandlerBase> handler) where T : EventHandlerBase
        {
            _helper.Unsubscribe(typeof(T), handler);
        }
        /// <summary>
        /// 取消订阅III型事件 ------ HTFAction(EventHandlerBase)
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
        /// 抛出I型事件（抛出事件时，请使用引用池生成事件处理者实例）
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="handler">事件处理类实例</param>
        public void Throw(object sender, EventHandlerBase handler)
        {
            _helper.Throw(sender, handler);
        }
        /// <summary>
        /// 抛出II型事件
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        public void Throw<T>() where T : EventHandlerBase
        {
            _helper.Throw(typeof(T));
        }
        /// <summary>
        /// 抛出II型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        public void Throw(Type type)
        {
            _helper.Throw(type);
        }
        /// <summary>
        /// 抛出III型事件（抛出事件时，请使用引用池生成事件处理者实例）
        /// </summary>
        /// <param name="handler">事件处理类实例</param>
        public void Throw(EventHandlerBase handler)
        {
            _helper.Throw(handler);
        }
    }
}