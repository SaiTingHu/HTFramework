using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// UI逻辑基类
    /// </summary>
    public abstract class UILogicBase : ISafetyCheckTarget
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
                return this is IDataDriver;
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
            int injectCount = 0;
            int bindCount = 0;

            if (IsAutomate)
            {
                FieldInfo[] fieldInfos = AutomaticTask.GetAutomaticFields(GetType());
                injectCount = AutomaticTask.ApplyInject(this, fieldInfos);

                if (IsSupportedDataDriver)
                {
                    bindCount = AutomaticTask.ApplyDataBinding(this, fieldInfos);
                }
            }

#if UNITY_EDITOR
            SafetyChecker.DoSafetyCheck(this, injectCount, bindCount);
#endif
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

        /// <summary>
        /// 性能及安全性检查
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns>检查是否通过</returns>
        public virtual bool OnSafetyCheck(params object[] args)
        {
            if (args.Length < 2)
                return true;

            int injectCount = (int)args[0];
            int bindCount = (int)args[1];

            if (IsAutomate)
            {
                if (injectCount <= 0 && bindCount <= 0)
                {
                    string content = string.Format("【{0}】启用了自动化任务，但不存在任何依赖注入字段[Inject]，和数据绑定字段[DataBind]，请考虑关闭自动化任务（IsAutomate = false）！", GetType().FullName);
                    SafetyChecker.DoSafetyWarning(content);
                    return false;
                }

                if (IsSupportedDataDriver)
                {
                    if (bindCount <= 0)
                    {
                        string content = string.Format("【{0}】实现了数据驱动接口（IDataDriver），但不存在任何数据绑定字段[DataBind]，请考虑移除数据驱动接口！", GetType().FullName);
                        SafetyChecker.DoSafetyWarning(content);
                        return false;
                    }
                }
            }
            return true;
        }
    }
}