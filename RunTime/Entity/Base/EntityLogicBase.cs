using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 实体逻辑基类
    /// </summary>
    public abstract class EntityLogicBase : IReference, ISafetyCheckTarget
    {
        /// <summary>
        /// 实体名称
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// 实体
        /// </summary>
        public GameObject Entity { get; internal set; }
        /// <summary>
        /// 实体是否显示
        /// </summary>
        public bool IsShowed
        {
            get
            {
                return Entity ? Entity.activeSelf : false;
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
        protected virtual bool IsAutomate => false;

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
        /// 显示实体
        /// </summary>
        public virtual void OnShow()
        {

        }
        /// <summary>
        /// 隐藏实体
        /// </summary>
        public virtual void OnHide()
        {

        }
        /// <summary>
        /// 销毁实体
        /// </summary>
        public virtual void OnDestroy()
        {
            
        }
        /// <summary>
        /// 实体逻辑刷新
        /// </summary>
        public virtual void OnUpdate()
        {

        }
        /// <summary>
        /// 重置实体
        /// </summary>
        public virtual void Reset()
        {

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
                    string content = $"【{GetType().FullName}】启用了自动化任务，但不存在任何依赖注入字段[Inject]，和数据绑定字段[DataBind]，请考虑关闭自动化任务（IsAutomate = false）！";
                    SafetyChecker.DoSafetyWarning(content);
                    return false;
                }

                if (IsSupportedDataDriver)
                {
                    if (bindCount <= 0)
                    {
                        string content = $"【{GetType().FullName}】实现了数据驱动接口（IDataDriver），但不存在任何数据绑定字段[DataBind]，请考虑移除数据驱动接口！";
                        SafetyChecker.DoSafetyWarning(content);
                        return false;
                    }
                }
            }
            return true;
        }
    }
}