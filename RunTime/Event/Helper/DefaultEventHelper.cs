using System;
using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 默认的事件管理器助手
    /// </summary>
    public sealed class DefaultEventHelper : IEventHelper
    {
        /// <summary>
        /// 事件管理器
        /// </summary>
        public InternalModuleBase Module { get; set; }
        /// <summary>
        /// 一型事件
        /// </summary>
        public Dictionary<Type, HTFAction<object, EventHandlerBase>> EventHandlerList1 { get; private set; } = new Dictionary<Type, HTFAction<object, EventHandlerBase>>();
        /// <summary>
        /// 二型事件
        /// </summary>
        public Dictionary<Type, HTFAction> EventHandlerList2 { get; private set; } = new Dictionary<Type, HTFAction>();
        /// <summary>
        /// 三型事件
        /// </summary>
        public Dictionary<Type, HTFAction<EventHandlerBase>> EventHandlerList3 { get; private set; } = new Dictionary<Type, HTFAction<EventHandlerBase>>();

        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInitialization()
        {
            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
            {
                return type.IsSubclassOf(typeof(EventHandlerBase)) && !type.IsAbstract;
            });
            for (int i = 0; i < types.Count; i++)
            {
                EventHandlerList1.Add(types[i], null);
                EventHandlerList2.Add(types[i], null);
                EventHandlerList3.Add(types[i], null);
            }
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnPreparatory()
        {

        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnRefresh()
        {
            
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTermination()
        {
            EventHandlerList1.Clear();
            EventHandlerList2.Clear();
            EventHandlerList3.Clear();
        }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {

        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnUnPause()
        {

        }

        /// <summary>
        /// 订阅一型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Subscribe(Type type, HTFAction<object, EventHandlerBase> handler)
        {
            if (EventHandlerList1.ContainsKey(type))
            {
                EventHandlerList1[type] += handler;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Event, "订阅I型事件失败：不存在可以订阅的事件类型 " + type.Name + " ！");
            }
        }
        /// <summary>
        /// 订阅二型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Subscribe(Type type, HTFAction handler)
        {
            if (EventHandlerList2.ContainsKey(type))
            {
                EventHandlerList2[type] += handler;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Event, "订阅II型事件失败：不存在可以订阅的事件类型 " + type.Name + " ！");
            }
        }
        /// <summary>
        /// 订阅三型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Subscribe(Type type, HTFAction<EventHandlerBase> handler)
        {
            if (EventHandlerList3.ContainsKey(type))
            {
                EventHandlerList3[type] += handler;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Event, "订阅III型事件失败：不存在可以订阅的事件类型 " + type.Name + " ！");
            }
        }
        /// <summary>
        /// 取消订阅一型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe(Type type, HTFAction<object, EventHandlerBase> handler)
        {
            if (EventHandlerList1.ContainsKey(type))
            {
                EventHandlerList1[type] -= handler;
            }
        }
        /// <summary>
        /// 取消订阅二型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe(Type type, HTFAction handler)
        {
            if (EventHandlerList2.ContainsKey(type))
            {
                EventHandlerList2[type] -= handler;
            }
        }
        /// <summary>
        /// 取消订阅三型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe(Type type, HTFAction<EventHandlerBase> handler)
        {
            if (EventHandlerList3.ContainsKey(type))
            {
                EventHandlerList3[type] -= handler;
            }
        }
        /// <summary>
        /// 清空已订阅的事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        public void ClearSubscribe(Type type)
        {
            if (EventHandlerList1.ContainsKey(type))
            {
                EventHandlerList1[type] = null;
            }
            if (EventHandlerList2.ContainsKey(type))
            {
                EventHandlerList2[type] = null;
            }
            if (EventHandlerList3.ContainsKey(type))
            {
                EventHandlerList3[type] = null;
            }
        }

        /// <summary>
        /// 抛出一型事件（抛出事件时，请使用引用池生成事件处理者实例）
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="handler">事件处理类实例</param>
        public void Throw(object sender, EventHandlerBase handler)
        {
            Type type = handler.GetType();
            if (EventHandlerList1.ContainsKey(type))
            {
                if (EventHandlerList1[type] != null)
                {
                    EventHandlerList1[type](sender, handler);
                    Main.m_ReferencePool.Despawn(handler);
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Event, "抛出I型事件失败：不存在可以抛出的事件类型 " + type.Name + " ！");
            }
        }
        /// <summary>
        /// 抛出二型事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        public void Throw(Type type)
        {
            if (EventHandlerList2.ContainsKey(type))
            {
                EventHandlerList2[type]?.Invoke();
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Event, "抛出II型事件失败：不存在可以抛出的事件类型 " + type.Name + " ！");
            }
        }
        /// <summary>
        /// 抛出三型事件（抛出事件时，请使用引用池生成事件处理者实例）
        /// </summary>
        /// <param name="handler">事件处理类实例</param>
        public void Throw(EventHandlerBase handler)
        {
            Type type = handler.GetType();
            if (EventHandlerList3.ContainsKey(type))
            {
                if (EventHandlerList3[type] != null)
                {
                    EventHandlerList3[type](handler);
                    Main.m_ReferencePool.Despawn(handler);
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Event, "抛出III型事件失败：不存在可以抛出的事件类型 " + type.Name + " ！");
            }
        }
    }
}