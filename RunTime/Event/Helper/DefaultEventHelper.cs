using System;
using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 默认的事件管理器助手
    /// </summary>
    internal sealed class DefaultEventHelper : IEventHelper
    {
        /// <summary>
        /// 所属的内置模块
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 事件列表
        /// </summary>
        public Dictionary<Type, HTFAction<EventHandlerBase>> EventHandlerList { get; private set; } = new Dictionary<Type, HTFAction<EventHandlerBase>>();

        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit()
        {
            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
            {
                return type.IsSubclassOf(typeof(EventHandlerBase)) && !type.IsAbstract;
            });
            for (int i = 0; i < types.Count; i++)
            {
                EventHandlerList.Add(types[i], null);
            }
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnReady()
        {

        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate()
        {
            
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        {
            EventHandlerList.Clear();
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
        public void OnResume()
        {

        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Subscribe(Type type, HTFAction<EventHandlerBase> handler)
        {
            if (EventHandlerList.ContainsKey(type))
            {
                EventHandlerList[type] += handler;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Event, $"订阅事件失败：不存在可以订阅的事件类型 {type.Name} ！");
            }
        }
        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        /// <param name="handler">事件处理者</param>
        public void Unsubscribe(Type type, HTFAction<EventHandlerBase> handler)
        {
            if (EventHandlerList.ContainsKey(type))
            {
                EventHandlerList[type] -= handler;
            }
        }
        /// <summary>
        /// 清空已订阅的事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        public void ClearSubscribe(Type type)
        {
            if (EventHandlerList.ContainsKey(type))
            {
                EventHandlerList[type] = null;
            }
        }
        /// <summary>
        /// 抛出事件
        /// </summary>
        /// <param name="type">事件处理类</param>
        public void Throw(Type type)
        {
            if (EventHandlerList.ContainsKey(type))
            {
                if (EventHandlerList[type] != null)
                {
                    EventHandlerList[type](null);
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Event, $"抛出事件失败：不存在可以抛出的事件类型 {type.Name} ！");
            }
        }
        /// <summary>
        /// 抛出事件（抛出事件时，请使用引用池生成事件处理者实例）
        /// </summary>
        /// <param name="handler">事件处理类实例</param>
        public void Throw(EventHandlerBase handler)
        {
            if (handler == null)
                return;

            Type type = handler.GetType();
            if (EventHandlerList.ContainsKey(type))
            {
                if (EventHandlerList[type] != null)
                {
                    EventHandlerList[type](handler);
                    Main.m_ReferencePool.Despawn(handler);
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Event, $"抛出事件失败：不存在可以抛出的事件类型 {type.Name} ！");
            }
        }
    }
}