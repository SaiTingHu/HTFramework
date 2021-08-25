using System;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// UI逻辑基类
    /// </summary>
    public abstract class UILogicBase
    {
        /// <summary>
        /// UI实体
        /// </summary>
        public GameObject UIEntity { get; internal set; }
        /// <summary>
        /// UI是否打开
        /// </summary>
        public bool IsOpened
        {
            get
            {
                return UIEntity ? UIEntity.activeSelf : false;
            }
        }
        /// <summary>
        /// UI实体是否已创建
        /// </summary>
        public bool IsCreated
        {
            get
            {
                return UIEntity;
            }
        }
        /// <summary>
        /// 是否支持数据驱动
        /// </summary>
        public bool IsSupportedDataDriver
        {
            get
            {
                return Array.Exists(GetType().GetInterfaces(), t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IDataDriver<>));
            }
        }
        /// <summary>
        /// 是否启用自动化，这将造成反射的性能消耗
        /// </summary>
        protected virtual bool IsAutomate => true;

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void OnInit()
        {
            if (IsAutomate)
            {
                FieldInfo[] fieldInfos = AutomaticTask.GetAutomaticFields(GetType());
                AutomaticTask.ApplyObjectPath(this, fieldInfos);

                if (IsSupportedDataDriver)
                {
                    AutomaticTask.ApplyDataBinding(this, fieldInfos);
                }
            }
        }
        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="args">可选参数</param>
        public virtual void OnOpen(params object[] args)
        {
            Main.m_Event.Throw(Main.m_ReferencePool.Spawn<EventUIOpened>().Fill(this));
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        public virtual void OnClose()
        {
            Main.m_Event.Throw(Main.m_ReferencePool.Spawn<EventUIClosed>().Fill(this));
        }
        /// <summary>
        /// 销毁UI
        /// </summary>
        public virtual void OnDestroy()
        {
            if (IsAutomate && IsSupportedDataDriver)
            {
                AutomaticTask.ClearDataBinding(this);
            }
        }
        /// <summary>
        /// UI逻辑刷新
        /// </summary>
        public virtual void OnUpdate()
        {
        }
        /// <summary>
        /// 打开自己
        /// </summary>
        protected void Open()
        {
            Main.m_UI.OpenUI(GetType());
        }
        /// <summary>
        /// 关闭自己
        /// </summary>
        protected void Close()
        {
            Main.m_UI.CloseUI(GetType());
        }
    }
}