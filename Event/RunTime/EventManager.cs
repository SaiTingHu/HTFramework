using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace HT.Framework
{
    /// <summary>
    /// 事件管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EventManager : ModuleManager
    {
        private Dictionary<Type, Action<object, EventHandler>> _eventHandlerList = new Dictionary<Type, Action<object, EventHandler>>();

        public override void Initialization()
        {
            base.Initialization();

            //注册所有存在的事件
            Assembly assembly = Assembly.GetAssembly(typeof(EventHandler));
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].BaseType == typeof(EventHandler))
                {
                    _eventHandlerList.Add(types[i], null);
                }
            }
        }

        public override void Termination()
        {
            base.Termination();

            _eventHandlerList.Clear();
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        public void Subscribe<T>(Action<object, EventHandler> handler) where T : EventHandler
        {
            if (_eventHandlerList.ContainsKey(typeof(T)))
            {
                _eventHandlerList[typeof(T)] += handler;
            }
            else
            {
                GlobalTools.LogError("订阅事件失败：不存在可以订阅的事件类型 " + typeof(T).Name + " ！");
            }
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        public void Subscribe(Type type, Action<object, EventHandler> handler)
        {
            if (_eventHandlerList.ContainsKey(type))
            {
                _eventHandlerList[type] += handler;
            }
            else
            {
                GlobalTools.LogError("订阅事件失败：不存在可以订阅的事件类型 " + type.Name + " ！");
            }
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        public void Unsubscribe<T>(Action<object, EventHandler> handler) where T : EventHandler
        {
            if (_eventHandlerList.ContainsKey(typeof(T)))
            {
                _eventHandlerList[typeof(T)] -= handler;
            }
            else
            {
                GlobalTools.LogError("取消订阅事件失败：不存在可以取消订阅的事件类型 " + typeof(T).Name + " ！");
            }
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        public void Unsubscribe(Type type, Action<object, EventHandler> handler)
        {
            if (_eventHandlerList.ContainsKey(type))
            {
                _eventHandlerList[type] -= handler;
            }
            else
            {
                GlobalTools.LogError("取消订阅事件失败：不存在可以取消订阅的事件类型 " + type.Name + " ！");
            }
        }

        /// <summary>
        /// 抛出事件
        /// </summary>
        public void Throw(object sender, EventHandler handler)
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
                GlobalTools.LogError("抛出事件失败：不存在可以抛出的事件类型 " + type.Name + " ！");
            }
        }
    }
}
