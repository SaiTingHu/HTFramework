using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 事件管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EventManager : ModuleManager
    {
        private Dictionary<Type, HTFAction<object, EventHandler>> _eventHandlerList = new Dictionary<Type, HTFAction<object, EventHandler>>();

        public override void OnInitialization()
        {
            base.OnInitialization();

            //注册所有存在的事件
            List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i].IsSubclassOf(typeof(EventHandler)))
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
        public void Subscribe<T>(HTFAction<object, EventHandler> handler) where T : EventHandler
        {
            if (_eventHandlerList.ContainsKey(typeof(T)))
            {
                _eventHandlerList[typeof(T)] += handler;
            }
            else
            {
                GlobalTools.LogError(string.Format("订阅事件失败：不存在可以订阅的事件类型 {0} ！", typeof(T).Name));
            }
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        public void Subscribe(Type type, HTFAction<object, EventHandler> handler)
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
        public void Unsubscribe<T>(HTFAction<object, EventHandler> handler) where T : EventHandler
        {
            if (_eventHandlerList.ContainsKey(typeof(T)))
            {
                _eventHandlerList[typeof(T)] -= handler;
            }
            else
            {
                GlobalTools.LogError(string.Format("取消订阅事件失败：不存在可以取消订阅的事件类型 {0} ！", typeof(T).Name));
            }
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        public void Unsubscribe(Type type, HTFAction<object, EventHandler> handler)
        {
            if (_eventHandlerList.ContainsKey(type))
            {
                _eventHandlerList[type] -= handler;
            }
            else
            {
                GlobalTools.LogError(string.Format("取消订阅事件失败：不存在可以取消订阅的事件类型 {0} ！", type.Name));
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
                GlobalTools.LogError(string.Format("抛出事件失败：不存在可以抛出的事件类型 {0} ！", type.Name));
            }
        }
    }
}
