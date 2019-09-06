using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 事件管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EventManager : ModuleManagerBase
    {
        private Dictionary<Type, HTFAction<object, EventHandlerBase>> _eventHandlerList = new Dictionary<Type, HTFAction<object, EventHandlerBase>>();

        public override void OnInitialization()
        {
            base.OnInitialization();

            //注册所有存在的事件
            List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i].IsSubclassOf(typeof(EventHandlerBase)))
                {
                    _eventHandlerList.Add(types[i], null);
                }
            }
        }

        public override void OnTermination()
        {
            base.OnTermination();

            _eventHandlerList.Clear();
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Subscribe<T>(HTFAction<object, EventHandlerBase> handler) where T : EventHandlerBase
        {
            Subscribe(typeof(T), handler);
        }
        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Subscribe(Type type, HTFAction<object, EventHandlerBase> handler)
        {
            if (_eventHandlerList.ContainsKey(type))
            {
                _eventHandlerList[type] += handler;
            }
            else
            {
                GlobalTools.LogError(string.Format("订阅事件失败：不存在可以订阅的事件类型 {0} ！", type.Name));
            }
        }
        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe<T>(HTFAction<object, EventHandlerBase> handler) where T : EventHandlerBase
        {
            Unsubscribe(typeof(T), handler);
        }
        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe(Type type, HTFAction<object, EventHandlerBase> handler)
        {
            if (_eventHandlerList.ContainsKey(type))
            {
                _eventHandlerList[type] -= handler;
            }
        }
        /// <summary>
        /// 清空已订阅的事件
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        public void ClearSubscribe<T>() where T : EventHandlerBase
        {
            ClearSubscribe(typeof(T));
        }
        /// <summary>
        /// 清空已订阅的事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        public void ClearSubscribe(Type type)
        {
            if (_eventHandlerList.ContainsKey(type))
            {
                _eventHandlerList[type] = null;
            }
        }

        /// <summary>
        /// 抛出事件（抛出事件时，请使用引用池生成事件处理者实例）
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="handler">事件处理类实例</param>
        public void Throw(object sender, EventHandlerBase handler)
        {
            Type type = handler.GetType();
            if (_eventHandlerList.ContainsKey(type))
            {
                if (_eventHandlerList[type] != null)
                {
                    _eventHandlerList[type](sender, handler);
                    Main.m_ReferencePool.Despawn(handler);
                }
            }
            else
            {
                GlobalTools.LogError(string.Format("抛出事件失败：不存在可以抛出的事件类型 {0} ！", type.Name));
            }
        }
    }
}
