using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 事件管理者
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.Event)]
    public sealed class EventManager : InternalModuleBase
    {
        //I型事件
        private Dictionary<Type, HTFAction<object, EventHandlerBase>> _eventHandlerList1 = new Dictionary<Type, HTFAction<object, EventHandlerBase>>();
        //II型事件
        private Dictionary<Type, HTFAction> _eventHandlerList2 = new Dictionary<Type, HTFAction>();
        //III型事件
        private Dictionary<Type, HTFAction<EventHandlerBase>> _eventHandlerList3 = new Dictionary<Type, HTFAction<EventHandlerBase>>();

        public override void OnInitialization()
        {
            base.OnInitialization();

            //注册所有存在的事件
            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
            {
                return type.IsSubclassOf(typeof(EventHandlerBase));
            });
            for (int i = 0; i < types.Count; i++)
            {
                _eventHandlerList1.Add(types[i], null);
                _eventHandlerList2.Add(types[i], null);
                _eventHandlerList3.Add(types[i], null);
            }
        }

        public override void OnTermination()
        {
            base.OnTermination();

            _eventHandlerList1.Clear();
            _eventHandlerList2.Clear();
            _eventHandlerList3.Clear();
        }

        /// <summary>
        /// 订阅I型事件 ------ HTFAction(object, EventHandlerBase)
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Subscribe<T>(HTFAction<object, EventHandlerBase> handler) where T : EventHandlerBase
        {
            Subscribe(typeof(T), handler);
        }
        /// <summary>
        /// 订阅I型事件 ------ HTFAction(object, EventHandlerBase)
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Subscribe(Type type, HTFAction<object, EventHandlerBase> handler)
        {
            if (_eventHandlerList1.ContainsKey(type))
            {
                _eventHandlerList1[type] += handler;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Event, "订阅I型事件失败：不存在可以订阅的事件类型 " + type.Name + " ！");
            }
        }
        /// <summary>
        /// 订阅II型事件 ------ HTFAction()
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Subscribe<T>(HTFAction handler) where T : EventHandlerBase
        {
            Subscribe(typeof(T), handler);
        }
        /// <summary>
        /// 订阅II型事件 ------ HTFAction()
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Subscribe(Type type, HTFAction handler)
        {
            if (_eventHandlerList2.ContainsKey(type))
            {
                _eventHandlerList2[type] += handler;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Event, "订阅II型事件失败：不存在可以订阅的事件类型 " + type.Name + " ！");
            }
        }
        /// <summary>
        /// 订阅III型事件 ------ HTFAction(EventHandlerBase)
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Subscribe<T>(HTFAction<EventHandlerBase> handler) where T : EventHandlerBase
        {
            Subscribe(typeof(T), handler);
        }
        /// <summary>
        /// 订阅III型事件 ------ HTFAction(EventHandlerBase)
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Subscribe(Type type, HTFAction<EventHandlerBase> handler)
        {
            if (_eventHandlerList3.ContainsKey(type))
            {
                _eventHandlerList3[type] += handler;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Event, "订阅III型事件失败：不存在可以订阅的事件类型 " + type.Name + " ！");
            }
        }
        /// <summary>
        /// 取消订阅I型事件 ------ HTFAction(object, EventHandlerBase)
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe<T>(HTFAction<object, EventHandlerBase> handler) where T : EventHandlerBase
        {
            Unsubscribe(typeof(T), handler);
        }
        /// <summary>
        /// 取消订阅I型事件 ------ HTFAction(object, EventHandlerBase)
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe(Type type, HTFAction<object, EventHandlerBase> handler)
        {
            if (_eventHandlerList1.ContainsKey(type))
            {
                _eventHandlerList1[type] -= handler;
            }
        }
        /// <summary>
        /// 取消订阅II型事件 ------ HTFAction()
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe<T>(HTFAction handler) where T : EventHandlerBase
        {
            Unsubscribe(typeof(T), handler);
        }
        /// <summary>
        /// 取消订阅II型事件 ------ HTFAction()
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe(Type type, HTFAction handler)
        {
            if (_eventHandlerList2.ContainsKey(type))
            {
                _eventHandlerList2[type] -= handler;
            }
        }
        /// <summary>
        /// 取消订阅III型事件 ------ HTFAction(EventHandlerBase)
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe<T>(HTFAction<EventHandlerBase> handler) where T : EventHandlerBase
        {
            Unsubscribe(typeof(T), handler);
        }
        /// <summary>
        /// 取消订阅III型事件 ------ HTFAction(EventHandlerBase)
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe(Type type, HTFAction<EventHandlerBase> handler)
        {
            if (_eventHandlerList3.ContainsKey(type))
            {
                _eventHandlerList3[type] -= handler;
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
            if (_eventHandlerList1.ContainsKey(type))
            {
                _eventHandlerList1[type] = null;
            }
            if (_eventHandlerList2.ContainsKey(type))
            {
                _eventHandlerList2[type] = null;
            }
            if (_eventHandlerList3.ContainsKey(type))
            {
                _eventHandlerList3[type] = null;
            }
        }

        /// <summary>
        /// 抛出I型事件（抛出事件时，请使用引用池生成事件处理者实例）
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="handler">事件处理类实例</param>
        public void Throw(object sender, EventHandlerBase handler)
        {
            Type type = handler.GetType();
            if (_eventHandlerList1.ContainsKey(type))
            {
                if (_eventHandlerList1[type] != null)
                {
                    _eventHandlerList1[type](sender, handler);
                    Main.m_ReferencePool.Despawn(handler);
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Event, "抛出I型事件失败：不存在可以抛出的事件类型 " + type.Name + " ！");
            }
        }
        /// <summary>
        /// 抛出III型事件（抛出事件时，请使用引用池生成事件处理者实例）
        /// </summary>
        /// <param name="handler">事件处理类实例</param>
        public void Throw(EventHandlerBase handler)
        {
            Type type = handler.GetType();
            if (_eventHandlerList3.ContainsKey(type))
            {
                if (_eventHandlerList3[type] != null)
                {
                    _eventHandlerList3[type](handler);
                    Main.m_ReferencePool.Despawn(handler);
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Event, "抛出III型事件失败：不存在可以抛出的事件类型 " + type.Name + " ！");
            }
        }
        /// <summary>
        /// 抛出II型事件
        /// </summary>
        /// <typeparam name="T">事件处理类</typeparam>
        public void Throw<T>() where T : EventHandlerBase
        {
            Throw(typeof(T));
        }
        /// <summary>
        /// 抛出II型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        public void Throw(Type type)
        {
            if (_eventHandlerList2.ContainsKey(type))
            {
                _eventHandlerList2[type]?.Invoke();
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Event, "抛出II型事件失败：不存在可以抛出的事件类型 " + type.Name + " ！");
            }
        }
    }
}