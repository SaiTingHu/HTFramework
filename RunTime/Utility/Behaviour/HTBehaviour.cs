using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// HT框架公共行为脚本基类
    /// </summary>
    public abstract class HTBehaviour : MonoBehaviour, ISafetyCheckTarget
    {
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

        protected virtual void Awake()
        {
            useGUILayout = false;

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

            if (Main.Current) Main.Current.RegisterBehaviour(this);

#if UNITY_EDITOR
            SafetyChecker.DoSafetyCheck(this, injectCount, bindCount);
#endif
        }
        protected virtual void OnDestroy()
        {
            if (Main.Current) Main.Current.UnregisterBehaviour(this);
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